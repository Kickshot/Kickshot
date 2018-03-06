using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTrigger : MonoBehaviour {
    private GameObject levelComplete;
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            if((levelComplete = GameObject.Find("LevelCompleteScreenManager")) != null)
            {
                levelComplete.GetComponent<LevelCompleteScreenManager>().DisplayLevelCompleteMenu();
            }

    }
}
