#if UNITY_EDITOR

using System;
using System.Collections.Generic;

namespace Viking.Keys
{
    /// <summary>
    /// Viking Keys; List of keys.
    /// </summary>
    [Serializable]
    public class Keys
    {
        /// <summary>
        /// List of keys.
        /// </summary>
        public List<Key> keys = new List<Key>();
    }
}

#endif
