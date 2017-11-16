using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanArea : MonoBehaviour {
    public float FanStrength = 50;

    void OnTriggerStay(Collider other)
    {
        SourcePlayer sp = other.GetComponent<SourcePlayer>();
        if (sp != null)
        {
            sp.velocity += (FanStrength * Time.deltaTime) * transform.up;
        }
    }
}
