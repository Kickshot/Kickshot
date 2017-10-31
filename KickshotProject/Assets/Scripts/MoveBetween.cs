using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves a GameObject between two points over a period of CycleLength seconds
public class MoveBetween : MonoBehaviour {
    
    public Transform Target1;
    public Transform Target2;
    public float CycleLength = 3;
    
    float cycleTime = 0;
    
	void Update () {
        // Update cycle time
        cycleTime += Time.deltaTime;
        if (cycleTime >= CycleLength)
            cycleTime -= CycleLength;

        // Move GameObject
        if(cycleTime < CycleLength / 2)
            transform.position = Vector3.Lerp(Target1.position, Target2.position, cycleTime / (CycleLength / 2));
        else
            transform.position = Vector3.Lerp(Target2.position, Target1.position, (cycleTime - CycleLength / 2) / (CycleLength / 2));

    }
}
