using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(MouseLook) )]
public class InflatorPossession : MonoBehaviour {
    private MouseLook look;
    void Start() {
        look = GetComponent<MouseLook> ();
    }
	// Update is called once per frame
	void Update () {
        look.wishXAxis = Input.GetAxisRaw ("Mouse X");
        look.wishYAxis = Input.GetAxisRaw ("Mouse Y");
	}
}
