using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    bool _postLevelState = false;
    LevelTimer _timer;

    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoad;
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        SendMessage("FindAssets");
    }

    void Update()
    {
        if(_postLevelState)
        {
            if(Input.GetButtonDown("Fire1"))
            {
                _postLevelState = false;
                LoadNext();
            }
        }
    }

    public void LoadNext()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LevelFinished()
    {
        _postLevelState = true;
    }
}
