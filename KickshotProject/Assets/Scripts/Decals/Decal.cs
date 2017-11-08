using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(MeshFilter) )]
[RequireComponent( typeof(MeshRenderer) )]
public class Decal : MonoBehaviour {
    public Material decal;
    public float offset = 0.01f;
    public bool randomRotateOnSpawn = false;
    public LayerMask layerMask;
    public List<GameObject> affectedObjects;
    private List<Vector3> newVerts = new List<Vector3> ();
    private List<Vector2> newUV = new List<Vector2>();
    //private List<Vector2> newNormals = new List<Vector2>();
    private List<int> newTri = new List<int> ();
    Dictionary<int, int> indexLookup = new Dictionary<int, int>();
    [HideInInspector]
    public List<GameObject> subDecals = new List<GameObject> ();

    private int triangleCount = 0;

    void Start() {
        // Randomly rotate the decal around its up axis.
        if (randomRotateOnSpawn) {
            transform.RotateAround (transform.position, transform.up, Random.Range (0f, 360f));
        }
        // Set the texture of our decal.
        GetComponent<MeshRenderer> ().material = decal;
        BuildDecal ();
        // Make sure we don't re-build the mesh by saying our transform hasn't changed.
        transform.hasChanged = false;
        if (triangleCount <= 0) {
            Destroy (gameObject);
        }
    }

    // This shows a pretty little box when we're in the editor.
    void OnDrawGizmosSelected() {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube( Vector3.zero, Vector3.one );
    }

    void Update() {
        // Check if our transform has changed while editing, and rebuld the mesh if so.
        // This doesn't run if we're playing since the decal would be rebuilt whenever
        // its parent moves, and we don't need that.
        if (transform.hasChanged && !Application.isPlaying && Application.isEditor ) {
            foreach (GameObject obj in subDecals) {
                for (int i = 0; i < obj.transform.childCount; i++) {
                    DestroyImmediate (obj.transform.GetChild (i).gameObject, true);
                }
                DestroyImmediate (obj, true);
            }
            subDecals.Clear ();
            BuildDecal ();
            transform.hasChanged = false;
        }
    }

    private void InstanciateDecalForMovable( GameObject obj ) {
        // Clear whatever mesh we might have generated already.
        StartBuildMesh ();
        BSPTree affectedMesh = obj.GetComponent<BSPTree> ();
        BuildMeshForObject (obj, affectedMesh);
        GameObject subDecal = new GameObject("SubDecalMesh");
        subDecal.transform.position = transform.position;
        subDecal.transform.rotation = transform.rotation;
        subDecal.transform.localScale = transform.localScale;
        GameObject subDecalTarget = new GameObject("SubDecalTarget");
        subDecalTarget.transform.position = transform.position;
        subDecalTarget.transform.rotation = transform.rotation;
        subDecalTarget.transform.SetParent (obj.transform);
        Follower subMeshFollower = subDecal.AddComponent<Follower> ();
        subMeshFollower.target = subDecalTarget.transform;
        MeshFilter subMeshFilter = subDecal.AddComponent<MeshFilter> ();
        MeshRenderer subRenderer = subDecal.AddComponent<MeshRenderer> ();
        subRenderer.material = decal;
        FinishMesh (subMeshFilter);
        subDecals.Add (subDecal);
        subDecals.Add (subDecalTarget);
    }

    void OnDestroy() {
        foreach (GameObject obj in subDecals) {
            Destroy (obj);
        }
        subDecals.Clear ();
    }

