using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    [Range(1f, 100f)]
    public float RandomRotationUpperBound;

    Vector3 randRot;
	// Use this for initialization
	void Start ()
    {
        randRot = Random.insideUnitSphere;
	}
	
	// Update is called once per frame
	void Update ()
    {
        print(randRot);
        transform.Rotate(randRot * RandomRotationUpperBound * Time.deltaTime);
	}
}
