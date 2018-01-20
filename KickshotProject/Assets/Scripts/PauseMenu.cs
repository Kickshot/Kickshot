using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

	public bool m_PauseGame = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Pause"))
			m_PauseGame = !m_PauseGame;

		if (Input.GetKeyDown (KeyCode.Q) && m_PauseGame)
			Application.Quit();
		if (Input.GetKeyDown (KeyCode.R) && m_PauseGame) {
			UnPause ();
			SceneManager.LoadScene (0);
		}

		if (m_PauseGame)
			Pause ();
		else
			UnPause ();
	}

	public void Pause() {
		Time.timeScale = 0;
	}

	public void UnPause() {
		Time.timeScale = 1;
	}

	void OnGUI() {

		
		if (m_PauseGame) {
			GUIStyle style = new GUIStyle();
			style.fontSize = 24;
			GUI.Label (new Rect (10, 100, 400, 200), "Game Paused",style);
			GUI.Label(new Rect (10, 200, 400, 200), "Press R To Restart Game.",style);
			GUI.Label(new Rect (10, 300, 400, 200), "Press Q To Quit.",style);
		}
	}

}
