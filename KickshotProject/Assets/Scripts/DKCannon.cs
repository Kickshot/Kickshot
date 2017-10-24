using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DKCannon : MonoBehaviour {
    public float strength = 200f;
    void OnTriggerEnter( Collider other ) {
        if (other.tag == "Player") {
            other.gameObject.GetComponent<SourcePlayer> ().velocity = transform.forward * strength;
        }
    }
}
