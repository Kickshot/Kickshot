using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using UnityEngine;

public static class Helper {
    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f) {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }
    public static int GetHitScanLayerMask() {
        //Hit everything but players, playerclips, and ignore raycast layers.
        return ~((1 << LayerMask.NameToLayer ("Player")) | (1 << LayerMask.NameToLayer ("PlayerClip")) | (1 << LayerMask.NameToLayer ("IgnoreRaycast")));
    }
    public static int GetLayerMask( GameObject obj ) {
        // This generates our layermask, making sure we only collide with stuff that's specified by the physics engine.
        // This makes it so that if we specify in-engine layers to not collide with the player, that we actually abide to it.
        int myLayer = obj.layer;
        int layerMask = 0;
        for (int i = 0; i < 32; i++) {
            if (!Physics.GetIgnoreLayerCollision (myLayer, i)) {
                layerMask = layerMask | 1 << i;
            }
        }
        return layerMask;
    }
    // Modulo implementation since C# doesn't have it.
    public static float fmod(float a, float b) {
        return a - b * Mathf.Floor (a / b);
    }

    // Pretty expensive way to grab a material, but I don't know of any other way :v
    public static Material getMaterial( RaycastHit hit ) {
        MeshFilter meshf = hit.collider.gameObject.GetComponent<MeshFilter> ();
        if (meshf != null) {
            Mesh mesh = meshf.mesh;
            if (mesh != null) {
                return getMaterialFromMesh (hit, mesh, hit.collider.gameObject);
            }
        }
        return null;
    }

    public static byte[] Save<T>( T obj ) {
        MemoryStream o = new MemoryStream ();
        BinaryFormatter binarySerializer = new BinaryFormatter();
        binarySerializer.Serialize(o,obj);
        return o.GetBuffer ();
    }

    public static T Load<T>( byte[] state ) {
        BinaryFormatter binarySerializer = new BinaryFormatter();
        MemoryStream o = new MemoryStream (state);
        return (T)binarySerializer.Deserialize (o);
    }

    public static Material getMaterialFromMesh( RaycastHit hit, Mesh mesh, GameObject parent ) {
        Renderer r = parent.GetComponent<Renderer> ();
        if (r == null) {
            return null;
        }
        if (mesh.subMeshCount == 1) {
            return r.material;
        }
        if ( !mesh.isReadable ) {
            Debug.LogError("Couldn't read mesh because it has a Batching Static flag enabled. Returning base material.");
            return r.material;
        }
        if (hit.collider is MeshCollider) {
            // Search for which submesh we hit, once we find it, return the coorisponding material.
            Vector3 findTri = new Vector3 (mesh.triangles [hit.triangleIndex * 3],
                                  mesh.triangles [hit.triangleIndex * 3 + 1],
                                  mesh.triangles [hit.triangleIndex * 3 + 2]);
            for (int i = 0; i < mesh.subMeshCount; i++) {
                int[] subMesh = mesh.GetTriangles (i);
                for (int j = 0; j < subMesh.Length; j += 3) {
                    Vector3 tri = new Vector3 (subMesh [j], subMesh [j + 1], subMesh [j + 2]);
                    if (tri == findTri) {
                        return r.materials [i];
                    }
                }
            }
        } else {
            return r.materials [0];
        }
        return null;
    }
    //Draws just the box at where it is currently hitting.
    public static void DrawBoxCastOnHit(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float hitInfoDistance, Color color)
    {
        origin = CastCenterOnCollision(origin, direction, hitInfoDistance);
        DrawBox(origin, halfExtents, orientation, color);
    }

    //Draws the full box from start of cast to its end distance. Can also pass in hitInfoDistance instead of full distance
    public static void DrawBoxCastBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float distance, Color color)
    {
        direction.Normalize();
        Box bottomBox = new Box(origin, halfExtents, orientation);
        Box topBox = new Box(origin + (direction * distance), halfExtents, orientation);

        Debug.DrawLine(bottomBox.backBottomLeft, topBox.backBottomLeft,    color);
        Debug.DrawLine(bottomBox.backBottomRight, topBox.backBottomRight, color);
        Debug.DrawLine(bottomBox.backTopLeft, topBox.backTopLeft, color);
        Debug.DrawLine(bottomBox.backTopRight, topBox.backTopRight,    color);
        Debug.DrawLine(bottomBox.frontTopLeft, topBox.frontTopLeft,    color);
        Debug.DrawLine(bottomBox.frontTopRight, topBox.frontTopRight, color);
        Debug.DrawLine(bottomBox.frontBottomLeft, topBox.frontBottomLeft, color);
        Debug.DrawLine(bottomBox.frontBottomRight, topBox.frontBottomRight,    color);

        DrawBox(bottomBox, color);
        DrawBox(topBox, color);
    }

    public static void DrawBox(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Color color)
    {
        DrawBox(new Box(origin, halfExtents, orientation), color);
    }
    public static void DrawBox(Box box, Color color)
    {
        Debug.DrawLine(box.frontTopLeft,     box.frontTopRight,    color);
        Debug.DrawLine(box.frontTopRight,     box.frontBottomRight, color);
        Debug.DrawLine(box.frontBottomRight, box.frontBottomLeft, color);
        Debug.DrawLine(box.frontBottomLeft,     box.frontTopLeft, color);

        Debug.DrawLine(box.backTopLeft,         box.backTopRight, color);
        Debug.DrawLine(box.backTopRight,     box.backBottomRight, color);
        Debug.DrawLine(box.backBottomRight,     box.backBottomLeft, color);
        Debug.DrawLine(box.backBottomLeft,     box.backTopLeft, color);

        Debug.DrawLine(box.frontTopLeft,     box.backTopLeft, color);
        Debug.DrawLine(box.frontTopRight,     box.backTopRight, color);
        Debug.DrawLine(box.frontBottomRight, box.backBottomRight, color);
        Debug.DrawLine(box.frontBottomLeft,     box.backBottomLeft, color);
    }

    public struct Box
    {
        public Vector3 localFrontTopLeft     {get; private set;}
        public Vector3 localFrontTopRight    {get; private set;}
        public Vector3 localFrontBottomLeft  {get; private set;}
        public Vector3 localFrontBottomRight {get; private set;}
        public Vector3 localBackTopLeft      {get {return -localFrontBottomRight;}}
        public Vector3 localBackTopRight     {get {return -localFrontBottomLeft;}}
        public Vector3 localBackBottomLeft   {get {return -localFrontTopRight;}}
        public Vector3 localBackBottomRight  {get {return -localFrontTopLeft;}}

        public Vector3 frontTopLeft     {get {return localFrontTopLeft + origin;}}
        public Vector3 frontTopRight    {get {return localFrontTopRight + origin;}}
        public Vector3 frontBottomLeft  {get {return localFrontBottomLeft + origin;}}
        public Vector3 frontBottomRight {get {return localFrontBottomRight + origin;}}
        public Vector3 backTopLeft      {get {return localBackTopLeft + origin;}}
        public Vector3 backTopRight     {get {return localBackTopRight + origin;}}
        public Vector3 backBottomLeft   {get {return localBackBottomLeft + origin;}}
        public Vector3 backBottomRight  {get {return localBackBottomRight + origin;}}

        public Vector3 origin {get; private set;}

        public Box(Vector3 origin, Vector3 halfExtents, Quaternion orientation) : this(origin, halfExtents)
        {
            Rotate(orientation);
        }
        public Box(Vector3 origin, Vector3 halfExtents)
        {
            this.localFrontTopLeft     = new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
            this.localFrontTopRight    = new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
            this.localFrontBottomLeft  = new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
            this.localFrontBottomRight = new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);

            this.origin = origin;
        }


        public void Rotate(Quaternion orientation)
        {
            localFrontTopLeft     = RotatePointAroundPivot(localFrontTopLeft    , Vector3.zero, orientation);
            localFrontTopRight    = RotatePointAroundPivot(localFrontTopRight   , Vector3.zero, orientation);
            localFrontBottomLeft  = RotatePointAroundPivot(localFrontBottomLeft , Vector3.zero, orientation);
            localFrontBottomRight = RotatePointAroundPivot(localFrontBottomRight, Vector3.zero, orientation);
        }
    }

    //This should work for all cast types
    static Vector3 CastCenterOnCollision(Vector3 origin, Vector3 direction, float hitInfoDistance)
    {
        return origin + (direction.normalized * hitInfoDistance);
    }

    static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
    {
        Vector3 direction = point - pivot;
        return pivot + rotation * direction;
    }
}
