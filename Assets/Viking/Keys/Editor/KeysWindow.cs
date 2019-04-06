using System.IO;
using UnityEngine;
using UnityEditor;
using static Viking.Keys.KeySettings;

namespace Viking.Keys
{
    /// <summary>
    /// KeysWindow; Customize and generate keys.
    /// </summary>
    public class KeysWindow : EditorWindow
    {
        #region Variables

        /// <summary>
        /// Editor window.
        /// </summary>
        private static KeysWindow window;

        /// <summary>
        /// Editor window settings.
        /// </summary>
        private KeySettings settings;

        /// <summary>
        /// Position of window scroll view.
        /// </summary>
        private Vector2 scroll = Vector2.zero;

        /// <summary>
        /// Letters for keys; Alphabet.
        /// </summary>
        private char[] letters = new char[]
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
        };

        /// <summary>
        /// Numbers for keys; Numeric.
        /// </summary>
        private char[] numbers = new char[]
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        #endregion

        /// <summary>
        /// Initialize window.
        /// </summary>
        [MenuItem("Viking/Keys")]
        private static void Init()
        {
            window = (KeysWindow)GetWindow(typeof(KeysWindow), true, "Viking Keys");

            window.Show();
        }

        #region EnableDisable

        /// <summary>
        /// When window is enabled; Active.
        /// </summary>
        private void OnEnable()
        {            
            // search for window settings
            string[] guids = AssetDatabase.FindAssets("t:keySettings");

            // load if found
            if (guids.Length > 0)
            {
                settings = AssetDatabase.LoadAssetAtPath<KeySettings>(AssetDatabase.GUIDToAssetPath(guids[0]));
            }
            else // otherwise initialize new
            {
                InitializeSettings();
            }

            Preview();
        }

        /// <summary>
        /// When window is disabled; Closed.
        /// </summary>
        private void OnDisable()
        {
            // save current window settings
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }

        #endregion

        /// <summary>
        /// Render window.
        /// </summary>
        private void OnGUI()
        {
            // toolbar
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
            if (GUILayout.Button(new GUIContent("Generate", "Generate a list of keys the length of the set quantity?"), EditorStyles.toolbarButton))
            {
                GenerateKeys();
            }
            if (GUILayout.Button(new GUIContent("Save", "Save the generated list of keys to a file?"), EditorStyles.toolbarButton))
            {
                // display dialogue if keys aren't generated
                if (settings.keys.keys.Count == 0)
                {
                    EditorUtility.DisplayDialog("Oops!", "You need to generate a list of keys first!", "Okay!");
                }
                else // otherwise prompt to save
                {
                    Save();
                }
            }
            settings.export = (Export)EditorGUILayout.EnumPopup(settings.export, EditorStyles.toolbarDropDown, GUILayout.Width(56));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Reset", "Reset the window settings?"), EditorStyles.toolbarButton))
            {
                InitializeSettings();
            }
            EditorGUILayout.EndHorizontal();

            // key preview
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUIStyle c = new GUIStyle(EditorStyles.boldLabel);
            c.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField(new GUIContent("Preview", "How the keys will generally look."), c);
            GUIStyle p = new GUIStyle(EditorStyles.helpBox);
            p.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField(settings.preview, p);
            EditorGUILayout.EndVertical();

            // key settings
            EditorGUILayout.BeginVertical();
            scroll = EditorGUILayout.BeginScrollView(scroll);

            // key length
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Length", "How long should the keys be?"));
            settings.length = EditorGUILayout.IntSlider(settings.length, settings.lengthBounds.x, settings.lengthBounds.y);
            EditorGUILayout.EndVertical();

            // key format
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Format", "Should the keys be all letters, numbers, or mixed? ie. abc 123 a2c"));
            settings.format = (Format)EditorGUILayout.EnumPopup(settings.format);

            EditorGUILayout.Space();

            // key style
            EditorGUILayout.LabelField(new GUIContent("Style", "Should the keys be all upper case, lower, or mixed? ie. AAA bbb CcC"));
            settings.style = (Style)EditorGUILayout.EnumPopup(settings.style);
            EditorGUILayout.EndVertical();

            // key special character options
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            settings.special = EditorGUILayout.BeginToggleGroup(new GUIContent("Special", "Should the keys contain special characters? ie. !?"), settings.special);
            EditorGUILayout.LabelField(new GUIContent("Characters", "Which special characters to include?"));
            settings.characters = EditorGUILayout.TextArea(settings.characters);
            EditorGUILayout.LabelField(new GUIContent("Chance", "What percent chance should there be a special character?"));
            settings.chance = EditorGUILayout.IntSlider(settings.chance, settings.chanceBounds.x, settings.chanceBounds.y);
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndVertical();

            // key hyphen options
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            settings.hyphen = EditorGUILayout.BeginToggleGroup(new GUIContent("Hyphen", "Should the keys contain hyphens? ie. AbbA-BccB"), settings.hyphen);
            EditorGUILayout.LabelField(new GUIContent("Group", "How many characters between hyphens?"));
            settings.groupSize = EditorGUILayout.IntSlider(settings.groupSize, settings.groupBounds.x, settings.groupBounds.y);
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndVertical();

            // key generation
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Quantity", "How many keys to generate?"));
            settings.quantity = EditorGUILayout.IntSlider(settings.quantity, settings.quantityBounds.x, settings.quantityBounds.y);
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            
            // update preview on changes
            if (GUI.changed)
            {
                Preview();
            }
        }

        #region Helpers

