using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
            //Debug.LogError("Couldn't read mesh because it has a Batching Static flag enabled. Returning base material.");
            return r.material;
        }
        if (!(hit.collider is MeshCollider)) {
            return r.material;
        }
        if (mesh.triangles.Length > 30000) {
            //Debug.LogError ("A mesh is too complicated to optimally grab materials. This causes performance problems! Try splitting it up in the editor...");
        }
        if (hit.triangleIndex < 0 || hit.triangleIndex > mesh.triangles.Length) {
            return r.material;
        }
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
        return r.material;
    }
    //Draws just the box at where it is currently hitting.
    /*public static void DrawBoxCastOnHit(Vector3 origin, Vector3 halfExtents, Quaternion orientation, Vector3 direction, float hitInfoDistance, Color color)
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
    }*/
    private class VertexData {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 uv;
        public VertexData( Vector3 pos, Vector3 norm, Vector2 u ) {
            position = pos;
            normal = norm;
            uv = u;
        }
        public override int GetHashCode()
        {
            return position.GetHashCode() ^ normal.GetHashCode() ^ uv.GetHashCode();
        }
        public override bool Equals (object obj) {
            VertexData b = (VertexData)obj;
            return position.Equals (b.position) && normal.Equals (b.normal) && uv.Equals (b.uv);
        }
    }

    #if UNITY_EDITOR
    // Welds similar vertices, saves memory. Then uses MeshUtility to optimize it.
    // It's pretty much only useful if you generate some really lazy meshes...
    public static void OptimizeMesh(Mesh m) {
        Debug.Log ("Optimizing mesh with " + m.vertices.Length + " vertices...");
        Dictionary<VertexData, int> memory = new Dictionary<VertexData, int> ();
        int vertexCount = 0;
        List<List<int>> newTriangles = new List<List<int>> ();
        for (int i = 0; i < m.subMeshCount; i++) {
            int[] subtriangles = m.GetTriangles (i);
            List<int> newSubTriangles = new List<int> ();
            for (int tri = 0; tri < subtriangles.Length; tri += 3) {
                int i1 = subtriangles [tri];
                int i2 = subtriangles [tri+1];
                int i3 = subtriangles [tri+2];
                VertexData v1 = new VertexData (m.vertices [i1], m.normals [i1], m.uv [i1]);
                VertexData v2 = new VertexData (m.vertices [i2], m.normals [i2], m.uv [i2]);
                VertexData v3 = new VertexData (m.vertices [i3], m.normals [i3], m.uv [i3]);
                int n1, n2, n3;
                if (!memory.TryGetValue (v1, out n1)) {
                    n1 = vertexCount;
                    memory.Add (v1, vertexCount);
                    vertexCount++;
                }
                newSubTriangles.Add (n1);
                if (!memory.TryGetValue (v2, out n2)) {
                    n2 = vertexCount;
                    memory.Add (v2, vertexCount);
                    vertexCount++;
                }
                newSubTriangles.Add (n2);
                if (!memory.TryGetValue (v3, out n3)) {
                    n3 = vertexCount;
                    memory.Add (v3, vertexCount);
                    vertexCount++;
                }
                newSubTriangles.Add (n3);
            }
            newTriangles.Add (newSubTriangles);
        }
        // Sort our dictionary
        List<VertexData> data = new List<VertexData> (memory.Keys);
        foreach (KeyValuePair<VertexData,int> p in memory) {
            data [p.Value] = p.Key;
        }
        // Generate our lists from the organized data.
        List<Vector3> newVertices = new List<Vector3>();
        List<Vector3> newNormals = new List<Vector3>();
        List<Vector2> newUVs = new List<Vector2>();
        foreach( VertexData v in data ) {
            newVertices.Add (v.position);
            newNormals.Add (v.normal);
            newUVs.Add (v.uv);
        }
        m.Clear ();
        m.vertices = newVertices.ToArray ();
        m.normals = newNormals.ToArray ();
        m.uv = newUVs.ToArray ();
        m.subMeshCount = newTriangles.Count;
        for (int i = 0; i < newTriangles.Count; i++) {
            m.SetTriangles (newTriangles [i].ToArray(), i);
        }
        MeshUtility.Optimize (m);
        Debug.Log ("Done! It's down to " + m.vertices.Length + " now!");
    }

    #endif

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

    static public void SetLayerRecursively( GameObject obj , int newLayer  ) {
        obj.layer = newLayer;
        foreach( Transform child in obj.transform ) {
            SetLayerRecursively( child.gameObject, newLayer );
        }
    }

    public static bool TriangleIntersectsAABB( Vector3 a, Vector3 b, Vector3 c, Vector3 boxCenter, Vector3 boxExtents )
    {
        // Translate triangle as conceptually moving AABB to origin
        var v0 = ( a - boxCenter );
        var v1 = ( b - boxCenter );
        var v2 = ( c - boxCenter );

        // Compute edge vectors for triangle
        var f0 = ( v1 - v0 );
        var f1 = ( v2 - v1 );
        var f2 = ( v0 - v2 );

        #region Test axes a00..a22 (category 3)

        // Test axis a00
        var a00 = new Vector3( 0, -f0.z, f0.y );
        var p0 = Vector3.Dot( v0, a00 );
        var p1 = Vector3.Dot( v1, a00 );
        var p2 = Vector3.Dot( v2, a00 );
        var r = boxExtents.y * Mathf.Abs( f0.z ) + boxExtents.z * Mathf.Abs( f0.y );
        if( Mathf.Max( -Mathf.Max(p0, p1, p2 ), Mathf.Min(p0, p1, p2 ) ) > r )
        {
            return false;
        }

        // Test axis a01
        var a01 = new Vector3( 0, -f1.z, f1.y );
        p0 = Vector3.Dot( v0, a01 );
        p1 = Vector3.Dot( v1, a01 );
        p2 = Vector3.Dot( v2, a01 );
        r = boxExtents.y * Mathf.Abs( f1.z ) + boxExtents.z * Mathf.Abs( f1.y );
        if( Mathf.Max( -Mathf.Max(p0, p1, p2 ), Mathf.Min(p0, p1, p2 ) ) > r )
        {
            return false;
        }

        // Test axis a02
        var a02 = new Vector3( 0, -f2.z, f2.y );
        p0 = Vector3.Dot( v0, a02 );
        p1 = Vector3.Dot( v1, a02 );
        p2 = Vector3.Dot( v2, a02 );
        r = boxExtents.y * Mathf.Abs( f2.z ) + boxExtents.z * Mathf.Abs( f2.y );
        if( Mathf.Max( -Mathf.Max(p0, p1, p2 ), Mathf.Min(p0, p1, p2 ) ) > r )
        {
            return false;
        }

        // Test axis a10
        var a10 = new Vector3( f0.z, 0, -f0.x );
        p0 = Vector3.Dot( v0, a10 );
        p1 = Vector3.Dot( v1, a10 );
        p2 = Vector3.Dot( v2, a10 );
        r = boxExtents.x * Mathf.Abs( f0.z ) + boxExtents.z * Mathf.Abs( f0.x );
        if( Mathf.Max( -Mathf.Max(p0, p1, p2 ), Mathf.Min(p0, p1, p2 ) ) > r )
        {
            return false;
        }

        // Test axis a11
        var a11 = new Vector3( f1.z, 0, -f1.x );
        p0 = Vector3.Dot( v0, a11 );
        p1 = Vector3.Dot( v1, a11 );
        p2 = Vector3.Dot( v2, a11 );
        r = boxExtents.x * Mathf.Abs( f1.z ) + boxExtents.z * Mathf.Abs( f1.x );
        if( Mathf.Max( -Mathf.Max(p0, p1, p2 ), Mathf.Min(p0, p1, p2 ) ) > r )
        {
            return false;
        }

        // Test axis a12
        var a12 = new Vector3( f2.z, 0, -f2.x );
        p0 = Vector3.Dot( v0, a12 );
        p1 = Vector3.Dot( v1, a12 );
        p2 = Vector3.Dot( v2, a12 );
        r = boxExtents.x * Mathf.Abs( f2.z ) + boxExtents.z * Mathf.Abs( f2.x );
        if( Mathf.Max( -Mathf.Max(p0, p1, p2 ), Mathf.Min(p0, p1, p2 ) ) > r )
        {
            return false;
        }

        // Test axis a20
        var a20 = new Vector3( -f0.y, f0.x, 0 );
        p0 = Vector3.Dot( v0, a20 );
        p1 = Vector3.Dot( v1, a20 );
        p2 = Vector3.Dot( v2, a20 );
        r = boxExtents.x * Mathf.Abs( f0.y ) + boxExtents.y * Mathf.Abs( f0.x );
        if( Mathf.Max( -Mathf.Max(p0, p1, p2 ), Mathf.Min(p0, p1, p2 ) ) > r )
        {
            return false;
        }

        // Test axis a21
        var a21 = new Vector3( -f1.y, f1.x, 0 );
        p0 = Vector3.Dot( v0, a21 );
        p1 = Vector3.Dot( v1, a21 );
        p2 = Vector3.Dot( v2, a21 );
        r = boxExtents.x * Mathf.Abs( f1.y ) + boxExtents.y * Mathf.Abs( f1.x );
        if( Mathf.Max( -Mathf.Max(p0, p1, p2 ), Mathf.Min(p0, p1, p2 ) ) > r )
        {
            return false;
        }

        // Test axis a22
        var a22 = new Vector3( -f2.y, f2.x, 0 );
        p0 = Vector3.Dot( v0, a22 );
        p1 = Vector3.Dot( v1, a22 );
        p2 = Vector3.Dot( v2, a22 );
        r = boxExtents.x * Mathf.Abs( f2.y ) + boxExtents.y * Mathf.Abs( f2.x );
        if( Mathf.Max( -Mathf.Max(p0, p1, p2 ), Mathf.Min(p0, p1, p2 ) ) > r )
        {
            return false;
        }

        #endregion 

        #region Test the three axes corresponding to the face normals of AABB b (category 1)

        // Exit if...
        // ... [-extents.x, extents.x] and [min(v0.x,v1.x,v2.x), max(v0.x,v1.x,v2.x)] do not overlap
        if( Mathf.Max(v0.x, v1.x, v2.x ) < -boxExtents.x || Mathf.Min(v0.x, v1.x, v2.x ) > boxExtents.x )
        {
            return false;
        }

        // ... [-extents.y, extents.y] and [min(v0.y,v1.y,v2.y), max(v0.y,v1.y,v2.y)] do not overlap
        if( Mathf.Max(v0.y, v1.y, v2.y ) < -boxExtents.y || Mathf.Min(v0.y, v1.y, v2.y ) > boxExtents.y )
        {
            return false;
        }

        // ... [-extents.z, extents.z] and [min(v0.z,v1.z,v2.z), max(v0.z,v1.z,v2.z)] do not overlap
        if( Mathf.Max(v0.z, v1.z, v2.z ) < -boxExtents.z || Mathf.Min(v0.z, v1.z, v2.z ) > boxExtents.z )
        {
            return false;
        }

        #endregion 

        #region Test separating axis corresponding to triangle face normal (category 2)

        var planeNormal = Vector3.Cross( f0, f1 );
        var planeDistance = Vector3.Dot( planeNormal, v0 );

        // Compute the projection interval radius of b onto L(t) = b.c + t * p.n
        r = boxExtents.x * Mathf.Abs( planeNormal.x )
            + boxExtents.y * Mathf.Abs( planeNormal.y )
            + boxExtents.z * Mathf.Abs( planeNormal.z );

        // Intersection occurs when plane distance falls within [-r,+r] interval
        if( planeDistance > r )
        {
            return false;
        }

        #endregion

        return true;
    }
}
