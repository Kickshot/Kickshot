using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfEnd : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player") {
			SourcePlayer player = other.GetComponent<SourcePlayer> ();
			player.SendMessage ("Damage", 99999);
		}
	}
}
