using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelCompleteScreenManager : MonoBehaviour
{

    // used to disable camera/movement
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public GameObject gun;
    [HideInInspector]
    public HUD hud;

    public GameObject levelCompleteScreen;
    public GameObject crosshair;
    public Text HUDTime;
    public Text completeTime;
    public Text BestText;
    public InGameGUIManager guiManager;


    public void DisplayLevelCompleteMenu()
    {
        hud = GameObject.Find("[HUD]").GetComponent<HUD>();
        SaveHighScore();
        DisplayHighScore();
        guiManager.activeMenu = true;
        Time.timeScale = 0;
        player = GameObject.Find("SourcePlayer");
        gun = GameObject.Find("DoubleGun");
        player.GetComponent<MouseLook>().enabled = false;
        player.GetComponent<CharacterController>().enabled = false;
        player.GetComponent<SourcePlayer>().enabled = false;
        gun.SetActive(false);
        crosshair.SetActive(false);
        levelCompleteScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        completeTime.text = HUDTime.text;
        HUDTime.enabled = false;
    }

    public void DisplayHighScore()
    {
        if(!PlayerPrefs.HasKey(SceneManager.GetActiveScene().name))
        {
            BestText.text = "Best : " + HUDTime.text;
        }
        else
        {
            BestText.text = "Best : " + ToTimerFormat(PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name));
        }
        StartCoroutine(OscillateHighScoreOpacity());
    }

    private IEnumerator OscillateHighScoreOpacity()
    {
        Color c = BestText.color;
        while (true)
        {
            BestText.color = new Color(c.r, c.g, c.b, ((Mathf.Sin(Time.unscaledTime * 2) + 1) / 2) + 0.4f );
            c = BestText.color;
            yield return new WaitForEndOfFrame();
        }
    }

    public void SaveHighScore()
    {
        if (hud == null)
        {
            print("Highscore not saved.");
            return;
        }

        if(PlayerPrefs.HasKey(SceneManager.GetActiveScene().name))
        {
            float highScore = PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name);
            if(hud.timer.Time < highScore)
            {
                PlayerPrefs.SetFloat(SceneManager.GetActiveScene().name, hud.timer.Time);
            }
        }
        else
        {
            PlayerPrefs.SetFloat(SceneManager.GetActiveScene().name, hud.timer.Time);
        }
    }

    public string ToTimerFormat(float t)
    {
        TimeSpan ts = TimeSpan.FromSeconds(t);
        //print("Minutes = " + ts.Minutes + " Seconds = " + ts.Seconds + " Milliseconds = " + ts.Milliseconds);
        return ts.Minutes.ToString().PadLeft(2, '0') + ":" + (ts.Seconds % 60).ToString().PadLeft(2, '0') + ":" + ts.Milliseconds.ToString().PadLeft(3, '0');
    }
    
    public void ClickMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void ClickNextLevel()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        //TODO: This will break if the build settings are changed at all. Probably better to adopt a "level1, level2" naming standard.
        int nextSceneBuildIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneBuildIndex == 12 || nextSceneBuildIndex == 15 || nextSceneBuildIndex == 19)
        {
            SceneManager.LoadScene("MainMenu");
        } else {
            SceneManager.LoadScene(nextSceneBuildIndex);
        }
    }
}
