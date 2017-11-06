using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodyMess : MonoBehaviour {
    public GameObject bloodSpatterSpawn;
    private Vector3 lastSpawn;
    void OnCollisionEnter( Collision col ) {
        if ((lastSpawn - transform.position).magnitude > 2f) {
            Instantiate(bloodSpatterSpawn,col.contacts [0].point,Quaternion.LookRotation(new Vector3(col.contacts [0].normal.z, col.contacts [0].normal.x, col.contacts [0].normal.y),col.contacts [0].normal)*Quaternion.AngleAxis(Random.Range(0,360),new Vector3(0,1,0)));
            lastSpawn = transform.position;
        }
    }
    void OnCollisionStay( Collision col ) {
        if ((lastSpawn - transform.position).magnitude > 2f) {
            Instantiate(bloodSpatterSpawn,col.contacts [0].point,Quaternion.LookRotation(new Vector3(col.contacts [0].normal.z, col.contacts [0].normal.x, col.contacts [0].normal.y),col.contacts [0].normal)*Quaternion.AngleAxis(Random.Range(0,360),new Vector3(0,1,0)));
            lastSpawn = transform.position;
        }
    }
}
