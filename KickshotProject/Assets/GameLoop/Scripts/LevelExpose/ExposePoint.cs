using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExposePoint : MonoBehaviour {

    public Mesh camMesh;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireMesh(camMesh, transform.position, transform.rotation);
    }
}
