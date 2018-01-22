using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSpawnPos : MonoBehaviour {

    public GameObject SpawnPos;
    public List<GameObject> spawnList;

    private void Start()
    {
        if (SpawnPos == null) return;
        foreach (GameObject obj in spawnList) {
            obj.transform.position = SpawnPos.transform.position;
        }
    }
}
