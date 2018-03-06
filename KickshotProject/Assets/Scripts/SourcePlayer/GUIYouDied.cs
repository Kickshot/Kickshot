using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIYouDied : MonoBehaviour {
    public Transform Camera;
    public GameObject Player;

    void Update() {
        if (Camera == null || Player == null) {
            return;
        }
        Vector3 targetPos = Player.transform.position + new Vector3 (Mathf.Sin (Time.time), 1f, Mathf.Cos (Time.time)) * 4f;
        Quaternion targetRot = Quaternion.LookRotation (Player.transform.position - Camera.position, Vector3.up);
        Camera.position += (targetPos - Camera.position) * Time.deltaTime;
        Camera.rotation = Quaternion.Lerp (Camera.rotation, targetRot,0.5f);
    }
}
