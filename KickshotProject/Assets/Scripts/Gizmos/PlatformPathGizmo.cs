using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPathGizmo : MonoBehaviour {
    public Transform OtherTarget;
    private void OnDrawGizmos()
    {
        if (OtherTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, OtherTarget.transform.position);
        }
    }

}
