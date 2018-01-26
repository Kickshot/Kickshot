using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallAway : MonoBehaviour {

    public Rigidbody fallRigid;

    private void Start()
    {
        fallRigid.isKinematic = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        fallRigid.isKinematic = false;
    }
}
