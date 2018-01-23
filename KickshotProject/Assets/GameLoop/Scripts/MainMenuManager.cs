using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

    public Animator uiAnimator;

    public void Play() {
        Debug.Log("Play");
        uiAnimator.SetTrigger("Play");
    }

    public void Options() {
        Debug.Log("Options");
        uiAnimator.SetTrigger("Options");
    }

    public void MainMenu() {
        Debug.Log("Main");
        uiAnimator.SetTrigger("ToMain");
    }

    public void Start() {
        Debug.Log("Start");
    }

    public void Quit() {
        Application.Quit();
    }
}
