using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class CircleColorChange : MonoBehaviour
{
    public Color to;
    public Color from;

    Image img;
    SourcePlayer player;
	// Use this for initialization
	void Start ()
    {
        img = GetComponent<Image>();

        if (player == null)
        {
            player = FindObjectOfType<SourcePlayer>();
            if (player == null)
            {
                Debug.LogError("Failed to assign player!");
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        img.fillAmount = map(player.velocity.magnitude,0,100,0,1);
        img.color = Color.Lerp(to, from, img.fillAmount); 
	}

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
