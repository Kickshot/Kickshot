using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAxisGizmo : MonoBehaviour {
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.parent.position);
    }
}
