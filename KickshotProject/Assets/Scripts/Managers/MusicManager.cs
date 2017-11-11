using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour {
    public static MusicManager manager;
    private Dictionary<int, string> music;
    void Awake() {
        if (!manager) {
            manager = this;
            DontDestroyOnLoad (gameObject);
        } else {
            Destroy (gameObject);
        }
        music = new Dictionary<int, string> ();
        //music [2] = "MarioMusic"; // Music to be played during level index x (whomp fortress). Music IDs are handled by the Resource Manager.
        //music [1] = "DKMusic";
        SceneManager.sceneLoaded += SceneLoaded;
    }
    void SceneLoaded(Scene scene, LoadSceneMode mode) {
        if (!music.ContainsKey (scene.buildIndex)) {
            return;
        }
        AudioSource m = Camera.main.gameObject.AddComponent<AudioSource> ();
        m.clip = ResourceManager.GetResource<AudioClip> (music [scene.buildIndex]);
        m.loop = true;
        m.Play ();
    }
}
