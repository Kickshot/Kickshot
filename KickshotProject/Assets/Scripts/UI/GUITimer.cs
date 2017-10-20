using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUITimer : MonoBehaviour {
    public Text GUITimerText;

    void Update()
    {
        GUITimerText.text = GameManager.instance.GameTimer.CurrentTime.ToString("N2");
    }
}
