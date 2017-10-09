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
		GUI.Label (new Rect (0, 0, 400, 100), "Speed: " + Mathf.Round (speed.magnitude).ToString ());
	}
}
