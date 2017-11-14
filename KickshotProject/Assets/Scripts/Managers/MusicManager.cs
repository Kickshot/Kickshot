using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour {
    public static MusicManager manager;
    private GameObject musicPlayer = null;
    private AudioSource audioSource;
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
        music [3] = "drum&bass";
        SceneManager.sceneLoaded += SceneLoaded;
    }
    void SceneLoaded(Scene scene, LoadSceneMode mode) {
        if (!music.ContainsKey (scene.buildIndex)) {
            return;
        }
        if (musicPlayer == null) {
            musicPlayer = new GameObject ();
            audioSource = musicPlayer.AddComponent<AudioSource> ();
        }
        audioSource.clip = ResourceManager.GetResource<AudioClip> (music [scene.buildIndex]);
        audioSource.loop = true;
        audioSource.Play ();
    }
}
