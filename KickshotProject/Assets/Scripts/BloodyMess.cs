using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodyMess : MonoBehaviour {
    public GameObject bloodSpatterSpawn;
    private Vector3 lastSpawn;
    void OnCollisionEnter( Collision col ) {
        if (col.gameObject.layer == LayerMask.NameToLayer ("TransparentFX")) {
            return;
        }
        Vector3 perp = new Vector3 (-col.contacts [0].normal.z, col.contacts [0].normal.x, -col.contacts [0].normal.y);
        if ((lastSpawn - transform.position).magnitude > 2f) {
            Instantiate(bloodSpatterSpawn,col.contacts [0].point - col.contacts[0].normal, Quaternion.LookRotation(perp,col.contacts[0].normal));
            lastSpawn = transform.position;
        }
    }
    void OnCollisionStay( Collision col ) {
        if (col.gameObject.layer == LayerMask.NameToLayer ("TransparentFX")) {
            return;
        }
        Vector3 perp = new Vector3 (-col.contacts [0].normal.z, col.contacts [0].normal.x, -col.contacts [0].normal.y);
        if ((lastSpawn - transform.position).magnitude > 2f) {
            Instantiate(bloodSpatterSpawn,col.contacts [0].point - col.contacts[0].normal,Quaternion.LookRotation(perp,col.contacts[0].normal));
            lastSpawn = transform.position;
        }
    }
}
