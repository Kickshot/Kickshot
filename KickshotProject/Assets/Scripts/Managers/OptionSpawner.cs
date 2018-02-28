using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionSpawner : MonoBehaviour {

    public GameObject ObjManager;
	// Use this for initialization
	void Start () {
        GameObject obj = GameObject.Find("[Options_Manager]");

        if (obj == null)
        {
            obj = Instantiate(ObjManager) as GameObject;
            obj.name = "[Options_Manager]";
        }


	}
	
	// Update is called once per frame
	void Update () {
      
	}
}
