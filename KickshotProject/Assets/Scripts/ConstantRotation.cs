using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ConstantRotation : MonoBehaviour {
    public float CycleTime = 3f;
    public bool Reverse = false;
    public Transform AxisTarget;

    Rigidbody body;
    Vector3 rotAxis;

    void Start()
    {
        rotAxis = (AxisTarget.transform.localPosition).normalized;
        body = GetComponent<Rigidbody>();
    }

    void Update () {
        float degPerSecond = 360 / CycleTime;
        degPerSecond *= Reverse ? -1 : 1; 
        Quaternion delta = Quaternion.AngleAxis(degPerSecond * Time.deltaTime, rotAxis);
        body.MoveRotation(transform.rotation * delta);
	}

    void Reset()
    {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        body.isKinematic = true;
        body.constraints = RigidbodyConstraints.FreezePosition;
        body.interpolation = RigidbodyInterpolation.Interpolate;

        AxisTarget = transform.Find("RotationAxis");
        if (AxisTarget == null)
        {
            Object t = Resources.Load("EditorGizmos/RotationAxis");
            AxisTarget = (Instantiate(t, transform.position + 3 * Vector3.up, Quaternion.identity, transform) as GameObject).transform;
            AxisTarget.gameObject.name = "RotationAxis";
        }
    }
}