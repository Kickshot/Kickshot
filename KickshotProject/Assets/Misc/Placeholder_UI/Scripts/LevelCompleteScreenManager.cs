using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteScreenManager : MonoBehaviour {

    // used to disable camera/movement
    public GameObject player, levelCompleteScreen, gun;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            DisplayLevelCompleteMenu();
        }
    }
    public void DisplayLevelCompleteMenu()
    {
        player.GetComponent<MouseLook>().enabled = false;
        player.GetComponent<CharacterController>().enabled = false;
        player.GetComponent<SourcePlayer>().enabled = false;
        gun.SetActive(false);
        levelCompleteScreen.SetActive(true);
        Cursor.visible = true;
    }

    public void ClickMainMenu()
    {
        SceneManager.LoadScene("MainMenuIslands");
    }

    public void ClickNextLevel()
    {
        Debug.Log("Next Level To Be Implemented");
    }
}
