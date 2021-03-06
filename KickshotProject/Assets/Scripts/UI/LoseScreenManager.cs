﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseScreenManager : MonoBehaviour {

    // used to disable camera/movement
    [HideInInspector]
    public GameObject crosshair, loseScreen;

    public InGameGUIManager guiManager;

    public void DisplayLoseScreen()
    {
        guiManager.activeMenu = true;
        crosshair.SetActive(false);
        loseScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        guiManager.showCursor = true;
    }

    public void ClickMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartLevel()
    {
        Cursor.lockState = CursorLockMode.Locked;
        guiManager.showCursor = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
}
