using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissiveCutoffShowoff : MonoBehaviour {

    public bool mainEmission; //_MainEmInt
    public bool altEmission; //_AltEmInt
    public bool interpolate; //_Cutoff

    private Material mat;

    private void Start()
    {
        mat = GetComponent<Renderer>().sharedMaterial;
    }

    private void Update()
    {
        if (interpolate)
            mat.SetFloat("_Cutoff", Mathf.Abs(Mathf.Sin(Time.time)));
        if (mainEmission)
            mat.SetFloat("_MainEmInt", Mathf.Abs(Mathf.Sin(Time.time)) * 10f);
        if (altEmission)
            mat.SetFloat("_AltEmInt", Mathf.Abs(Mathf.Sin(Time.time)) * 10f);
    }
}
