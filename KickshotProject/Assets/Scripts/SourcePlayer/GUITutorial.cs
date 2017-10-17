using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUITutorial : MonoBehaviour {
    private SourcePlayer body;
    public Texture2D wasd;
    public Texture2D mouse;
    public Texture2D wasd_d;
    void Start () {
        body = GetComponent<SourcePlayer> ();
    }
    void OnGUI () {
        Vector3 speed = body.velocity;
        speed.y = 0;
        string text;
        if (speed.magnitude < 9.8) {
            GUI.DrawTexture (new Rect (Screen.width / 2f, Screen.height / 2f, 512, 512), wasd);
            text = "Reach 300 speed! Hold W...";
        } else if (speed.magnitude > 9.8 && speed.magnitude < 25) {
            text = "Release W, and hold D towards the column...";
            GUI.DrawTexture (new Rect (Screen.width / 2f, Screen.height / 2f, 512, 512), wasd_d);
            GUI.DrawTexture (new Rect (Screen.width / 2f + ((Time.time*200f)%300f), Screen.height / 2f - 400, 512, 512), mouse);
        } else if (speed.magnitude < 29.9) {
            text = "Good job, keep going!";
        } else {
            text = "Woo you did it!";
        }
        GUIStyle style = GUIStyle.none;
        style.normal.textColor = Color.black;
        style.fontSize = 24;
        GUI.Label (new Rect (Screen.width/2f-250f+1f, Screen.height/2f-200, 500, 100), text, style);
        style.normal.textColor = Color.red;
        GUI.Label (new Rect (Screen.width/2f-250f, Screen.height/2f-200, 500, 100), text, style);
    }
}
