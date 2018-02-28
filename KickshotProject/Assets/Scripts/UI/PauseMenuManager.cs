using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [HideInInspector]
    public GameObject player, gun;
    public GameObject pauseMenu, crosshair;
    private bool m_paused;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pause") && !m_paused)
        {
            Pause();
        }
    }

    private void Pause()
    {
        m_paused = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);
        // Hide crosshair
        crosshair.SetActive(false);
		Cursor.lockState = CursorLockMode.None;
        player = GameObject.Find("SourcePlayer");
        gun = GameObject.Find("DoubleGun");
		gun.SetActive(false);
        player.GetComponent<MouseLook>().enabled = false;
        player.GetComponent<CharacterController>().enabled = false;
        player.GetComponent<SourcePlayer>().enabled = false;
    }

    public void ClickResume()
    {
        m_paused = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        // Show crosshair
        crosshair.SetActive(true);
		Cursor.lockState = CursorLockMode.Locked;
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

    public void ClickRestart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ClickOptions()
    {

    }

    public void FadeOut(GameObject menu)
    {
        CanvasGroup group = menu.GetComponent<CanvasGroup>();
        group.alpha = 0;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    public void FadeIn(GameObject menu)
    {
        CanvasGroup group = menu.GetComponent<CanvasGroup>();
        group.alpha = 1;
        group.interactable = true;
        group.blocksRaycasts = true;
    }
}
