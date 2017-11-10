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

            SceneManager.LoadScene("LevelEditor");
        }

        public void MainMenu() {
            //TODO Once a main menu is made load the scene
            Debug.Log("Going to main menu");
        }

        private void CreateLevelData(string levelPath)
        {
            levelData = GameObject.Find("LevelData").GetComponent<LevelData>();
            if (levelData == null)
            {
                levelData = Instantiate(new LevelData(), Vector3.zero, Quaternion.identity, SceneOrganizationManager.Instance.DataParent);
                levelData.name = "LevelData";
                DontDestroyOnLoad(levelData);
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