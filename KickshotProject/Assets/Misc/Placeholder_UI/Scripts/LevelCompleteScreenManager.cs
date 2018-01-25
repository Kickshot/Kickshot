using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompleteScreenManager : MonoBehaviour {

    // used to disable camera/movement
    public GameObject player, levelCompleteScreen, gun;
    public Text HUDTime, completeTime;

    void Start()
    {
        //player = GameObject.Find("SourcePlayer");
       // DisplayLevelCompleteMenu();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            DisplayLevelCompleteMenu();
        }
    }
    public void DisplayLevelCompleteMenu()
    {
        Time.timeScale = 0;
        player.GetComponent<MouseLook>().enabled = false;
        player.GetComponent<CharacterController>().enabled = false;
        player.GetComponent<SourcePlayer>().enabled = false;
        gun.SetActive(false);
        levelCompleteScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        completeTime.text = HUDTime.text;
        HUDTime.enabled = false;
    }

    public void ClickMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void ClickNextLevel()
    {
        Time.timeScale = 1;
        Debug.Log("Next Level To Be Implemented");
    }
}
