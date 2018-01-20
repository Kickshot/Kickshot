using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(Rigidbody) )]
public class ConstantVelocity : MonoBehaviour {
    public Vector3 AngularVelocity;
    private Rigidbody body;
    void Start() {
        body = GetComponent<Rigidbody> ();
    }
	void Update () {
        body.maxAngularVelocity = Mathf.Max (body.maxAngularVelocity, AngularVelocity.magnitude);
        body.angularVelocity = AngularVelocity;
	}
}
