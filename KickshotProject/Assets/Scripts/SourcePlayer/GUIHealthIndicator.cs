using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIHealthIndicator : MonoBehaviour {
	private SourcePlayer body;
	void Start () {
		body = GetComponent<SourcePlayer> ();
	}
	void OnGUI () {
		float health = body.health;
		GUIStyle style = GUIStyle.none;
		style.normal.textColor = Color.black;
		style.fontSize = 24;
		GUI.Label (new Rect (51f, Screen.height-50f, 100, 50), "♥ " + Mathf.Round (health).ToString (), style);
		style.normal.textColor = Color.red;
		GUI.Label (new Rect (50f, Screen.height-50f, 100, 50), "♥ " + Mathf.Round (health).ToString (), style);
	}
}