        /// <summary>
        /// Generate a key preview.
        /// </summary>
        private void Preview()
        {
            settings.preview = GenerateKey();
        }

        #region Generation

        /// <summary>
        /// Generate a key.
        /// </summary>
        /// <returns>Generated key per settings.</returns>
        private string GenerateKey()
        {
            string temp = "";

            // iterate each character
            for (int i = 0; i < settings.length; i++)
            {
                // check if there should be hyphens
                if (settings.hyphen)
                {
                    // add a hyphen after a group
                    if (i % settings.groupSize == 0 && i > settings.groupSize - 1)
                    {
                        temp += "-";
                    }
                }

                // check if there should be special characters
                if (settings.special)
                {
                    // roll based on chance percentage
                    if (Random.Range(0, 101) < settings.chance)
                    {
                        // add a special character
                        temp = Special(temp);
                        continue;
                    }
                }

                // add a character
                temp = Character(temp);
            }

            return temp;
        }

        /// <summary>
        /// Generate a list of keys.
        /// </summary>
        private void GenerateKeys()
        {
            // reset current keys
            settings.keys.keys.Clear();

            // iterate list length
            for (int i = 0; i < settings.quantity; i++)
            {
                // initialize a new key
                Key key = new Key();

                do
                {
                    // generate a key
                    key.key = GenerateKey();
                }
                while (settings.keys.keys.Contains(key));   // repeat if there's a duplicate

                // add the new key to the list
                settings.keys.keys.Add(key);
            }
        }

        /// <summary>
        /// Add a character to a key.
        /// </summary>
        /// <param name="temp">Temporary key.</param>
        /// <returns>Appended key.</returns>
        private string Character(string temp)
        {
            if (settings.format == Format.Letter)       // if key format is letters
            {
                // add a letter
                temp = Letter(temp);
            }
            else if (settings.format == Format.Alpha)   // if keyformat is numbers
            {
                // add a number
                temp = Number(temp);
            }
            else if (settings.format == Format.Mix)     // if key format is both
            {
                // roll for either a letter or number
                if (Random.Range(0, 2) == 1)
                {
                    temp = Letter(temp);    // add a letter
                }
                else
                {
                    temp = Number(temp);    // add a number
                }
            }

            return temp;
        }

        /// <summary>
        /// Add a letter to a key.
        /// </summary>
        /// <param name="temp">Temporary key.</param>
        /// <returns>Appended key.</returns>
        private string Letter(string temp)
        {
            // get a random letter
            char l = letters[Random.Range(0, letters.Length)];

            // depending on key style
            switch (settings.style)
            {
                // add the letter as uppercase
                case Style.Upper:
                    temp += l.ToString().ToUpper();
                    break;
                // add the letter as lowercase
                case Style.Lower:
                    temp += l.ToString();
                    break;
                // add the letter as either upper or lowercase
                case Style.Mix:
                    temp += (Random.Range(0, 2) == 1) ? l.ToString().ToUpper() : l.ToString();
                    break;
            }

            return temp;
        }

        /// <summary>
        /// Add a number to a key.
        /// </summary>
        /// <param name="temp">Temporary key.</param>
        /// <returns>Appended key.</returns>
        private string Number(string temp)
        {
            // get a random number
            char n = numbers[Random.Range(0, numbers.Length)];

            // add number
            temp += n;

            return temp;
        }

        /// <summary>
        /// Add a special character to a key.
        /// </summary>
        /// <param name="temp">Temporary key.</param>
        /// <returns>Appended key.</returns>
        private string Special(string temp)
        {
            // if special characters aren't specified
            if (settings.characters.Length == 0)
            {
                // add a character
                temp = Character(temp);
            }
            else
            {
                // convert special character list to an array
                char[] specials = settings.characters.ToCharArray();

                // get a special character
                char s = specials[Random.Range(0, specials.Length)];

                // add special character
                temp += s;
            }

            return temp;
        }

        #endregion

        #region IO

        /// <summary>
        /// Save generated keys to file.
        /// </summary>
        private void Save()
        {
            // check how to export the keys
            switch (settings.export)
            {
                // export as a plain text file
                case Export.Text:
                    ExportText();
                    break;
                // export as json
                case Export.Json:
                    ExportJson();
                    break;
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Export keys as a plain text file.
        /// </summary>
        private void ExportText()
        {
            string text = "";

            // add each key to a new line
            for (int i = 0; i < settings.keys.keys.Count; i++)
            {
                text += settings.keys.keys[i].key + "\n";
            }

            // prompt for save location
            settings.path = EditorUtility.SaveFilePanel("Save generated keys", "", "keys.txt", "txt");

            // save file
            if (settings.path.Length != 0)
            {
                File.WriteAllText(settings.path, text);
            }
        }

        /// <summary>
        /// Export keys as a json file.
        /// </summary>
        private void ExportJson()
        {
            // serialize keys to json
            string json = JsonUtility.ToJson(settings.keys, true);

            // prompt for save location
            settings.path = EditorUtility.SaveFilePanel("Save generated keys", "", "keys.json", "json");

            // save file
            if (settings.path.Length != 0)
            {
                File.WriteAllText(settings.path, json);
            }
        }

        /// <summary>
        /// Create new key settings.
        /// </summary>
        private void InitializeSettings()
        {
            settings = CreateInstance<KeySettings>();

            AssetDatabase.CreateAsset(settings, "Assets/Viking/Keys/KeySettings.asset");
        }

        #endregion
        #endregion
    }
}
