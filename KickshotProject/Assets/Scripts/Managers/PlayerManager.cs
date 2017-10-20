using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    [HideInInspector] public GameObject Player;
    [HideInInspector] public GameObject StartPoint;
    [HideInInspector] public GameObject FinishPoint;

    public GameObject PlayerPrefab;
    public GameObject StartPrefab;
    public GameObject FinishPrefab;


    private GameObject _finish_text;
    private Text _speed_text;

    void Start() {
        FindAssets();
        Died();
    }

    void FindAssets()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)
            Player = Instantiate(PlayerPrefab);

        StartPoint = GameObject.FindGameObjectWithTag("Start");
        if (StartPoint == null)
            StartPoint = Instantiate(StartPrefab, Player.transform.position, Quaternion.identity);

        FinishPoint = GameObject.FindGameObjectWithTag("Finish");

        _finish_text = GameObject.Find("FinishText");
        _speed_text = GameObject.Find("SpeedText").GetComponent<Text>();
        SaveManager.Save ();
    }

    void Update()
    {
        updateSpeedText();
    }

    void updateSpeedText()
    {
        Vector3 vel = Player.GetComponent<SourcePlayer>().velocity;
        vel.y = 0;
        float speed = vel.magnitude * 10;

        _speed_text.text = speed.ToString("N0");
    }

    // Call this in a fail state to reset the player.
    // Eventually this needs to involve some sort of timed delay, 
    // so the you don't just restart immediately
    public void Died()
    {
        LevelTimer timer = GetComponent<LevelTimer>();

        FindAssets();
        Player.transform.position = getSpawnLocation();
        
        timer.Reset();
        timer.StartTimer();
    }

    Vector3 getSpawnLocation()
    {
        Vector3 startLoc;
        RaycastHit groundHit = new RaycastHit();
        if (Physics.Raycast(StartPoint.transform.position, Vector3.down, out groundHit))
        {
            float yOffset = Player.GetComponent<CapsuleCollider>().height * Player.transform.lossyScale.y / 2;
            startLoc = new Vector3(groundHit.point.x, groundHit.point.y, groundHit.point.z);
            Player.transform.position = startLoc;
        }

        else
        {
            startLoc = StartPoint.transform.position;
        }

        return startLoc;
    }

    public void LevelFinished()
    {
        Text t = _finish_text.GetComponent<Text>();
        t.color = new Color(0, 0, 0, 1);
    }

    public void SetNewPlayer(GameObject newPlayer)
    {
        Player = newPlayer;
    }
}
