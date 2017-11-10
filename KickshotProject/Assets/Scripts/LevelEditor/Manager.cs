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
        private LoadPath levelData;

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
                levelData = lvlDataObject.GetComponent<LoadPath>();
            else
            {
                Debug.Log("Could not find LevelData, generating empty");
                levelData = new GameObject().AddComponent<LoadPath>();
                levelData.gameObject.name = "LevelData";
                levelData.transform.parent = SceneOrganizationManager.Instance.DataParent;
            }
        }

        public void Export()
        {
            //Level data structure used for JSON serialization
            LevelData level = new LevelData();

            LevelObject[] objects = FindObjectsOfType<LevelObject>();
            for (int i = 0; i < objects.Length; i++) {
                LevelObjectData data = new LevelObjectData();
                data.objectID = objects[i].objectID;
                data.name = objects[i].name;
                data.position = objects[i].transform.position;
                data.rotation = objects[i].transform.eulerAngles;
                data.scale = objects[i].transform.localScale;
                level.objects.Add(data);
            }

            //For now just log it
            //TODO open up a dialog to export it to a file location
            Debug.Log(JsonUtility.ToJson(level));
        }
    }

    [System.Serializable]
    public class LevelData
    {
        public List<LevelObjectData> objects;
    }
}