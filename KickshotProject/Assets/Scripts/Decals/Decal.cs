﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

[RequireComponent( typeof(MeshFilter) )]
[RequireComponent( typeof(MeshRenderer) )]
public class Decal : MonoBehaviour {
    public Material decal;
    public float offset = 0.005f;
    public bool randomRotateOnSpawn = false;
    public LayerMask layerMask;
    public List<GameObject> affectedObjects;
    private List<Vector3> newVerts = new List<Vector3> ();
    private List<Vector2> newUV = new List<Vector2>();
    private List<int> newTri = new List<int> ();
    [HideInInspector]
    public List<GameObject> subDecals = new List<GameObject> ();
    private List<DecalBuilder> jobs = new List<DecalBuilder>();
    private bool buildingMesh = false;

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
    }

    // This shows a pretty little box when we're in the editor.
    void OnDrawGizmosSelected() {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube( Vector3.zero, Vector3.one );
    }

    public void Update() {
        // Check if our transform has changed while editing, and rebuld the mesh if so.
        // This doesn't run if we're playing since the decal would be rebuilt whenever
        // its parent moves, and we don't need that.
        if (transform.hasChanged && !Application.isPlaying && Application.isEditor ) {
            BuildDecal ();
            transform.hasChanged = false;
        }
        for (int i = 0; i < jobs.Count; i++) {
            if (jobs[i].Update()) {
                jobs.RemoveAt (i);
                i--;
            }
        }
        if (jobs.Count <= 0 && buildingMesh) {
            buildingMesh = false;
            Mesh newMesh = new Mesh ();
            newMesh.name = "DecalMesh";
            GetComponent<MeshFilter> ().mesh = newMesh;
            newMesh.vertices = newVerts.ToArray ();
            newMesh.uv = newUV.ToArray ();
            newMesh.triangles = newTri.ToArray();
        }
    }

    private MeshFilter InstanciateDecalForMovable( GameObject obj, GameObject target ) {
        if (target == null) {
            Debug.LogError ("We shouldn't ever get a null target for moving decals...");
        }
        GameObject subDecal = new GameObject("SubDecalMesh");
        subDecal.transform.position = transform.position;
        subDecal.transform.rotation = transform.rotation;
        subDecal.transform.localScale = transform.localScale;
        Follower subMeshFollower = subDecal.AddComponent<Follower> ();
        subMeshFollower.target = target.transform;
        MeshFilter subMeshFilter = subDecal.AddComponent<MeshFilter> ();
        MeshRenderer subRenderer = subDecal.AddComponent<MeshRenderer> ();
        subRenderer.material = decal;
        subDecals.Add (subDecal);
        return subMeshFilter;
    }

    void OnDestroy() {
        foreach (GameObject obj in subDecals) {
            Destroy (obj);
        }
        subDecals.Clear ();
    }

    // Radius
    private float GetScale(GameObject obj ) {
        Vector3 scale = transform.lossyScale;
        Vector3 otherScale = obj.transform.lossyScale;
        float maxScale = Mathf.Max(Mathf.Max(scale.x,scale.y),scale.z)/2f;
        float minScale = Mathf.Min(Mathf.Min(otherScale.x,otherScale.y),otherScale.z);
        return maxScale / minScale;
    }

    // Try our best to determine how to apply our mesh.
    public void BuildDecal() {
        buildingMesh = true;
        // Clear whatever mesh we might have generated already.
        StartBuildMesh ();
        // Separate our affected objects into moving and static objects.
        affectedObjects = GetAffectedObjects ();
        List<GameObject> movingObjects = new List<GameObject> ();
        List<GameObject> staticObjects = new List<GameObject> ();
        foreach (GameObject obj in affectedObjects) {
            if (obj.GetComponent<Movable> () != null || obj.GetComponent<Rigidbody> () != null || obj.GetComponentInParent<Movable> () != null || obj.GetComponentInParent<Rigidbody> () != null ) {
                movingObjects.Add (obj);
            } else {
                staticObjects.Add (obj);
            }
        }
        foreach (GameObject obj in movingObjects) {
            GameObject subDecalTarget = new GameObject("SubDecalTarget");
            subDecalTarget.transform.position = transform.position;
            subDecalTarget.transform.rotation = transform.rotation;
            subDecalTarget.transform.SetParent (obj.transform);
            subDecals.Add (subDecalTarget);

            BSPTree affectedMesh = obj.GetComponent<BSPTree> ();
            DecalBuilder builder = new DecalBuilder ();
            builder.mat = transform.worldToLocalMatrix * obj.transform.localToWorldMatrix;
            builder.position = affectedMesh.transform.InverseTransformPoint(transform.position);
            builder.scale = GetScale (obj);
            builder.tree = affectedMesh;
            builder.offset = offset*Random.Range(0.1f,1f);
            builder.decal = this;
            builder.target = obj;
            builder.isStatic = false;
            builder.targetPos = subDecalTarget;
            builder.Start ();
            jobs.Add (builder);
        }
        // Try building a mesh for each static object.
        foreach (GameObject obj in staticObjects) {
            BSPTree affectedMesh = obj.GetComponent<BSPTree> ();
            DecalBuilder builder = new DecalBuilder ();
            builder.mat = transform.worldToLocalMatrix * obj.transform.localToWorldMatrix;
            builder.position = affectedMesh.transform.InverseTransformPoint(transform.position);
            builder.scale = GetScale (obj);
            builder.tree = affectedMesh;
            builder.offset = offset*Random.Range(0.1f,1f);
            builder.decal = this;
            builder.target = obj;
            builder.isStatic = true;
            builder.Start ();
            jobs.Add (builder);
        }
    }

    private void StartBuildMesh() {
        foreach (GameObject obj in subDecals) {
            if (obj == null) {
                continue;
            }
            for (int i = 0; i < obj.transform.childCount; i++) {
                if (obj.transform.GetChild (i) == null) {
                    continue;
                }
                DestroyImmediate (obj.transform.GetChild (i).gameObject, true);
            }
            DestroyImmediate (obj, true);
        }
        foreach (DecalBuilder job in jobs) {
            job.Abort ();
        }
        jobs.Clear ();
        subDecals.Clear ();
        newVerts.Clear ();
        newUV.Clear ();
        newTri.Clear ();
    }

    public void FinishMesh( bool isStatic, GameObject obj, List<Vector3> verts, List<Vector2> uvs, LinkedList<int> tris, GameObject target = null) {
        if (tris.Count <= 0) {
            return;
        }
        // For moving objects, we create a new object to parent to it.
        if (!isStatic) {
            MeshFilter mf = InstanciateDecalForMovable (obj, target);
            Mesh newMesh = new Mesh ();
            newMesh.name = "DecalMesh";
            mf.mesh = newMesh;
            newMesh.vertices = verts.ToArray ();
            newMesh.uv = uvs.ToArray ();
            int[] triangles = new int[tris.Count];
            tris.CopyTo (triangles, 0);
            newMesh.triangles = triangles;
            return;
        }
        // For everything else, we collect all of the mesh data from our jobs, then finally collapse them into a singleton mesh.
        int offset = newVerts.Count;
        newVerts.AddRange (verts);
        newUV.AddRange (uvs);
        int[] triangles1 = new int[tris.Count];
        tris.CopyTo (triangles1, 0);
        for (int i = 0; i < triangles1.Length; i++) {
            triangles1 [i] += offset;
        }
        newTri.AddRange (triangles1);
    }

    // Use the physics engine to get nearby affected objects.
    private List<GameObject> GetAffectedObjects() {
        List<GameObject> objects = new List<GameObject>();
        foreach( Collider col in Physics.OverlapBox(transform.position, transform.lossyScale/2f, transform.rotation, layerMask, QueryTriggerInteraction.Ignore) ) {
            if (objects.Contains (col.gameObject)) continue;
            if ( col.gameObject.GetComponent<BSPTree>() == null ) continue;
            // If we are trying to apply ourselves to another decal, ignore.
            if( col.gameObject.GetComponent<Decal>() != null ) continue;
            // If we're a trigger, ignore.
            if( col.isTrigger ) continue;
            objects.Add(col.gameObject);
        }
        return objects;
    }
}
