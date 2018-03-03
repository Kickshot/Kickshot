using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompleteScreenManager : MonoBehaviour
{

    // used to disable camera/movement
    [HideInInspector]
    public GameObject player, gun;

    public GameObject levelCompleteScreen, crosshair;
    public Text HUDTime, completeTime;
    public InGameGUIManager guiManager;

    public void DisplayLevelCompleteMenu()
    {
        guiManager.activeMenu = true;
        Time.timeScale = 0;
        player = GameObject.Find("SourcePlayer");
        gun = GameObject.Find("DoubleGun");
        player.GetComponent<MouseLook>().enabled = false;
        player.GetComponent<CharacterController>().enabled = false;
        player.GetComponent<SourcePlayer>().enabled = false;
        gun.SetActive(false);
        crosshair.SetActive(false);
        levelCompleteScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        completeTime.text = HUDTime.text;
        HUDTime.enabled = false;
        Debug.Log(HUDTime.text);
    }

    public void ClickMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void ClickNextLevel()
    {
        Time.timeScale = 1;
        //TODO: This will break if the build settings are changed at all. Probably better to adopt a "level1, level2" naming standard.
        int nextSceneBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneBuildIndex == 10 || nextSceneBuildIndex == 13)
        {
            SceneManager.LoadScene("MainMenu");
        } else {
            SceneManager.LoadScene(nextSceneBuildIndex);
        }
    }
}
