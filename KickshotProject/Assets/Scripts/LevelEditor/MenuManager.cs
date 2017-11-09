using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using UnityEngine.SceneManagement;

namespace LevelEditor
{
    public class MenuManager : MonoBehaviour
    {
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
    }
}