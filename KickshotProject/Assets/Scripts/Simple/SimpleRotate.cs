using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotate : MonoBehaviour {

    public float rotateSpeed = 5f;
    public bool x;
    public bool y;
    public bool z;

    private void Update()
    {
        Vector3 rotDelta = Vector3.zero;
        if (x)
            rotDelta.x += rotateSpeed;
        if (y)
            rotDelta.y += rotateSpeed;
        if (z)
            rotDelta.z += rotateSpeed;
        rotDelta *= Time.deltaTime;
        transform.Rotate(rotDelta);
    }

}
