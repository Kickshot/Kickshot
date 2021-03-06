﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemSwitch : MonoBehaviour {

    public ParticleSystem system;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            system.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            system.Stop();
        }
    }
}
