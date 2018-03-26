using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HighscoreManager : MonoBehaviour
{
    public InGameGUIManager guiManager;

    bool Active;

    public void Awake()
    {
        Active = false;
    }

    public void FadeOut(GameObject menu)
    {
        if (guiManager == null)
        {
            Active = false;
            Animator anim = menu.GetComponent<Animator>();
            anim.SetTrigger("FadeOut2");
            return;
        }

        if (guiManager.optionsOpen)
        {
            guiManager.optionsOpen = false;
            Active = false;
            Animator anim = menu.GetComponent<Animator>();
            anim.SetTrigger("FadeOut2");
        }
        else
        {
            FadeIn(menu);
        }
    }

    public void FadeIn(GameObject menu)
    {
        UpdateHighScoreListings(menu);
        if (guiManager == null)
        {
            Active = true;
            Animator anim = menu.GetComponent<Animator>();
            anim.SetTrigger("FadeIn2");
            return;
        }

        if (!guiManager.optionsOpen)
        {
            guiManager.optionsOpen = true;
            Active = true;
            Animator anim = menu.GetComponent<Animator>();
            anim.SetTrigger("FadeIn2");
        }
        else
        {
            FadeOut(menu);
        }
    }

    public void Activate()
    {
        CanvasGroup group = gameObject.GetComponent<CanvasGroup>();
        group.alpha = 1;
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    public void Deactivate()
    {
        CanvasGroup group = gameObject.GetComponent<CanvasGroup>();
        group.alpha = 0;
        group.interactable = false;
        group.blocksRaycasts = false;
        Active = false;
    }

    private void UpdateHighScoreListings(GameObject menu)
    {
        Text[] ct = GameObject.Find("Level Times").GetComponentsInChildren<Text>();
        //1 + DesertLevelsCount for Menu Scene.
        for (int i = 1, j = 0; i < 11; i++, j++)
        {
            if(PlayerPrefs.HasKey(SceneManager.GetSceneByBuildIndex(i).name))
            {
                ct[j].text = ToTimerFormat(PlayerPrefs.GetFloat(SceneManager.GetSceneByBuildIndex(i).name));
            }
            else
            {
                ct[j].text = "Unplayed";
            }
        }

        print("count " + SceneManager.sceneCountInBuildSettings);

        for (int i = 15, j = 10; i < 19; i++, j++)
        {
            if (PlayerPrefs.HasKey(SceneManager.GetSceneByBuildIndex(i).name))
            {
                ct[j].text = ToTimerFormat(PlayerPrefs.GetFloat(SceneManager.GetSceneByBuildIndex(i).name));
            }
            else
            {
                ct[j].text = "Unplayed";
            }
        }
    }

    public string ToTimerFormat(float t)
    {
        TimeSpan ts = TimeSpan.FromSeconds(t);
        //print("Minutes = " + ts.Minutes + " Seconds = " + ts.Seconds + " Milliseconds = " + ts.Milliseconds);
        return ts.Minutes.ToString().PadLeft(2, '0') + ":" + (ts.Seconds % 60).ToString().PadLeft(2, '0') + ":" + ts.Milliseconds.ToString().PadLeft(3, '0');
    }
}
