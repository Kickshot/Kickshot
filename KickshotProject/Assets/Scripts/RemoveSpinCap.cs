using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(Rigidbody) )]
public class RemoveSpinCap : MonoBehaviour {
	void Start () {
        GetComponent<Rigidbody> ().maxAngularVelocity = 100f;
	}
}
