using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUICrosshair : MonoBehaviour {
    public Texture2D crosshairTexture;
    public float size = 128f;
    private Rect position;
    void Start() {
        position = new Rect((Screen.width - size)/2, (Screen.height - size)/2, size, size);
    }
    void OnGUI () {
        GUI.DrawTexture (position, crosshairTexture, ScaleMode.ScaleToFit);
    }
}
