using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartCamera : MonoBehaviour {

    public Transform targetTransform { get; private set; }

    GameObject parentGO;
	
    // Use this for initialization
	void Start ()
    {
        parentGO = new GameObject("Camera Helper");

        targetTransform = parentGO.transform;
        targetTransform.transform.SetPositionAndRotation(transform.position, transform.rotation);

        parentGO.transform.parent = this.transform.parent;
        this.transform.parent = parentGO.transform;

        GameObject.Find("SourcePlayer").GetComponent<MouseLook>().view = targetTransform;
	}
	
	// Update is called once per frame
	void Update ()
    {
        parentGO.transform.SetPositionAndRotation(targetTransform.position, targetTransform.rotation);
        print(targetTransform.eulerAngles);
	}
}
