using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUISpeedometer : MonoBehaviour {
	private SourcePlayer body;
	void Start () {
		body = GetComponent<SourcePlayer> ();
	}
	void OnGUI () {
		Vector3 speed = body.velocity;
		speed.y = 0;
		GUIStyle style = GUIStyle.none;
		style.normal.textColor = Color.black;
		style.fontSize = 24;
		GUI.Label (new Rect (Screen.width/2f-100f+1f, Screen.height/2f, 200, 100), ">> " + Mathf.Round (speed.magnitude*10).ToString (), style);
		style.normal.textColor = Color.red;
		GUI.Label (new Rect (Screen.width/2f-100f, Screen.height/2f, 200, 100), ">> " + Mathf.Round (speed.magnitude*10).ToString (), style);
	}
}
