using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Tornado : MonoBehaviour {

    public AnimationCurve shellDensity;


    private Material tornadoShell;

    private void Awake()
    {
        MeshRenderer r = GetComponent<MeshRenderer>();
        if (r != null) {
            tornadoShell = r.sharedMaterial;
        }
    }

    private void Update()
    {
        tornadoShell.SetFloat("_NoiseCutoff", shellDensity.Evaluate(Time.time));
    }
}
