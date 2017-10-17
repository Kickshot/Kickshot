using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnDestroy : MonoBehaviour {
    private bool quitting = false;
    public List<AudioClip> audios;
    void OnDestroy() {
        if (quitting) {
            return;
        }
        int index = Random.Range (0, audios.Count);
        AudioSource.PlayClipAtPoint (audios [index], transform.position);
    }
    void OnApplicationQuit() {
        quitting = true;
    }
}
