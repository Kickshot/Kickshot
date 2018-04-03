using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour {
    public static MusicManager manager;
    private Animator animator;
    private AudioSource audioSourceIn;
    private AudioSource audioSourceOut;
    private Dictionary<int, string> music;
    private bool InPlaying;
    private int oldBuildIndex;
    void Awake() {
        if (!manager) {
            manager = this;
            DontDestroyOnLoad (gameObject);
        } else {
            Destroy (gameObject);
        }
        animator = GetComponent<Animator>();
        music = new Dictionary<int, string> ();
        oldBuildIndex = 0;

        music [1] = "I04";
        music[2] = "I01";
        music[3] = "I02"; // Music to be played during level index x (whomp fortress). Music IDs are handled by the Resource Manager.
        music[4] = "I01";
        music[5] = "I03";
        music[6] = "I04";
        music[7] = "I05";
        music[8] = "I06";
        music[9] = "I07";
        music[10] = "I08";
        music[11] = "I09";
        music[12] = "G01";
        music[13] = "G02";
        music[14] = "G03";
        music[15] = "D01";
        music[16] = "D02";
        music[17] = "D03";
        music[18] = "D04";
        SceneManager.sceneLoaded += SceneLoaded;
    }
    void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!music.ContainsKey (scene.buildIndex))
        {
            return;
        }
        if (audioSourceIn == null || audioSourceOut == null)
        {
            AudioSource[] audioSources = manager.GetComponentsInChildren<AudioSource>();
            audioSourceIn = audioSources[0];
            audioSourceOut = audioSources[1];

            audioSourceIn.clip = ResourceManager.GetResource<AudioClip>(music[scene.buildIndex]);
            audioSourceIn.loop = true;
            audioSourceIn.Play();
            audioSourceOut.loop = true;
            audioSourceOut.Play();
        }
        else
        {
            if (scene.buildIndex != manager.oldBuildIndex)
            {
                Crossfade(scene.buildIndex);
            }
        }
        manager.oldBuildIndex = scene.buildIndex;
    }

    void Crossfade(int toSceneIndexNumber)
    {
        manager.animator.SetTrigger("Crossfade");
        if(!InPlaying)
        {
            //Fading to Out
            audioSourceOut.clip = ResourceManager.GetResource<AudioClip>(music[toSceneIndexNumber]);
            audioSourceOut.loop = true;
            audioSourceOut.Play();
        }
        else
        {
            //Fading to In
            audioSourceIn.clip = ResourceManager.GetResource<AudioClip>(music[toSceneIndexNumber]);
            audioSourceIn.loop = true;
            audioSourceIn.Play();
        }
        InPlaying = !InPlaying; 
    }
}
