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
		GUI.Label (new Rect (200f+1f, Screen.height-50f, 100, 50), ">> " + Mathf.Round (speed.magnitude*10).ToString (), style);
		style.normal.textColor = Color.red;
		GUI.Label (new Rect (200f, Screen.height-50f, 100, 50), ">> " + Mathf.Round (speed.magnitude*10).ToString (), style);
	}
}
