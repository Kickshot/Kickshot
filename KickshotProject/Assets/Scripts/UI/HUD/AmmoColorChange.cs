using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoColorChange : MonoBehaviour
{
    public enum Variable
    {
        RocketLaucher , GrappleHook
    }

    public Color to;
    public Color from;
    public Variable variable;

    Image img;
    DoubleGun gun;
    // Use this for initialization
    void Start()
    {
        img = GetComponent<Image>();

        if (gun == null)
        {
            gun = FindObjectOfType<DoubleGun>();
            if (gun == null)
            {
                Debug.LogError("Failed to assign player!");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        gun = FindObjectOfType<DoubleGun>();
        if (gun == null)
            return;
        switch (variable)
        {
            case Variable.RocketLaucher:
                img.fillAmount = map(gun.heat, 0, 100, 0, 1);
                break;
            case Variable.GrappleHook:
                img.fillAmount = map(gun.energy, 0, 100, 0, 1);
                break;
        }
        img.color = Color.Lerp(to, from, img.fillAmount);
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
