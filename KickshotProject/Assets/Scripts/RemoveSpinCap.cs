using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(Rigidbody) )]
public class RemoveSpinCap : MonoBehaviour {
	public float MaxAngularSpeed = 7f;
	void Start () {
		GetComponent<Rigidbody> ().maxAngularVelocity = MaxAngularSpeed;
	}
}
