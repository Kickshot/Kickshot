using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {
	public List<GameObject> explosions;
	public float speed = 10f;
	public float radius = .5f;
	public float inheritPercentage = .5f;
	public Vector3 inheritedVel;
	void Update() {
		transform.position += (inheritedVel*inheritPercentage + transform.forward * speed) * Time.deltaTime;
		foreach (Collider col in Physics.OverlapSphere(transform.position, radius )) {
			if (col.gameObject == gameObject) {
				continue;
			}
			if (!col.gameObject.CompareTag ("Player")) {
				Destroy (gameObject);
				int rand = (int)Random.Range (0, explosions.Count);
				Instantiate (explosions [rand], transform.position, Quaternion.identity);
			}
		}
	}
}
