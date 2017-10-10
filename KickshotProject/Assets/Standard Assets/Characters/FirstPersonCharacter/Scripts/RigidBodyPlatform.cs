using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyPlatform : MonoBehaviour {
	private Movable body;
	private Rigidbody rbody;
	// Use this for initialization
	void Start () {
		body = GetComponent<Movable> ();
		rbody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		rbody.AddForce (new Vector3 (Mathf.Sin (Time.time), 0f, Mathf.Cos (Time.time)));
		body.velocity = rbody.velocity;
	}
}
