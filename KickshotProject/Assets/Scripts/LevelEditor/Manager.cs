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
            levelData = GameObject.Find("LevelData").GetComponent<LevelData>();
            if (levelData == null)
                levelData = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity, SceneOrganizationManager.Instance.DataParent).GetComponent<LevelData>();
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