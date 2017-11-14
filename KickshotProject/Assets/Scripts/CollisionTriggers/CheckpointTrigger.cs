using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour {

	void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            SaveManager.Save ();
            //GameManager.instance.GetComponent<PlayerManager>().StartPoint.transform.position = transform.position;
    }
}
