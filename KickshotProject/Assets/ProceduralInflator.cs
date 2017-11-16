using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralInflator : MonoBehaviour {
    public float inflateAmount = 0.012f;
    public GameObject gibs;
    private float extrudeAmount;
    private SkinnedMeshRenderer mesh;
    private Shader inflatorShader;
    private Material material;
    private Vector3 velocity;
	void Start () {
        extrudeAmount = 0;
        inflatorShader = Shader.Find ("Custom/Fatten");
        mesh = GetComponentInChildren<SkinnedMeshRenderer> ();
        material = GetComponentInChildren<Renderer> ().material;
        material.shader = inflatorShader;
	}
    public void FitToPlayer( GameObject obj, Vector3 vel) {
        if (mesh == null) {
            mesh = GetComponentInChildren<SkinnedMeshRenderer> ();
        }
        MouseLook look = obj.GetComponentInChildren<MouseLook> ();
        if (look != null) {
            MouseLook ourLook = GetComponent<MouseLook> ();
            if (ourLook) {
                ourLook.SetRotation (look.view.rotation);
            }
        }
        SkinnedMeshRenderer otherMesh = obj.GetComponentInChildren<SkinnedMeshRenderer> ();
        if (otherMesh == null || mesh.bones.Length != otherMesh.bones.Length) {
            Debug.LogError ("Tried to fit to unknown object!");
            return;
        }
        // Just copy the exact positions at that moment.
        for (int i = 0; i < mesh.bones.Length; i++) {
            mesh.bones [i].localPosition = otherMesh.bones [i].localPosition;
            mesh.bones [i].localRotation = otherMesh.bones [i].localRotation;
            //mesh.bones [i].localScale = otherMesh.bones [i].localScale;
        }
        velocity = vel;
    }
	void Update () {
        if (extrudeAmount < inflateAmount) {
            extrudeAmount += Time.deltaTime*inflateAmount*12f;
        } else {
            extrudeAmount = inflateAmount;
        }
        material.SetFloat ("_Amount", extrudeAmount);
        if (extrudeAmount == inflateAmount) {
            GameObject g = Instantiate(gibs,transform.position,transform.rotation);
            g.GetComponent<GibPile>().FitToPlayer (gameObject, velocity);
            Destroy (gameObject);
        }
	}
}
