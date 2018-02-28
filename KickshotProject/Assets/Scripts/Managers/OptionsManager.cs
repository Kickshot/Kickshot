using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsManager : MonoBehaviour
{

    private float m_sensitivity;
    private float m_fov;
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        m_sensitivity = PlayerPrefs.GetFloat("Sensitivity");
        m_fov = PlayerPrefs.GetFloat("Fov");

        if (m_sensitivity == 0)
            m_sensitivity = 50;

        if (m_fov == 0)
            m_fov = 90;
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

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
            cam.fieldOfView = m_fov;
        }
    }
}
