using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSound : MonoBehaviour {
    public List<AudioClip> audios;
    private AudioSource source;
    // Use this for initialization
    void Start () {
        source = GetComponent<AudioSource> ();
        int index = Random.Range (0, audios.Count);
        source.clip = audios [index];
        source.Play ();
    }
}
