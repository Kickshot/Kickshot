using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves a GameObject between two points over a period of CycleLength seconds
[RequireComponent( typeof(Movable) )]
public class MoveBetween : MonoBehaviour {
    
    public Transform Target1;
    public Transform Target2;
    public float CycleLength = 3;
	private Vector3 lastPosition;
	void Update () {
		lastPosition = transform.position;
		float progress = (Mathf.Cos (Time.timeSinceLevelLoad * 2 * Mathf.PI / CycleLength) + 1f) / 2f;
		transform.position = Target1.position * progress + Target2.position * (1f - progress);
		GetComponent<Movable> ().velocity = (transform.position - lastPosition)/Time.deltaTime;
    }
}
