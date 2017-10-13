using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour {
	private Movable body;
	public Vector3 direction = Vector3.right;
	public int seed = 0;
	public float speed = 5f;
	public float interval = 0.5f; // revolutions per second.
	// Use this for initialization
	void Start () {
		body = GetComponent<Movable> ();
	}

	// Update is called once per frame
	void Update () {
		body.velocity = direction * Mathf.Round(Mathf.Sin ((seed * Mathf.PI) + 2f * Mathf.PI * Time.timeSinceLevelLoad * interval)) * speed;
	}
}
