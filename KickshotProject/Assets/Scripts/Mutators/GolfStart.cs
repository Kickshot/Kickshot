using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfStart : MonoBehaviour {

	GolfMutator mutator;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			mutator = other.GetComponent<GolfMutator> ();
			mutator.ChangeWalk (true);
		}
	}
	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "Player") {
			mutator = other.GetComponent<GolfMutator> ();
			mutator.ChangeWalk (false);
		}
	}
}
