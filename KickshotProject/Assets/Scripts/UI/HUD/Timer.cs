using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{

    public float Time { 
        get {
            return time;
        }
    }
    private float time;

    private void Start()
    {
        time = 0f;
    }

    private void Update()
    {
        time += UnityEngine.Time.deltaTime;
    }

}