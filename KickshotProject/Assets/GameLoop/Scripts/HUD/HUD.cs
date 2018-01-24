using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        timerText.text = timer.Time.ToString();
        speedText.text = player.velocity.magnitude.ToString();
    }
}
