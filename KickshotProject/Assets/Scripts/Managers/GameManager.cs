using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    LevelTimer m_timer;

    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        tag = "GameController";
        m_timer = GetComponent<LevelTimer>();
    }

    void Countdown()
    {
        m_timer.StartCountdown();
    }
}
