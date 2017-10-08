using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUISpeedometer : MonoBehaviour {
	private CPMPlayer body;
	void Start () {
		body = GetComponent<CPMPlayer> ();
	}
	void OnGUI () {
		Vector3 speed = body.velocity;
		speed.y = 0;
		GUI.Label (new Rect (0, 0, 400, 100), "Speed: " + Mathf.Round ((speed.magnitude * 100)).ToString ());
	}
}
