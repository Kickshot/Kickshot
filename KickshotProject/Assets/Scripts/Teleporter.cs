using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour {
	public Transform teleportExit;
	void OnTriggerEnter( Collider other ) {
		SourcePlayer p = other.gameObject.GetComponent<SourcePlayer> ();
		if (p != null) {
			other.gameObject.transform.position = teleportExit.position;
			other.gameObject.transform.rotation = teleportExit.rotation;
		}
	}
}
