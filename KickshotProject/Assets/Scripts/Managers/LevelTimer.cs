using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTimer : MonoBehaviour
{
    public float CurrentTime;          // Time left on clock in seconds
    bool _running;              // True when the Timer is actually counting down
    
    // Use this for initialization
    void Awake()
    {
        Reset();
    }

    void Update()
    {
        if (_running)
        {
            CurrentTime += Time.deltaTime;
        }
    }

    public void StartTimer()
    {
        _running = true;
    }
    public void StopTimer()
    {
        _running = false;
    }
    public void Reset()
    {
        CurrentTime = 0;
        _running = true;  // Change this eventually not to start until the player does something
    }
}
