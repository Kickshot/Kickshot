using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryBox : MonoBehaviour {
	private AudioSource victorySting;
	private bool victory = false;
	void Start() {
		victorySting = GetComponent<AudioSource> ();
	}
	void OnGUI() {
		if (!victory) {
			return;
		}
		GUIStyle style = GUIStyle.none;
		style.normal.textColor = Color.black;
		style.fontSize = 24;
		GUI.Label (new Rect (Screen.width/2f+1f-125f, Screen.height/2f-50f, 250, 100), "You win!", style);
		style.normal.textColor = Color.red;
		GUI.Label (new Rect (Screen.width/2f-125f, Screen.height/2f-50f, 250, 100), "You win!", style);
	}
	void Update() {
		if (victory && Input.GetButtonDown ("Fire1")) {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}
	}
	void OnTriggerEnter( Collider other ) {
		if (other.gameObject.GetComponent<SourcePlayer> () != null) {
			victory = true;
			victorySting.Play ();
		}
	}
}
