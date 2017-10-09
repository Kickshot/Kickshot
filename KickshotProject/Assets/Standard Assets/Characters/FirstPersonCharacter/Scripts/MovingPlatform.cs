using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
	private GroundEntity body;
	// Use this for initialization
	void Start () {
		body = GetComponent<GroundEntity> ();
	}
	
	// Update is called once per frame
	void Update () {
		body.velocity = new Vector3 ( Mathf.Sin (Time.time)*4f, 0f, 0f);
	}
}
