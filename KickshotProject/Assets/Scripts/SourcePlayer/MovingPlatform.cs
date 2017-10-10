using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
	private Movable body;
	// Use this for initialization
	void Start () {
		body = GetComponent<Movable> ();
	}
	
	// Update is called once per frame
	void Update () {
		body.velocity = new Vector3 ( Mathf.Sin (Time.time)*5f, 0f, 0f);
	}
}
