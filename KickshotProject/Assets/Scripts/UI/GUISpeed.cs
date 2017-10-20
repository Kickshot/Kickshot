using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUISpeed : MonoBehaviour {
    public Text GUISpeedText;

    void Update()
    {
        Vector3 vel = GameManager.instance.Player.velocity;
        vel.y = 0;
        float speed = vel.magnitude * 10;

        GUISpeedText.text = speed.ToString("N0");
    }
}
