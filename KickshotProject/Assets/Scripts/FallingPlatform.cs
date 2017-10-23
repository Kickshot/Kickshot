using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour {
    private Rigidbody body;
    public float waitTime = 1f;
    public float respawnTime = 10f;
    private float timer;
    private bool activated;
    private Vector3 pos;
    private Quaternion rot;
	// Use this for initialization
	void Start () {
        activated = false;
        body = GetComponent<Rigidbody> ();
        body.constraints = RigidbodyConstraints.FreezeAll;
        pos = transform.position;
        rot = transform.rotation;
	}
    void Update() {
        if (!activated) {
            return;
        }
        timer += Time.deltaTime;
        if (timer < waitTime) {
            return;
        }
        if (timer < respawnTime) {
            body.constraints = RigidbodyConstraints.None;
        } else {
            transform.position = pos;
            transform.rotation = rot;
            body.constraints = RigidbodyConstraints.FreezeAll;
            timer = 0f;
            activated = false;
        }
    }
    void OnCollisionEnter( Collision other ) {
        if (other.contacts[0].otherCollider.tag == "Player") {
            activated = true;
        }
    }
    void OnTriggerEnter( Collider other ) {
        if (other.tag == "Player") {
            activated = true;
        }
    }
}
