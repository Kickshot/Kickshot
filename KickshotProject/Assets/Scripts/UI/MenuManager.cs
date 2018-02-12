using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public Canvas mainMenu;
    public List<Canvas> subMenus;

    private bool transitionActive = false;
    private Canvas curCanvas;
    private Canvas oldCanvas;

    public void Start()
    {
        if (mainMenu == null)
        {
            Debug.LogError("Need to specify main menu!");
        }
        curCanvas = mainMenu;
    }

    public void Transition(Canvas next)
    {
        //if (transitionActive) return;
        //transitionActive = true;
        oldCanvas = curCanvas;
        curCanvas = next;
        oldCanvas.gameObject.SetActive(false);
        curCanvas.gameObject.SetActive(true);
        //oldCanvas.GetComponent<CanvasFade>().ToggleFade();
        //transitionAnimator.SetTrigger("Close");
    }

    public void FinishedFade()
    {
        curCanvas.GetComponent<CanvasFade>().ToggleFade();
        transitionActive = false;
    }

    public void Opened()
    {
        transitionActive = false;
    }

    public void Closed()
    {
        
    }

    public void Quit()
    {
        Debug.Log("Quit");
        //TODO open up confirmation modal
    }
}
