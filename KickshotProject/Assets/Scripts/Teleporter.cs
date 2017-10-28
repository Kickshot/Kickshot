using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {
    public Transform teleportExit;
    public bool maintainVelocity = false;
    void OnTriggerEnter( Collider other ) {
        SourcePlayer p = other.gameObject.GetComponent<SourcePlayer> ();
        if (p != null) {
            MouseLook m = other.gameObject.GetComponent<MouseLook> ();
            Quaternion difference = Quaternion.Inverse(other.gameObject.transform.rotation) * teleportExit.rotation;
            m.SetRotation (teleportExit.rotation);
            if (maintainVelocity) {
                p.velocity = difference * p.velocity;
            } else {
                p.velocity = Vector3.zero;
            }
            other.gameObject.transform.position = teleportExit.position;
            other.gameObject.transform.rotation = teleportExit.rotation;
        }
    }
}