    // Try our best to determine how to apply our mesh.
    public void BuildDecal() {
        // Separate our affected objects into moving and static objects.
        affectedObjects = GetAffectedObjects ();
        List<GameObject> movingObjects = new List<GameObject> ();
        List<GameObject> staticObjects = new List<GameObject> ();
        foreach (GameObject obj in affectedObjects) {
            if (obj.GetComponent<Movable> () != null || obj.GetComponent<Rigidbody> () != null) {
                movingObjects.Add (obj);
            } else {
                staticObjects.Add (obj);
            }
        }
        foreach (GameObject obj in movingObjects) {
            InstanciateDecalForMovable (obj);
        }
        // Clear whatever mesh we might have generated already.
        StartBuildMesh ();
        // Try building a mesh for each static object.
        foreach (GameObject obj in staticObjects) {
            BSPTree affectedMesh = obj.GetComponent<BSPTree> ();
            BuildMeshForObject (obj, affectedMesh);
        }
        // Clip the mesh to fall within our boundaries, then set the mesh.
        FinishMesh (GetComponent<MeshFilter>());
    }

    private void StartBuildMesh() {
        newVerts.Clear ();
        newUV.Clear ();
        newTri.Clear ();
    }

    private void BuildMeshForObject( GameObject obj, BSPTree tree ) {
        // Clear the index lookup, we use it to check which verticies are shared.
        // This keeps us from having to rebuild the mesh so intricately.
        indexLookup.Clear ();
        List<int> triangles = new List<int>();
        // Use a BSP tree to find nearby triangles at log(n) speeds.
        tree.FindClosestTriangles (transform.position, transform.lossyScale.magnitude/2f, triangles);
        // Calculate the matrix needed to transform a point from the obj's local mesh, to our local mesh.
        Matrix4x4 mat = transform.worldToLocalMatrix * obj.transform.localToWorldMatrix;
        for (int i = 0; i < triangles.Count; i++) {
            // We use the indices of the original mesh's triangles to determine which
            // verticies are shared. So we don't have to calculate that ourselves.
            // we grab them here.
            int i1, i2, i3;
            tree.GetIndices (triangles [i], out i1, out i2, out i3);

            // Here we get the points of the obj's mesh in our local space.
            Vector3 v1, v2, v3;
            tree.GetVertices (triangles [i], out v1, out v2, out v3);
            v1 = mat.MultiplyPoint (v1);
            v2 = mat.MultiplyPoint (v2);
            v3 = mat.MultiplyPoint (v3);

            // We do a quick normal calculation to see if the triangle is mostly facing us.
            // we have to recalculate it since the normal is different in our local space
            // (maybe we could just transform the original bsptree's precomputed normals with the matrix ?)
            Vector3 side1 = v2 - v1;
            Vector3 side2 = v3 - v1;
            Vector3 normal = Vector3.Cross(side1, side2).normalized;

            if (normal.y <= 0.2f) {
                continue;
            }

            // To prevent z-fighting, I randomly offset each vertex by the normal.
            float off = Random.Range(0f,1f)*offset;
            v1 += normal * off;
            v2 += normal * off;
            v3 += normal * off;

            // First we check to see if a vertex has already been grabbed and calculated.
            // If it has been, we just use that as the index for the triangle.
            // Otherwise we create a new vertex, with coorisponding UV mapping.
            // Since we're in a local space where the decal spans from -0.5f to 0.5f,
            // Our UV is just the x and z values in the space offset by 0.5f
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

            // Finally we add the triangle to the triangle list.
            newTri.Add (ni1);
            newTri.Add (ni2);
            newTri.Add (ni3);
        }
    }
    // This function takes a mesh, a triangle index, and a plane
    // Then it tries to clip the triangle by the plane, creating new
    // triangles if needed. It returns how many triangle indices have
    // been removed (and thus, if iterating, would use that value to offset the current index pointer.)
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
        // I couldn't think of the general case of generating new triangles.
        // So i break the problem into each of its pieces.
        switch( violationCount ) {
            // If no vertices were outside the plane, we do nothing.
            case 0:
                return 0;
            // If one vertex is outside the plane..
            case 1:
                // It was difficult for me to think of a general solution
                // So I first find which of the three vertices are the violating one.
                Vector3 v1 = Vector3.zero, v2 = Vector3.zero, v3 = Vector3.zero;
                int i1 = -1, i2 = -1, i3 = -1;
                for (int i = 0; i < 3; i++) {
                    // Once found, I record their indices, and vertex locations while keeping
                    // their order intact.
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
                // Create triangle number one
                Vector3 nv = LineCast (plane, v1, v2);
                verts.Add (nv);
                uvs.Add (new Vector2 (nv.x + 0.5f, nv.z + 0.5f));
                int i4 = verts.Count - 1;
                tris.Add (i4);
                tris.Add (i2);
                tris.Add (i3);

                // Create triangle number two
                tris.Add (i3);
                nv = LineCast (plane, v3, v1);
                verts.Add (nv);
                uvs.Add (new Vector2 (nv.x + 0.5f, nv.z + 0.5f));
                tris.Add (verts.Count - 1);
                tris.Add (i4);
                // Delete the old triangle.
                tris.RemoveRange (triangle, 3);
                return 3;
            // If two vertices are outside the plane...
            case 2:
                for (int i = 0; i < 3; i++) {
                    // If we're a vertex thats not outside the plane, we are going to be part of the new triangle.
                    if (!violating [i]) {
                        tris.Add (tris [triangle + i]);
                    }
                    // We use XOR to check if we cross the plane while going to the next vertex.
                    if (violating [i] ^ violating [(i + 1) % 3]) {
                        // If we did cross a plane, we create a new vertex and use that as part of our triangle.
                        Vector3 v = LineCast (plane, verts [tris [triangle + i]], verts [tris [triangle + ((i + 1) % 3)]]);
                        verts.Add (v);
                        uvs.Add (new Vector2 (v.x + 0.5f, v.z + 0.5f));
                        tris.Add (verts.Count - 1);
                    }
                }
                // Delete our old triangle.
                tris.RemoveRange (triangle, 3);
                return 3;
            // If all of our vertices are outside the plane, we just delete the whole triangle and exit.
            case 3:
                tris.RemoveRange (triangle, 3);
                return 3;
        }   
        return 0;
    }
    // Clips the given mesh to fit entirely on one side of the plane.
    private void ClipPlane( List<Vector3> verts, List<Vector2> uvs, List<int> tris, Plane plane ) {
        int triCount = tris.Count;
        for (int i = 0; i < triCount; i += 3) {
            int deleted = ClipTriangle (verts, uvs, tris, i, plane);
            i -= deleted;
            triCount -= deleted;
        }
    }
    // Find the point on a plane where a line intersects.
    private static Vector3 LineCast(Plane plane, Vector3 a, Vector3 b) {
        float dis;
        Ray ray = new Ray(a, b-a);
        plane.Raycast( ray, out dis );
        return ray.GetPoint(dis);
    }

