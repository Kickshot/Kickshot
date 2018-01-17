using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ChildLock : MonoBehaviour {

    private Vector3 pos;

    private void Start()
    {
        pos = transform.position;
    }

    private void Awake()
    {
        pos = transform.position;
    }

    private void LateUpdate()
    {
        transform.position = pos;
    }
}
