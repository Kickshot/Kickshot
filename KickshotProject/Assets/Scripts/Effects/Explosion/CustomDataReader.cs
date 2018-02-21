using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomDataReader : MonoBehaviour {

    private Material sharedMat;
    private ParticleSystem sys;
    private ParticleSystem.Particle[] sysParticles;

    private void Start()
    {
        sys = GetComponent<ParticleSystem>();
        sysParticles = new ParticleSystem.Particle[sys.main.maxParticles];

        ParticleSystemRenderer rend = GetComponent<ParticleSystemRenderer>();
        Debug.Assert(rend != null);
        sharedMat = rend.material;
    }

    private void LateUpdate () {
        int numAlive = sys.GetParticles(sysParticles);
        for (int i = 0; i < numAlive; i++)
        {
            float dataPoint = sys.customData.GetVector(ParticleSystemCustomData.Custom1, 0).Evaluate((sysParticles[i].startLifetime-sysParticles[i].remainingLifetime)/sysParticles[i].startLifetime);
            sharedMat.SetFloat("_Magnitude", dataPoint);
        }
    }
}
