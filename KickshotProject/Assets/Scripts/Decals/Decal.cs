using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(MeshFilter) )]
[RequireComponent( typeof(MeshRenderer) )]
public class Decal : MonoBehaviour {
    public Material decal;
    public float offset = 0.01f;
    public LayerMask layerMask;
    private List<Vector3> newVerts = new List<Vector3> ();
    private List<Vector2> newUV = new List<Vector2>();
    //private List<Vector2> newNormals = new List<Vector2>();
    private List<int> newTri = new List<int> ();
    Dictionary<int, int> indexLookup = new Dictionary<int, int>();

    void Start() {
        GetComponent<MeshRenderer> ().material = decal;
        BuildDecal ();
        transform.hasChanged = false;
    }

    void OnDrawGizmosSelected() {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube( Vector3.zero, Vector3.one );
    }

    void Update() {
        if (transform.hasChanged) {
            BuildDecal ();
            transform.hasChanged = false;
        }
    }

    public void BuildDecal() {
        StartBuildMesh ();
        foreach (GameObject obj in GetAffectedObjects()) {
            BSPTree affectedMesh = obj.GetComponent<BSPTree>();
            BuildMeshForObject (obj, affectedMesh);
        }
        FinishMesh ();
    }

    private void StartBuildMesh() {
        newVerts.Clear ();
        newUV.Clear ();
        newTri.Clear ();
    }

