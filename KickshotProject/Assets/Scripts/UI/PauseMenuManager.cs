using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenu, crosshair, player, gun;
    private bool m_paused;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            Pause();
        }
    }

    private void Pause()
    {
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        // Hide crosshair
        crosshair.SetActive(false);
		Cursor.lockState = CursorLockMode.None;
		if (player == null) {
			return;
		}
		gun.SetActive(false);
        player.GetComponent<MouseLook>().enabled = false;
        player.GetComponent<CharacterController>().enabled = false;
        player.GetComponent<SourcePlayer>().enabled = false;
    }

    public void ClickResume()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        // Show crosshair
        crosshair.SetActive(true);
		Cursor.lockState = CursorLockMode.Locked;
		if (player == null) {
			return;
		}
        player.GetComponent<MouseLook>().enabled = true;
        player.GetComponent<CharacterController>().enabled = true;
        player.GetComponent<SourcePlayer>().enabled = true;
        gun.SetActive(true);
    }

    public void ClickQuit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void ClickOptions()
    {

    }
}
