using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper {
	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f) {
		GameObject myLine = new GameObject();
		myLine.transform.position = start;
		myLine.AddComponent<LineRenderer>();
		LineRenderer lr = myLine.GetComponent<LineRenderer>();
		lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		lr.SetColors(color, color);
		lr.SetWidth(0.1f, 0.1f);
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);
		GameObject.Destroy(myLine, duration);
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

	public static Material getMaterialFromMesh( RaycastHit hit, Mesh mesh, GameObject parent ) {
		Renderer r = parent.GetComponent<Renderer> ();
		if (r == null) {
			return null;
		}
		if (mesh.subMeshCount == 1) {
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
		return null;
	}
}
