using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingSparkles : MonoBehaviour {

    public float amplitude = 0.5f;
    public float frequency = 1.0f;

    Vector3 positionOffset = new Vector3();
    Vector3 temporaryPosition = new Vector3();

    // Use this for initialization
    void Start () {
        positionOffset = transform.position;

    }

    // Update is called once per frame
    void Update () {
        temporaryPosition = positionOffset;
        temporaryPosition.y += Mathf.Sin((Time.fixedTime) / 2 * Mathf.PI * frequency) * amplitude;

        transform.position = temporaryPosition;
    }
}
