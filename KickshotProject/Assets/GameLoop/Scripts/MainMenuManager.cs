using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

    public string levelsScene;

    public void Play() {
        SceneManager.LoadScene(levelsScene);
    }

    public void Options() {
        Debug.Log("Options Trigger");
    }

    public void Quit() {
        Application.Quit();
    }
}