    // Clips the mesh, and sends it to the renderer.
    private void FinishMesh(MeshFilter mf) {
        ClipPlane( newVerts, newUV, newTri, new Plane( Vector3.right, Vector3.right/2f ));
        ClipPlane( newVerts, newUV, newTri, new Plane( -Vector3.right, -Vector3.right/2f ));
        ClipPlane( newVerts, newUV, newTri, new Plane( Vector3.forward, Vector3.forward/2f ));
        ClipPlane( newVerts, newUV, newTri, new Plane( -Vector3.forward, -Vector3.forward/2f ));
        ClipPlane( newVerts, newUV, newTri, new Plane( Vector3.up, Vector3.up/2f ));
        ClipPlane( newVerts, newUV, newTri, new Plane( -Vector3.up, -Vector3.up/2f ));
        triangleCount += newVerts.Count;
        Mesh newMesh = new Mesh ();
        newMesh.name = "DecalMesh";
        mf.mesh = newMesh;
        newMesh.vertices = newVerts.ToArray ();
        newMesh.uv = newUV.ToArray ();
        newMesh.triangles = newTri.ToArray ();
    }

    // Use the physics engine to get nearby affected objects.
    private List<GameObject> GetAffectedObjects() {
        List<GameObject> objects = new List<GameObject>();
        foreach( Collider col in Physics.OverlapBox(transform.position, transform.lossyScale/2f, transform.rotation, layerMask, QueryTriggerInteraction.Ignore) ) {
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
            objects.Add(r.gameObject);
        }
        return objects;
    }
}
