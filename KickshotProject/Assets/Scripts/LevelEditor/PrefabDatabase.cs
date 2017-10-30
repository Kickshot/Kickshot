using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// Class containing the prefab database and helper methods.
    /// </summary>
    public class PrefabDatabase : Singleton<PrefabDatabase>
    {

        //static Dictionary<int, string> prefabs = new Dictionary<int, string>();

        /// <summary>
        /// Gets the prefab path from the local prefab dictionary.
        /// </summary>
        /// <returns>The prefab path.</returns>
        /// <param name="prefabID">Prefab identifier.</param>
        public string GetPrefabPath(int prefabID)
        {
            return "";
        }

        /// <summary>
        /// Gets the prefab identifier from a path to prefab.
        /// </summary>
        /// <returns>The prefab identifier.</returns>
        /// <param name="prefabPath">Prefab path.</param>
        public int GetPrefabID(string prefabPath)
        {
            return 0;
        }

        /// <summary>
        /// Gets a prefab game object from its ID.
        /// </summary>
        /// <returns>The prefab game object.</returns>
        /// <param name="prefabID">Prefab identifier.</param>
        public GameObject GetPrefabGameObject(int prefabID)
        {
            return null;
        }

        /// <summary>
        /// Gets the prefab game object from its path.
        /// </summary>
        /// <returns>The prefab game object.</returns>
        /// <param name="prefabPath">Prefab path.</param>
        public GameObject GetPrefabGameObject(string prefabPath)
        {
            return null;
        }
    }
}