    private void BuildMeshForObject( GameObject obj, BSPTree tree ) {
        indexLookup.Clear ();
        List<int> triangles = new List<int>();
        tree.FindClosestTriangles (transform.position, transform.lossyScale.magnitude/2f, triangles);
        Matrix4x4 mat = transform.worldToLocalMatrix * obj.transform.localToWorldMatrix;
        for (int i = 0; i < triangles.Count; i++) {
            int i1, i2, i3;
            tree.GetIndices (triangles [i], out i1, out i2, out i3);
            Vector3 v1, v2, v3;
            tree.GetVertices (triangles [i], out v1, out v2, out v3);
            v1 = mat.MultiplyPoint (v1);
            v2 = mat.MultiplyPoint (v2);
            v3 = mat.MultiplyPoint (v3);

            Vector3 side1 = v2 - v1;
            Vector3 side2 = v3 - v1;
            Vector3 normal = Vector3.Cross(side1, side2).normalized;

            if (normal.y <= 0.05f) {
                continue;
            }
            float off = Random.Range (0.1f, 1f)*offset;
            v1 += normal * off;
            v2 += normal * off;
            v3 += normal * off;

            int ni1, ni2, ni3;
            if (!indexLookup.TryGetValue (i1, out ni1)) {
                newVerts.Add (v1);
                newUV.Add (new Vector2 (v1.x + 0.5f, v1.z + 0.5f));
                ni1 = newVerts.Count - 1;
                indexLookup [i1] = ni1;
            }
            if (!indexLookup.TryGetValue (i2, out ni2)) {
                newVerts.Add (v2);
                newUV.Add (new Vector2 (v2.x + 0.5f, v2.z + 0.5f));
                ni2 = newVerts.Count - 1;
                indexLookup [i2] = ni2;
            }
            if (!indexLookup.TryGetValue (i3, out ni3)) {
                newVerts.Add (v3);
                newUV.Add (new Vector2 (v3.x + 0.5f, v3.z + 0.5f));
                ni3 = newVerts.Count - 1;
                indexLookup [i3] = ni3;
            }
            //newNormals.Add (normal);

            newTri.Add (ni1);
            newTri.Add (ni2);
            newTri.Add (ni3);

            //if (Helper.TriangleIntersectsAABB (v1, v2, v3, new Vector3 (0, 0, 0), new Vector3 (0.5f, 0.5f, 0.5f))) {
            //AddTriangle (v1, v2, v3, normal);
            //}
        }
    }
    private int ClipTriangle( List<Vector3> verts, List<Vector2> uvs, List<int> tris, int triangle, Plane plane ) {
        // Detect violating vertices.
        bool[] violating = new bool[3];
        int violationCount = 0;
        for (int i = 0; i < 3; i++) {
            violating[i] = plane.GetSide (verts [tris[triangle+i]]);
            if (violating[i]) {
                violationCount++;
            }
        }
        if (violationCount == 3) {
            tris.RemoveRange (triangle, 3);
            return 3;
        } else if (violationCount == 0) {
            return 0;
        } else if (violationCount == 2) {
            for (int i = 0; i < 3; i++) {
                if (!violating [i]) {
                    tris.Add (tris [triangle + i]);
                }
                // If our current vertex is outside/inside the plane, and our next is the opposite...
                if (violating [i] ^ violating [(i + 1) % 3]) {
                    Vector3 v = LineCast (plane, verts [tris [triangle + i]], verts [tris [triangle + ((i + 1) % 3)]]);
                    verts.Add (v);
                    uvs.Add (new Vector2 (v.x + 0.5f, v.z + 0.5f));
                    tris.Add (verts.Count - 1);
                }
            }
            tris.RemoveRange (triangle, 3);
            return 3;
        } else if (violationCount == 1) {
            Vector3 v1 = Vector3.zero, v2 = Vector3.zero, v3 = Vector3.zero;
            int i1 = -1, i2 = -1, i3 = -1;
            for (int i = 0; i < 3; i++) {
                if (violating [i]) {
                    i1 = tris [triangle + i];
                    i2 = tris [triangle + (i + 1) % 3];
                    i3 = tris [triangle + (i + 2) % 3];
                    v1 = verts [i1];
                    v2 = verts [i2];
                    v3 = verts [i3];
                    break;
                }
            }
            Vector3 v = LineCast (plane, v1, v2);
            verts.Add (v);
            uvs.Add (new Vector2 (v.x + 0.5f, v.z + 0.5f));
            int i4 = verts.Count - 1;
            tris.Add (i4);
            tris.Add (i2);
            tris.Add (i3);

            tris.Add (i3);
            v = LineCast (plane, v3, v1);
            verts.Add (v);
            uvs.Add (new Vector2 (v.x + 0.5f, v.z + 0.5f));
            tris.Add (verts.Count - 1);
            tris.Add (i4);
            tris.RemoveRange (triangle, 3);
            return 3;
        }   
        return 0;
    }
    private void ClipPlane( List<Vector3> verts, List<Vector2> uvs, List<int> tris, Plane plane ) {
        int triCount = tris.Count;
        for (int i = 0; i < triCount; i += 3) {
            int deleted = ClipTriangle (verts, uvs, tris, i, plane);
            i -= deleted;
            triCount -= deleted;
        }
    }
    private static Vector3 LineCast(Plane plane, Vector3 a, Vector3 b) {
        float dis;
        Ray ray = new Ray(a, b-a);
        plane.Raycast( ray, out dis );
        return ray.GetPoint(dis);
    }
    private void FinishMesh() {
        ClipPlane( newVerts, newUV, newTri, new Plane( Vector3.right, Vector3.right/2f ));
        ClipPlane( newVerts, newUV, newTri, new Plane( -Vector3.right, -Vector3.right/2f ));
        ClipPlane( newVerts, newUV, newTri, new Plane( Vector3.forward, Vector3.forward/2f ));
        ClipPlane( newVerts, newUV, newTri, new Plane( -Vector3.forward, -Vector3.forward/2f ));
        ClipPlane( newVerts, newUV, newTri, new Plane( Vector3.up, Vector3.up/2f ));
        ClipPlane( newVerts, newUV, newTri, new Plane( -Vector3.up, -Vector3.up/2f ));
        Mesh newMesh = new Mesh ();
        newMesh.name = "DecalMesh";
        GetComponent<MeshFilter> ().mesh = newMesh;
        newMesh.vertices = newVerts.ToArray ();
        newMesh.uv = newUV.ToArray ();
        newMesh.triangles = newTri.ToArray ();
    }

    private List<GameObject> GetAffectedObjects() {
        List<GameObject> objects = new List<GameObject>();
        foreach( Collider col in Physics.OverlapBox(transform.position, transform.lossyScale, transform.rotation, layerMask, QueryTriggerInteraction.Ignore) ) {
            Renderer r = col.gameObject.GetComponent<Renderer>();
            // If the object doesn't render anything, ignore.
            if (r == null ) continue;
            if (objects.Contains (col.gameObject)) continue;
            if ( r.GetComponent<BSPTree>() == null ) continue;
            // If we're trying to apply to a disabled object, ignore.
            if( !r.enabled ) continue;
            // If we are trying to apply ourselves to another decal, ignore.
            if( r.GetComponent<Decal>() != null ) continue;
            if ( r.GetComponent<MeshFilter>() == null ) continue;
            // If we're a trigger, ignore.
            if( col.isTrigger ) continue;
            // If we're not static, ignore.
            if ( r.GetComponent<Rigidbody>() != null ) {
                continue;
            }
            if ( r.GetComponent<Movable>() != null ) {
                continue;
            }
            objects.Add(r.gameObject);
        }
        return objects;
    }
}
