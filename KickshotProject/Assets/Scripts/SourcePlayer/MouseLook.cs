using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {
	public Transform  view;
	public Vector3 viewOffset = new Vector3(0f,0.6f,0f);
	private Quaternion offset;
	public float xMouseSensitivity = 30.0f;
	public float yMouseSensitivity = 30.0f;
	private float rotX;
	private float rotY;
	public void SetRotation( Quaternion r ) {
		rotX = 0f;
		rotY = 0f;
		offset = r;
	}
	void Start() {
		SetRotation (transform.rotation);
	}
	void Update () {
		if (Cursor.lockState == CursorLockMode.None) {
			if (Input.GetMouseButtonDown (0)) {
				Cursor.lockState = CursorLockMode.Locked;
			}
		}
		rotX -= Input.GetAxis("Mouse Y") * xMouseSensitivity * 0.02f;
		rotY += Input.GetAxis("Mouse X") * yMouseSensitivity * 0.02f;
		if (rotX < -90f) {
			rotX = -90f;
		} else if (rotX > 90f) {
			rotX = 90f;
		}
		this.transform.rotation = offset * Quaternion.Euler(0, rotY, 0); // Rotates the collider
		view.rotation = offset * Quaternion.Euler(rotX, rotY, 0); // Rotates the camera
	}
}
