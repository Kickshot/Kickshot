﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(Decal) )]
public class RandomDecal : MonoBehaviour {
    public Material[] materials;
	void Start () {
		GetComponent<MeshRenderer> ().material = materials [Random.Range (0, materials.Length - 1)];
	}
}
