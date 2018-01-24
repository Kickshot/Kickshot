using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    public Canvas mainMenu;
    public Canvas transition;
    public List<Canvas> subMenus;

    private bool transitionActive = false;
    private Animator transitionAnimator;
    private Canvas curCanvas;
    private Canvas oldCanvas;

    public void Start()
    {
        if (mainMenu == null) {
            Debug.LogError("Need to specify main menu!");
        }
        if (transition == null) {
            Debug.LogError("Need to specify transistion canvas!");
        }

        transitionAnimator = transition.GetComponent<Animator>();
        if (transitionAnimator == null) {
            Debug.LogError("Failed to find transition animator!");
        }

        AnimationEventListener listener = transitionAnimator.GetComponent<AnimationEventListener>();
        if (listener == null) {
            Debug.Log("Failed to find AnimationEventListener");
        }

        UnityEvent openedEvent = new UnityEvent();
        openedEvent.AddListener(Opened);
        listener.AddListener(openedEvent, "opened");

        UnityEvent closedEvent = new UnityEvent();
        closedEvent.AddListener(Closed);
        listener.AddListener(closedEvent, "closed");
        curCanvas = mainMenu;
    }

    public void Transition(Canvas next) {
        //if (transitionActive) return;
        //transitionActive = true;
        oldCanvas = curCanvas;
        curCanvas = next;
        oldCanvas.gameObject.SetActive(false);
        curCanvas.gameObject.SetActive(true);
        //oldCanvas.GetComponent<CanvasFade>().ToggleFade();
        //transitionAnimator.SetTrigger("Close");
    }

    public void FinishedFade(){
        curCanvas.GetComponent<CanvasFade>().ToggleFade();
        transitionActive = false;
    }

    public void Opened() {
        transitionActive = false;
    }

    public void Closed() {
        transitionAnimator.SetTrigger("Open");
    }

    public void Quit() {
        Debug.Log("Quit");
        //TODO open up confirmation modal
    }
}
