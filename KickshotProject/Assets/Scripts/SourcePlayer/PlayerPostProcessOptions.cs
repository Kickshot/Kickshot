﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PlayerPostProcessOptions : MonoBehaviour
{
    internal PostProcessingBehaviour ppb;
    public SourcePlayer player;
    public float baseFOV;

    [Header("   FOV Options\n")]
    [Range(10f,100f)] //Speed needed to start applying FOV distortion to the player.
    public float FOV_minDistortionSpeed = 20f;
    [Range(10f,100f)]  //Speed needed to apply maximum distortion amount according to FOV_distortionAmount.
    public float FOV_maxDistortionSpeed = 100f;
    [Range(1f,20f)] // Range of distortion to apply.
    public float FOV_distortionAmount = 5f;
    [Range(0f,1f)]
    public float FOV_LerpT = 0.1f;

    // Use this for initialization
    void Start ()
    {
        ppb = Camera.main.GetComponent<PostProcessingBehaviour>();
        baseFOV = Camera.main.fieldOfView;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (player == null) {
            return;
        }
        Vector3 flatvel = new Vector3 (player.velocity.x, 0, player.velocity.z);
        if(flatvel.magnitude > FOV_minDistortionSpeed)
        {
            float targetFOV;
            if (flatvel.magnitude < FOV_maxDistortionSpeed)
                targetFOV = baseFOV + ((flatvel.magnitude - FOV_minDistortionSpeed) / FOV_maxDistortionSpeed) * FOV_distortionAmount;
            else
                targetFOV = baseFOV + FOV_distortionAmount;
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, targetFOV,FOV_LerpT);
        }
	}
}
