using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour {

    public Sprite[] crosshairImages;
    private Image guiCrosshair;
	// Use this for initialization
	void Start () {
        guiCrosshair = gameObject.GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void switchImage(int index)
    {
        guiCrosshair.sprite = crosshairImages[index];
    }
}
