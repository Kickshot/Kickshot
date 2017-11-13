using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LevelEditor
{
    /// <summary>
    /// Scene organization manager.
    /// [DATA]      used for all inter-scene communication and data (Persists through scene loads).
    /// [WORLD]     used for all world objects such as terrain and iteractables.
    /// [OTHER]     used for other game objects, UI, Canvas, EventSystem.
    /// [MANAGERS]  used to store empty GameObjects of all managers in the scene (Does not persist through scene loads).
    /// </summary>
    public class SceneOrganizationManager : Singleton<SceneOrganizationManager>
    {
        /// <summary>
        /// Generates all parent transforms if checked
        /// </summary>
        public bool generateOnAwake = false;

        public Transform DataParent
        {
            get
            {
                if (dataParent == null)
                {
                    dataParent = CreateParent("DATA");
                    DontDestroyOnLoad(dataParent.gameObject);
                }
                return dataParent;
            }
        }
        public Transform ManagerParent
        {
            get
            {
                if (managerParent == null)
                    managerParent = CreateParent("MANAGERS");
                return managerParent;
            }
        }
        public Transform OtherParent
        {
            get
            {
                if (otherParent == null)
                    otherParent = CreateParent("OTHER");
                return otherParent;
            }
        }
        public Transform WorldParent
        {
            get
            {
                if (worldParent == null)
                    worldParent = CreateParent("WORLD");
                return worldParent;
            }
        }

        private Transform dataParent;
        private Transform managerParent;
        private Transform otherParent;
        private Transform worldParent;

        private void Awake()
        {
            if (!generateOnAwake) return;
            Generate();
        }

        private void Generate()
        {
            GameObject data = GameObject.Find("[DATA]");
            dataParent = data == null ? CreateParent("DATA") : data.transform;
            GameObject managers = GameObject.Find("[MANAGERS]");
            managerParent = managers == null ? CreateParent("MANAGERS") : managers.transform;
            GameObject other = GameObject.Find("[OTHER]");
            otherParent = other == null ? CreateParent("OTHER") : other.transform;
            GameObject world = GameObject.Find("[WORLD]");
            worldParent = world == null ? CreateParent("WORLD") : world.transform;
        }

        private Transform CreateParent(string parentName)
        {
            Transform parent;
            parent = new GameObject().transform;
            parent.name = string.Format("[{0}]", parentName);
            return parent;
        }

#if UNITY_EDITOR
        [MenuItem("Tools/Level Editor/Generate Empty Parents")]
        public static void GenerateParents()
        {
            Instance.Generate();
        }
#endif
    }
}