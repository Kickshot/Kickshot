using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class CombineMeshes
{
    [MenuItem("Tools/Combine Selected Meshes")]
    private static void Combine() {
        List<CombineInstance> combine = new List<CombineInstance> ();
        Dictionary<Material,int> materials = new Dictionary<Material,int> ();
        Dictionary<int,Mesh> meshs = new Dictionary<int,Mesh> ();
        int matcount = 0;
        foreach (GameObject obj in Selection.gameObjects) {
            foreach( MeshFilter mf in obj.GetComponentsInChildren<MeshFilter>() ) {
                MeshRenderer mr = mf.GetComponent<MeshRenderer> ();
                if (mr == null) {
                    continue;
                }
                for (int i = 0; i < mf.sharedMesh.subMeshCount; i++) {
                    if (!materials.ContainsKey (mr.sharedMaterials [i])) {
                        materials.Add (mr.sharedMaterials [i], matcount);
                        meshs.Add (matcount, new Mesh());
                        matcount++;
                    }
                    Mesh m = meshs [materials [mr.sharedMaterials [i]]];

                    List<Vector3> vertices = new List<Vector3> (m.vertices);
                    List<Vector3> normals = new List<Vector3> (m.normals);
                    List<Vector2> uvs = new List<Vector2> (m.uv);
                    List<int> triangles = new List<int> (m.triangles);
                    int count = vertices.Count;
                    Matrix4x4 toWorld = mf.transform.localToWorldMatrix;
                    for (int tri = 0; tri < mf.sharedMesh.GetTriangles(i).Length; tri += 3) {
                        int i1 = mf.sharedMesh.GetTriangles(i)[tri];
                        int i2 = mf.sharedMesh.GetTriangles(i)[tri+1];
                        int i3 = mf.sharedMesh.GetTriangles(i)[tri+2];
                        Vector3 v1 = toWorld.MultiplyPoint (mf.sharedMesh.vertices [i1]);
                        Vector3 v2 = toWorld.MultiplyPoint (mf.sharedMesh.vertices [i2]);
                        Vector3 v3 = toWorld.MultiplyPoint (mf.sharedMesh.vertices [i3]);
                        vertices.Add (v1);
                        vertices.Add (v2);
                        vertices.Add (v3);
                        Vector3 side1 = v2 - v1;
                        Vector3 side2 = v3 - v1;
                        Vector3 normal = Vector3.Cross(side1, side2).normalized;
                        normals.Add (normal);
                        normals.Add (normal);
                        normals.Add (normal);
                        uvs.Add (mf.sharedMesh.uv [i1]);
                        uvs.Add (mf.sharedMesh.uv [i2]);
                        uvs.Add (mf.sharedMesh.uv [i3]);
                        triangles.Add (count++);
                        triangles.Add (count++);
                        triangles.Add (count++);
                    }  
                    m.vertices = vertices.ToArray();
                    m.normals = normals.ToArray();
                    m.uv = uvs.ToArray();
                    m.triangles = triangles.ToArray();
                }
            }
        }
        List<Material> matarray = new List<Material> (materials.Keys);
        foreach (KeyValuePair<Material,int> pair in materials) {
            matarray [pair.Value] = pair.Key;
        }
        int tempcount = 0;
        for (int i = 0; i < matarray.Count; i++) {
            if (meshs [materials [matarray [i]]].triangles.Length <= 0) {
                matarray.RemoveAt (i);
                i--;
                continue;
            }
            CombineInstance ci = new CombineInstance ();
            ci.mesh = meshs [materials[matarray[i]]];
            tempcount += ci.mesh.triangles.Length;
            //ci.subMeshIndex = pair.Value;
            ci.transform = Matrix4x4.identity;
            combine.Add (ci);
        }
        foreach (Material mat in matarray) {
            CombineInstance ci = new CombineInstance ();
            ci.mesh = meshs [materials[mat]];
            tempcount += ci.mesh.triangles.Length;
            ci.transform = Matrix4x4.identity;
            combine.Add (ci);
        }
        if (combine.Count <= 0) {
            Debug.LogError ("No static meshes found in selection!");
            return;
        }
        GameObject combinedMesh = new GameObject ();
        MeshFilter combinedMeshFilter = combinedMesh.AddComponent<MeshFilter> ();
        MeshRenderer combinedMeshRenderer = combinedMesh.AddComponent<MeshRenderer> ();
        MeshCollider combinedMeshCollider = combinedMesh.AddComponent<MeshCollider> ();
        combinedMeshFilter.sharedMesh = new Mesh ();
        combinedMeshFilter.sharedMesh.CombineMeshes (combine.ToArray(),false);
        combinedMeshRenderer.sharedMaterials = matarray.ToArray ();
        MeshUtility.Optimize (combinedMeshFilter.sharedMesh);
        combinedMeshCollider.sharedMesh = combinedMeshFilter.sharedMesh;
    }
}