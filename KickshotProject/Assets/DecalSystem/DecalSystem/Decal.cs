using UnityEngine;
using System.Collections.Generic;

[RequireComponent( typeof(MeshFilter) )]
[RequireComponent( typeof(MeshRenderer) )]
public class Decal : MonoBehaviour {

    public Material material;
    public Sprite sprite;

    public float maxAngle = 90.0f;
    public float pushDistance = 0.009f;
    public LayerMask affectedLayers = -1;
    private GameObject[] affectedObjects;

    private Matrix4x4 oldMatrix;
    private Vector3 oldScale;

    void OnDrawGizmosSelected() {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube( Vector3.zero, Vector3.one );
    }

    public Bounds GetBounds() {
        Vector3 size = transform.lossyScale;
        Vector3 min = -size/2f;
        Vector3 max =  size/2f;

        Vector3[] vts = new Vector3[] {
            new Vector3(min.x, min.y, min.z),
                new Vector3(max.x, min.y, min.z),
                new Vector3(min.x, max.y, min.z),
                new Vector3(max.x, max.y, min.z),

                new Vector3(min.x, min.y, max.z),
                new Vector3(max.x, min.y, max.z),
                new Vector3(min.x, max.y, max.z),
                new Vector3(max.x, max.y, max.z),
        };

        for(int i=0; i<8; i++) {

            vts[i] = transform.TransformDirection( vts[i] );
        }

        min = max = vts[0];
        foreach(Vector3 v in vts) {
            min = Vector3.Min(min, v);
            max = Vector3.Max(max, v);
        }

        return new Bounds(transform.position, max-min);
    }

    // Update is called once per frame
    void Update() {
        // Only rebuild mesh when scaling
        //bool hasChanged = oldMatrix != transform.localToWorldMatrix;
        bool hasChanged = oldScale != transform.localScale;
        //oldMatrix = transform.localToWorldMatrix;
        oldScale = transform.localScale;


        if(hasChanged) {
            BuildDecal( this );
        }

    }

    public void BuildDecal(Decal decal) {
        MeshFilter filter = decal.GetComponent<MeshFilter>();
        if(filter == null) filter = decal.gameObject.AddComponent<MeshFilter>();
        if(decal.GetComponent<Renderer>() == null) decal.gameObject.AddComponent<MeshRenderer>();
        decal.material = decal.GetComponent<Renderer>().material;

        if(decal.material == null || decal.sprite == null) {
            filter.mesh = null;
            return;
        }

        affectedObjects = GetAffectedObjects(decal.GetBounds(), decal.affectedLayers);
        foreach(GameObject go in affectedObjects) {
            DecalBuilder.BuildDecalForObject( decal, go );
        }
        DecalBuilder.Push( decal.pushDistance );

        Mesh mesh = DecalBuilder.CreateMesh();
        if(mesh != null) {
            mesh.name = "DecalMesh";
            filter.mesh = mesh;
        }
    }

    private static GameObject[] GetAffectedObjects(Bounds bounds, LayerMask affectedLayers) {
        List<GameObject> objects = new List<GameObject>();
        foreach( Collider col in Physics.OverlapBox(bounds.center, bounds.extents, Quaternion.identity, affectedLayers, QueryTriggerInteraction.Ignore) ) {
            Renderer r = col.gameObject.GetComponent<Renderer>();
            // If the object doesn't render anything, ignore.
            if (r == null ) continue;
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
        return objects.ToArray();
    }
}
