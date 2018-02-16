using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour {
    public static GameManager instance = null;

    SourcePlayer _player;
    LevelTimer _timer;
    float _postLevelTimer;
    bool _saveOnNextFrame = false;
    bool _postLevelState = false;
	public bool flyby = false;

    public SourcePlayer Player
    {
        get {
            if (_player == null) {
                GameObject p = GameObject.FindGameObjectWithTag ("Player");
                if (p != null) {
                    _player = p.GetComponent<SourcePlayer> ();
                    return _player;
                }
            }
            return _player;
        }
    }

	[HideInInspector]
	public Vector3 playerVelocity;

    void Awake() {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        GameObject m = new GameObject ();        
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += SceneLoaded;
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode) {
        _saveOnNextFrame = true;
    }

	void FinishFlyby() {
		flyby = false;
	}

    void Update() {
		if (_saveOnNextFrame && !flyby) {
            SaveManager.Save();
            _saveOnNextFrame = false;
        }
		if (Player != null) {
			playerVelocity = Player.velocity;
		}
        if(_postLevelState)
        {
            if(Input.GetButtonDown("Fire1") || _postLevelTimer <= 0 )
            {
                _postLevelState = false;
                LoadNext();
            }
            _postLevelTimer -= Time.deltaTime;
        }
    }

    public void Died() {
        SaveManager.Load();
    }

    public void LevelFinished() {
        _postLevelState = true;
        _postLevelTimer = 2f;
    }

    public void LoadNext() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
