using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroy : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        Destroy(this.gameObject, GetComponent<ParticleSystem>().main.duration);
    }
    
}
