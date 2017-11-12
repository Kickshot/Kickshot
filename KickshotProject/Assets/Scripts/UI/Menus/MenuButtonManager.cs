using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonManager : MonoBehaviour {

    public void NewGame(){
        //Assumes scene 1 in the build path is the first level of the game
        SceneManager.LoadScene(1);
    }

    public void ExitGame(){
        Application.Quit();
    }

    public void LevelSelectButton(){
        // This should redirect to the load level menu
        SceneManager.LoadScene("LevelSelectMenu");
    }

    public void LevelEditorButton() { 
        // This should redirect to the level editor menu
        SceneManager.LoadScene("LevelEditorMenu");
    }

    public void LeaderboardsButton()
    {
        // This should redirect to the leaderboards menu
        //SceneManager.LoadScene();
    }
}
