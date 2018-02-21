using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickExplosion : MonoBehaviour {

    public GameObject explosion;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Click");
            Ray dir = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(dir, out hit))
            {
                GameObject obj = Instantiate(explosion, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }
}
