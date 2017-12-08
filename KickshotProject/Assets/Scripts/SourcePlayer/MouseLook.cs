using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {
    public Transform  view;
    private Vector3 viewOffset;
    public float xMouseSensitivity = 35.0f;
    public float yMouseSensitivity = 35.0f;
    [HideInInspector]
    public float rotX;
    [HideInInspector]
    public float rotY;
    private Vector3 shakePos;
    private float shakeTimer;
    private float shakeTimerMax;
    private Vector3 shakeIntensity;

    [HideInInspector]
    public bool useDeltas = true;
    [HideInInspector]
    public float wishYAxis;
    [HideInInspector]
    public float wishXAxis;
    [HideInInspector]
    public Quaternion wishDir;

    public void ShakeImpact (Vector3 intensity) {
        shakeIntensity += intensity/3.5f;
        if (shakeIntensity.magnitude > 0.5f) {
            shakeIntensity = Vector3.Normalize (shakeIntensity)*0.5f;
        }
        shakeTimer = Mathf.Min(shakeIntensity.magnitude,1f);
        shakeTimerMax = shakeTimer;
    }
    public void SetRotation( Quaternion r ) {
        rotX = r.eulerAngles.x;
        rotY = r.eulerAngles.y;
        while (rotX > 90) {
            rotX -= 360;
        }
        while (rotX < 0) {
            rotX += 360;
        }
        view.rotation = Quaternion.Euler (rotX, rotY, 0);
    }
    void Start()
    {
        view = view ?? Camera.main.transform;
        viewOffset = view.localPosition;
        SetRotation(view.rotation);
    }
    void Update ()
    {
        if (useDeltas) {
            if (Cursor.lockState == CursorLockMode.None) {
                if (Input.GetMouseButtonDown (0)) {
                    Cursor.lockState = CursorLockMode.Locked;
                }
            } else {
                if (Input.GetButtonDown ("Cancel")) {
                    //Application.Quit ();
                    //Cursor.lockState = CursorLockMode.None;
                }
            }

            if (shakeTimer > 0) {
                shakePos = shakeIntensity * EasingFunction.EaseInSine (1, 0, (shakeTimerMax - shakeTimer) / shakeTimerMax);
                shakeTimer -= Time.deltaTime;
                //shakeIntensity -= Time.deltaTime;
            } else {
                shakeTimer = 0f;
                shakeTimerMax = 0f;
                shakeIntensity = Vector3.zero;
                shakePos = Vector3.zero;
            }

            view.localPosition = viewOffset + shakePos;
            rotX -= wishYAxis * xMouseSensitivity * 0.03f;
            rotY += wishXAxis * yMouseSensitivity * 0.03f;
            if (rotX < -90f) {
                rotX = -90f;
            } else if (rotX > 90f) {
                rotX = 90f;
            }
            this.transform.rotation = Quaternion.Euler (0, rotY, 0); // Rotates the collider
            view.rotation = Quaternion.Euler (rotX, rotY, 0); // Rotates the camera
        } else {
            view.rotation = wishDir;
            transform.rotation = Quaternion.Euler (0, wishDir.eulerAngles.y, 0);
        }
    }
}
