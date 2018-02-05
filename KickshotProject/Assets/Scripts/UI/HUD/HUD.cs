using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HUD : MonoBehaviour {

    [Header("HUD References")]
    public SourcePlayer player;
    public Timer timer;

    [Header("Component References")]
    public Canvas hudCanvas;
    public Text speedText;
    public Text timerText;
    public Image gHookImage;
    public Image rocketImage;

    private void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<SourcePlayer>();
            if (player == null) {
                Debug.LogError("Failed to assign player!");
            }
        }
        if (hudCanvas == null)
            Debug.LogError("Failed to find HUD canvas!");
        if (speedText == null)
            Debug.LogError("Failed to find speedometer text.");
        if (timerText == null)
            Debug.LogError("Failed to find timer text.");
        if (gHookImage == null)
            Debug.LogError("Failed to find grappling hook image,");
        if (rocketImage == null)
            Debug.LogError("Failed to find rocket launcher image.");
    }

    private void LateUpdate()
    {
        if (player == null)
            player = FindObjectOfType<SourcePlayer>();

        if (player == null)
            return;

        TimeSpan ts = TimeSpan.FromSeconds(timer.Time);
        //print("Minutes = " + ts.Minutes + " Seconds = " + ts.Seconds + " Milliseconds = " + ts.Milliseconds);
        timerText.text = ts.Minutes.ToString().PadLeft(2, '0') + ":" + (ts.Seconds % 60).ToString().PadLeft(2,'0') + ":" + ts.Milliseconds.ToString().PadLeft(3, '0');
        speedText.text = ((int)player.velocity.magnitude).ToString().PadLeft(2,'0');
    }
}
