using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [HideInInspector]
    public GameObject Player;
    public GameObject PlayerPrefab;

    public GameObject StartPoint;
    public GameObject FinishPoint;
    

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)
            Player = Instantiate(PlayerPrefab);
        
        StartPoint = GameObject.FindGameObjectWithTag("Start");
        FinishPoint = GameObject.FindGameObjectWithTag("Finish");

        Died();
    }

    // Call this in a fail state to reset the player.
    // Eventually this needs to involve some sort of timed delay, 
    // so the you don't just restart immediately
    public void Died()
    {
        LevelTimer timer = GetComponent<LevelTimer>();

        timer.Reset();
        // Player.GetComponent<PlayerController>().Reset()      // put something like this here when it is written
        Player.transform.position = StartPoint.transform.position;

        timer.Reset();
        timer.StartCountdown();
    }

    public void LevelFinished()
    {
        print("Beat the Level");
    }
}
