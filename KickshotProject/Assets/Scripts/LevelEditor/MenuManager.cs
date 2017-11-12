using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using UnityEngine.SceneManagement;

namespace LevelEditor
{
    public class MenuManager : MonoBehaviour
    {
        private LevelData levelData;

        public void NewLevel() {
            Debug.Log("New Level");
            CreateLevelData("");
            SceneManager.LoadScene("LevelEditor");
        }

        public void LoadLevel() {
            string[] paths = StandaloneFileBrowser.OpenFilePanel("", "", "", false);
            if (paths[0] == "")
            {
                Debug.Log("No selection made");
                return;
            }
            string path = paths[0];
            Debug.Log("Path: " + path);

            CreateLevelData(path);

            SceneManager.LoadScene("LevelEditor");
        }

        public void MainMenu() {
            //TODO Once a main menu is made, load the scene
            SceneManager.LoadScene("MainMenu");
            Debug.Log("Going to main menu");
        }

        private void CreateLevelData(string levelPath)
        {
            GameObject lvlDataObject = GameObject.Find("LevelData");
            if (lvlDataObject == null)
            {
                levelData = new GameObject().AddComponent<LevelData>();
                levelData.transform.parent = SceneOrganizationManager.Instance.DataParent;
                levelData.name = "LevelData";
            } else {
                levelData = lvlDataObject.GetComponent<LevelData>();
            }
            if (levelPath == "") {
                return;
            }
            levelData.levelPath = levelPath;
            levelData.loadLevel = true;
        }
    }

    public class LevelData : MonoBehaviour 
    {
        public bool loadLevel = false;
        public string levelPath = "";
    }
}