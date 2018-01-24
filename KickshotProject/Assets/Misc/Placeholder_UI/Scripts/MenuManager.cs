using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    public GameObject mainMenu, levelSelect;

    void Awake()
    {
        levelSelect.transform.localScale = new Vector3(0, 0, 0);
    }
    // Opens the Level Select
    public void ClickPlay()
    {
        mainMenu.transform.localScale = new Vector3(0, 0, 0);
        levelSelect.transform.localScale = new Vector3(1, 1, 1);
    }
    // Quits the game
    public void ClickExit()
    {
        Application.Quit();
    }
    // Returns to the Main Menu
    public void ClickBack()
    {
        mainMenu.transform.localScale = new Vector3(1, 1, 1);
        levelSelect.transform.localScale = new Vector3(0, 0, 0);
    }
    // Load the Islands Level.
    // The plan is to eventually make clicking the level change the flyby in the background. 
    // Then have a "Select" button which loads the level.
    public void ClickIslands()
    {
        Debug.Log("Islands");
        SceneManager.LoadScene("Islands_1");
    }

    public void LevelSelect(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
