using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBob : MonoBehaviour {

    [Range (0f, 2f)]
    public float bobDistance;
    [Range (0f, 2f)]
    public float cycleSpeed;

    private float offset;

    private void Start()
    {
        offset = Random.Range(0f, 5f);
    }

    private void Update()
    {
        transform.Translate(Vector3.down * bobDistance * Time.deltaTime * Mathf.Sin((Time.time + offset) / cycleSpeed));
    }
}
