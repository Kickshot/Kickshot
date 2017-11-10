using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    /// <summary>
    /// Class containing the prefab database and helper methods.
    /// </summary>
    public class LevelObjectDatabase : Singleton<LevelObjectDatabase>
    {

        private Dictionary<int, string> objects = new Dictionary<int, string>();

        /// <summary>
        /// Gets the prefab path from the local prefab dictionary.
        /// </summary>
        /// <returns>The prefab path.</returns>
        /// <param name="prefabID">Prefab identifier.</param>
        public string GetPrefabPath(int prefabID)
        {
            string path;
            if (!objects.TryGetValue(prefabID, out path))
                throw new UnityException(string.Format("Failed to find prefab path with id {0}", prefabID));
            return path;
        }

        /// <summary>
        /// Gets the prefab identifier from a path to prefab.
        /// </summary>
        /// <returns>The prefab identifier.</returns>
        /// <param name="prefabPath">Prefab path.</param>
        public int GetPrefabID(string prefabPath)
        {
            if (!objects.ContainsValue(prefabPath))
                throw new UnityException(string.Format("Failed to find entry for value {0}", prefabPath));
            foreach(KeyValuePair<int, string> obj in objects) {
                if (obj.Value.Equals(prefabPath))
                    return obj.Key;
            }
            throw new UnityException("Contains path, but failed to find it. Big fuckup if this goes");
        }

        /// <summary>
        /// Gets a prefab game object from its ID.
        /// </summary>
        /// <returns>The prefab game object.</returns>
        /// <param name="prefabID">Prefab identifier.</param>
        public GameObject GetPrefabGameObject(int prefabID)
        {
            GameObject prefab;
            try
            {
                string path = GetPrefabPath(prefabID);
                prefab = Resources.Load(path) as GameObject;
                return prefab;
            }
            catch (UnityException e) {
                throw e;
            }
        }

        /// <summary>
        /// Adds a new level object to the database and returns its ID
        /// </summary>
        /// <returns>Level object ID</returns>
        /// <param name="prefabPath">Prefab path.</param>
        public int AddLevelObject(string prefabPath)
        {
            int id = prefabPath.GetHashCode();
            objects.Add(id, prefabPath);
            return id;
        }
    }
}