using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {
    public Transform teleportExit;
    void OnTriggerEnter( Collider other ) {
        SourcePlayer p = other.gameObject.GetComponent<SourcePlayer> ();
        if (p != null) {
            MouseLook m = other.gameObject.GetComponent<MouseLook> ();
            Quaternion difference = Quaternion.Inverse(other.gameObject.transform.rotation) * teleportExit.rotation;
            m.SetRotation (teleportExit.rotation);
            p.velocity = difference * p.velocity;
            other.gameObject.transform.position = teleportExit.position;
            other.gameObject.transform.rotation = teleportExit.rotation;
        }
    }
}
