using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class PlaySplash : MonoBehaviour {

    public VideoPlayer vp;

	// Use this for initialization
	void Awake () {
        vp.Play();
        vp.loopPointReached += LoadMainMenu;
        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {
	}

    void LoadMainMenu( VideoPlayer vp)
    {
        Cursor.visible = true;
        //Load in Main Menu here.
        SceneManager.LoadScene("MainMenuIslands");
    }
}
