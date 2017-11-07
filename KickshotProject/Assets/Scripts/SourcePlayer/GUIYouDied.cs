using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIYouDied : MonoBehaviour {
    public Transform Camera;
    public GameObject Player;
    void OnGUI () {
        GUIStyle style = GUIStyle.none;
        style.normal.textColor = Color.black;
        style.fontSize = 24;
        GUI.Label (new Rect (Screen.width/2f+1f-125f, Screen.height/2f-50f, 250, 100), "You died!", style);
        style.normal.textColor = Color.red;
        GUI.Label (new Rect (Screen.width/2f-125f, Screen.height/2f-50f, 250, 100), "You died!", style);
    }
    void Update() {
        Vector3 targetPos = transform.position + new Vector3 (Mathf.Sin (Time.time), 1f, Mathf.Cos (Time.time)) * 10f;
        Quaternion targetRot = Quaternion.LookRotation (Player.transform.position - Camera.position, Vector3.up);
        Camera.position += (targetPos - Camera.position) * Time.deltaTime;
        Camera.rotation = Quaternion.Lerp (Camera.rotation, targetRot,0.5f);
        if (Input.GetButtonDown ("Fire1")) {
            GameManager.instance.Died ();
            Destroy (gameObject);
        }
    }
}
