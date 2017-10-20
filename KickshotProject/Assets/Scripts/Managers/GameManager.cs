using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    SourcePlayer _player;
    LevelTimer _timer;

    bool _postLevelState = false;

    public SourcePlayer Player
    {
        get
        {
            if (_player == null)
                _player = GameObject.FindGameObjectWithTag("Player").GetComponent<SourcePlayer>();
            return _player;
        }
    }
    public LevelTimer GameTimer
    {
        get
        {
            if (_timer == null)
                _timer = GetComponent<LevelTimer>();
            return _timer;
        }
    }


    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += SceneLoaded;
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SaveManager.Save();
        GameTimer.Reset();
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

    public void Died()
    {
        SaveManager.Load();
        GameTimer.Reset();
    }

    public void LevelFinished()
    {
        _postLevelState = true;
    }

    public void LoadNext()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
