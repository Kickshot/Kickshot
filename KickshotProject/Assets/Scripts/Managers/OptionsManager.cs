﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    public InGameGUIManager guiManager;
    private float m_sensitivity;
    private float m_fov;
    bool Active = false;

    void Awake()
    {
        m_sensitivity = PlayerPrefs.GetFloat("Sensitivity");
        m_fov = PlayerPrefs.GetFloat("Fov");

        if (m_sensitivity == 0)
            m_sensitivity = 50;

        if (m_fov == 0)
            m_fov = 90;
    }

    void OnLevelWasLoaded(int level)
    {
        setPlayerSensitivity();
        setPlayerFov();
    }

    public void setSensitivity(float value)
    {
        m_sensitivity = value;
        PlayerPrefs.SetFloat("Sensitivity", m_sensitivity);
        setPlayerSensitivity();
    }

    public float getSensitivity()
    {
        return m_sensitivity;
    }

    public void setFov(float value)
    {
        m_fov = value;
        PlayerPrefs.SetFloat("Fov", m_fov);
        setPlayerFov();
    }

    public float getFov()
    {
        return m_fov;
    }

    private void setPlayerSensitivity()
    {
        GameObject Player = GameObject.Find("SourcePlayer");
        if (Player != null)
        {
            MouseLook mouse = Player.GetComponent<MouseLook>();
            mouse.xMouseSensitivity = m_sensitivity;
            mouse.yMouseSensitivity = m_sensitivity;
        }
    }

    private void setPlayerFov()
    {
        GameObject Player = GameObject.Find("SourcePlayer");
        if (Player != null)
        {
            Camera cam = Player.GetComponentInChildren<Camera>();
			PlayerPostProcessOptions PostPros = Player.GetComponentInChildren<PlayerPostProcessOptions> ();
			PostPros.baseFOV = m_fov;
            cam.fieldOfView = m_fov;
        }
    }

    public void FadeOut(GameObject menu)
    {
		if (guiManager == null) {
			Active = false;
			Animator anim = menu.GetComponent<Animator>();
			anim.SetTrigger("FadeOut");
			return;
		}

        if (guiManager.optionsOpen)
        {
            guiManager.optionsOpen = false;
            Active = false;
            Animator anim = menu.GetComponent<Animator>();
            anim.SetTrigger("FadeOut");
        } else
        {
            FadeIn(menu);
        }
    }

    public void FadeIn(GameObject menu)
    {
		if (guiManager == null) {
			Active = true;
			Animator anim = menu.GetComponent<Animator>();
			anim.SetTrigger("FadeIn");
			return;
		}
			
        if (!guiManager.optionsOpen)
        {
            guiManager.optionsOpen = true;
            Active = true;
            Animator anim = menu.GetComponent<Animator>();
            anim.SetTrigger("FadeIn");
        } else
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
}
