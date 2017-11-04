using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodyMess : MonoBehaviour {
    public GameObject bloodSpatterSpawn;
    private Vector3 lastSpawn;
    void OnCollisionEnter( Collision col ) {
        if ((lastSpawn - transform.position).magnitude > 0.5f) {
            Instantiate (bloodSpatterSpawn, col.contacts [0].point, Quaternion.LookRotation (-col.contacts [0].normal));
            lastSpawn = transform.position;
        }
    }
}
