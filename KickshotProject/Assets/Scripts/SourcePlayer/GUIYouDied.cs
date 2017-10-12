using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIYouDied : MonoBehaviour {
	public Transform Camera;
	public GameObject Player;
	void OnGUI () {
		GUIStyle style = GUIStyle.none;
		style.normal.textColor = Color.black;
		style.fontSize = 24;
		GUI.Label (new Rect (Screen.width/2f+1f-125f, Screen.height/2f-50f, 250, 100), "You died!", style);
		style.normal.textColor = Color.red;
		GUI.Label (new Rect (Screen.width/2f-125f, Screen.height/2f-50f, 250, 100), "You died!", style);
	}
	void Update() {
		Camera.position = transform.position + new Vector3 (Mathf.Sin (Time.time), 1f, Mathf.Cos (Time.time))*10f;
		Camera.LookAt (transform.position);
		if (Input.GetButtonDown ("Fire1")) {
			Application.LoadLevel(Application.loadedLevel);
		}
	}
}
