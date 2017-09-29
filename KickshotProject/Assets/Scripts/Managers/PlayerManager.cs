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
        Player.GetComponent<PlayerController>().ResetPlayerVars();

        Vector3 startLoc;
        RaycastHit groundHit = new RaycastHit();
        if(Physics.Raycast(StartPoint.transform.position, Vector3.down, out groundHit))
        {
            float yOffset = Player.GetComponent<CapsuleCollider>().height * Player.transform.lossyScale.y / 2;
            startLoc = new Vector3(groundHit.point.x, groundHit.point.y, groundHit.point.z);
            Player.transform.position = startLoc;
        }

        else
        {
            startLoc = StartPoint.transform.position;
        }
        Player.transform.position = startLoc;
        
        timer.Reset();
        timer.StartCountdown();
    }

    public void LevelFinished()
    {
        print("Beat the Level");
    }
}
