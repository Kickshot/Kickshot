using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    SourcePlayer _player;
    LevelTimer _timer;
    float _postLevelTimer;
    public Vector3 playerVelocity;

    bool _postLevelState = false;

    public SourcePlayer Player
    {
        get
        {
            if (_player == null) {
                GameObject p = GameObject.FindGameObjectWithTag ("Player");
                if (p != null) {
                    _player = p.GetComponent<SourcePlayer> ();
                    return _player;
                }
            }
            return null;
        }
    }
    /*public LevelTimer GameTimer
    {
        get
        {
            if (_timer == null)
                _timer = GetComponent<LevelTimer>();
            return _timer;
        }
    }*/


    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        GameObject m = new GameObject ();
        m.AddComponent<MusicManager> ();
        
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += SceneLoaded;
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SaveManager.Save();
        //GameTimer.Reset();
    }

    void Update()
    {
        if(_postLevelState)
        {
            //if (Player != null) {
                //playerVelocity = Player.velocity;
            //}
            if(Input.GetButtonDown("Fire1") || _postLevelTimer <= 0 )
            {
                _postLevelState = false;
                LoadNext();
            }
            _postLevelTimer -= Time.deltaTime;
        }
    }

    public void Died()
    {
        SaveManager.Load();
        //GameTimer.Reset();
    }

    public void LevelFinished()
    {
        _postLevelState = true;
        _postLevelTimer = 4f;
    }

    public void LoadNext()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
