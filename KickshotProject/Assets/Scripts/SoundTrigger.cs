using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundTrigger : MonoBehaviour {
    public bool playOnlyOnce = true;
    public bool played = false;
    void OnTriggerEnter( Collider other ) {
        if (other.tag == "Player" && ((playOnlyOnce && !played) || !playOnlyOnce)) {
            GetComponent<AudioSource> ().Play ();
            played = true;
        }
    }
}
