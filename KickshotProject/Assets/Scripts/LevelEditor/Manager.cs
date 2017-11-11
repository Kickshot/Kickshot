using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    public class Manager : Singleton<Manager>
    {

        public string objectTag = "LevelObject";
        public GameObject activeSelection;

        private Camera cam;
        private LevelData levelData;

        private void Start()
        {
            cam = Camera.main;
            if (cam == null)
                throw new UnityException("Failed to find camera for Level Editor");
        }

        private void Awake()
        {
            GameObject lvlDataObject = GameObject.Find("LevelData");
            if (lvlDataObject != null)
                levelData = lvlDataObject.GetComponent<LevelData>();
            else
            {
                Debug.Log("Could not find LevelData, generating empty");
                levelData = new GameObject().AddComponent<LevelData>();
                levelData.gameObject.name = "LevelData";
                levelData.transform.parent = SceneOrganizationManager.Instance.DataParent;
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) {
                
            }
        }

        public void Export()
        {
            List<GameObject> objs = new List<GameObject>(GameObject.FindGameObjectsWithTag(objectTag));
            LevelObject testObj = ComputeTransformStruct(objs[0]);
            Debug.Log(JsonUtility.ToJson(testObj));
        }

        private LevelObject ComputeTransformStruct(GameObject o)
        {
            LevelObject ret;
            ret.name = o.name;
            ret.position = o.transform.position;
            ret.rotation = o.transform.rotation;
            ret.scale = o.transform.localScale;
            return ret;
        }
    }

    [System.Serializable]
    public struct LevelObject
    {
        public string name;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

}