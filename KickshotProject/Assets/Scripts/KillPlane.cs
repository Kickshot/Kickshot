using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlane : MonoBehaviour {
    void Update () {
        foreach ( SourcePlayer obj in GameObject.FindObjectsOfType<SourcePlayer>() ) {
            if (Vector3.Dot (obj.transform.position - transform.position, transform.up) < 0) {
                obj.SendMessage ("Damage", 99999);
            }
        }
    }
}
