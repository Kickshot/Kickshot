using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    public GameObject PlayerPrefab;
    GameObject Player;

    public GameObject StartPoint;
    public GameObject FinishPoint;
    

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)
            Player = Instantiate(PlayerPrefab);
        
        StartPoint = GameObject.FindGameObjectWithTag("Start");
        FinishPoint = GameObject.FindGameObjectWithTag("Finish");

        Respawn();
    }

    public void Respawn()
    {
        Player.transform.position = StartPoint.transform.position;
    }

    public void LevelFinished()
    {
        print("Beat the Level");
    }
}
