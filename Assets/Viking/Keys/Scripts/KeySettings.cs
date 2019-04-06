#if UNITY_EDITOR

using UnityEngine;

namespace Viking.Keys
{
    /// <summary>
    /// Settings for the KeysWindow.
    /// </summary>
    public class KeySettings : ScriptableObject
    {
        /// <summary>
        /// Key length.
        /// </summary>
        public int length = 12;

        /// <summary>
        /// Bounds restricting key length; Minimum and maximum.
        /// </summary>
        public Vector2Int lengthBounds = new Vector2Int(1, 24);

        /// <summary>
        /// Key formats.
        /// </summary>
        public enum Format
        {
            Letter, // just letters
            Alpha,  // just numbers
            Mix     // both
        }

        /// <summary>
        /// Key format.
        /// </summary>
        public Format format = Format.Letter;

        /// <summary>
        /// Key styles.
        /// </summary>
        public enum Style
        {
            Upper,  // all uppercase
            Lower,  // all lowercase
            Mix     // both
        }

        /// <summary>
        /// Key style.
        /// </summary>
        public Style style = Style.Upper;

        /// <summary>
        /// Use special characters?
        /// </summary>
        public bool special = false;

        /// <summary>
        /// Special characters to use.
        /// </summary>
        public string characters = "!$?";

        /// <summary>
        /// Percent chance a character is special.
        /// </summary>
        public int chance = 20;

        /// <summary>
        /// Bounds restricting special character chance; Minimum and maximum.
        /// </summary>
        public Vector2Int chanceBounds = new Vector2Int(0, 100);
        
        /// <summary>
        /// Use hyphens?
        /// </summary>
        public bool hyphen = false;

        /// <summary>
        /// How many characters between hyphens.
        /// </summary>
        public int groupSize = 4;

        /// <summary>
        /// Bounds restricting hyphen group size; Minimum and maximum.
        /// </summary>
        public Vector2Int groupBounds = new Vector2Int(2, 12);

        /// <summary>
        /// How many keys to generate.
        /// </summary>
        public int quantity = 10;

        /// <summary>
        /// Bounds restricting key generation quantity; Minimum and maximum.
        /// </summary>
        public Vector2Int quantityBounds = new Vector2Int(1, 100);

        /// <summary>
        /// Sample of a key per current settings.
        /// </summary>
        public string preview;

        /// <summary>
        /// Path to save the keys to.
        /// </summary>
        public string path;

        /// <summary>
        /// List of generate keys.
        /// </summary>
        public Keys keys = new Keys();

        /// <summary>
        /// List export options.
        /// </summary>
        public enum Export
        {
            Text, Json
        }

        /// <summary>
        /// List export format.
        /// </summary>
        public Export export;
    }
}

#endif
