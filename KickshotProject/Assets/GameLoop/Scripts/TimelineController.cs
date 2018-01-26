using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineController : MonoBehaviour {

    public List<GameObject> disableds;
    public List<GameObject> enableds;

    public PlayableDirector dir;

    private void Start()
    {

        foreach (GameObject o in enableds)
        {
            o.SetActive(false);
        }
    }

    private void Update()
    {
        if (dir.state != PlayState.Playing) {
            StartGame();
        }
    }

    private void StartGame() {
        Debug.Log("Timeline over");
        foreach (GameObject o in disableds) {
            o.SetActive(false);
        }
        foreach (GameObject o in enableds) {
            o.SetActive(true);
        }
    }
}
