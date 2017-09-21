using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoRigidBodyPC : MonoBehaviour {

    public GameObject _camera;
    
    void Update () {
        GetComponent<CharacterController>().SimpleMove(_camera.transform.forward * 10);
	}
}
