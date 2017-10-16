using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {
	public Transform  view;
	private Vector3 viewOffset;
	private Quaternion offset;
	public float xMouseSensitivity = 30.0f;
	public float yMouseSensitivity = 30.0f;
	private float rotX;
	private float rotY;
	private Vector3 shakePos;
	private float shakeTimer;
	private float shakeTimerMax;
	private Vector3 shakeIntensity;

	public void ShakeImpact (Vector3 intensity) {
		shakeIntensity += intensity/3f;
		shakeTimer = intensity.magnitude/3f;
		shakeTimerMax = intensity.magnitude/3f;
	}
	public void SetRotation( Quaternion r ) {
		rotX = 0f;
		rotY = 0f;
		offset = r;
	}
	void Start() {
		viewOffset = view.localPosition;
		SetRotation (transform.rotation);
	}
	void Update () {
		if (Cursor.lockState == CursorLockMode.None) {
			if (Input.GetMouseButtonDown (0)) {
				Cursor.lockState = CursorLockMode.Locked;
			}
		}

		if (shakeTimer > 0) {
			shakePos = shakeIntensity * EasingFunction.EaseInSine (1, 0, (shakeTimerMax-shakeTimer)/shakeTimerMax);
			shakeTimer -= Time.deltaTime;
			//shakeIntensity -= Time.deltaTime;
		} else {
			shakeTimer = 0f;
			shakeTimerMax = 0f;
			shakeIntensity = Vector3.zero;
			shakePos = Vector3.zero;
		}

		view.localPosition = viewOffset+shakePos;
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
