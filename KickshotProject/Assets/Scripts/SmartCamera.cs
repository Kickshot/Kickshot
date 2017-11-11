using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartCamera : MonoBehaviour {

    public Transform targetTransform { get; private set; }
    public float ShakeIntensity = 0.25f;
    public float DecreaseFactor = 1f;
    public float MaxShake = 1f;
    public float MaxRecoil = 20f;
    public float RecoilSpeed = 10f;
    public float RecoilDecayFactor = 1f;
    public bool WallRunning = false;
    public float WallRunAngle = 25f;
    private float wallRunAmount = 0;

    

    GameObject parentGO;

    float shake;
    float recoil;
    // Use this for initialization
	void Start ()
    {
        parentGO = new GameObject("Camera Helper");

        targetTransform = parentGO.transform;
        targetTransform.transform.SetPositionAndRotation(transform.position, transform.rotation);

        parentGO.transform.parent = this.transform.parent;
        this.transform.parent = parentGO.transform;

        GetComponentInParent<MouseLook>().view = targetTransform;

        shake = 0f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (WallRunning == false)
            wallRunAmount = 0f;

        HandleShake();
        HandleRecoil();
        
        parentGO.transform.SetPositionAndRotation(targetTransform.position, targetTransform.rotation);
	}

    public void AddShake(float amount)
    {
        shake += amount;
    }

    public void AddRecoil(float amount)
    {
        recoil += amount;
    }

    void HandleShake()
    {
        shake = Mathf.Clamp(shake, 0, MaxShake);

        if (shake > 0)
        {
            transform.localPosition = Random.insideUnitCircle * ShakeIntensity * shake;
            shake -= Time.deltaTime * DecreaseFactor;
        }
    }

    void HandleRecoil()
    {
        if (recoil > 0f)
        {
            
            Quaternion Recoil = Quaternion.Euler(-recoil, 0f, wallRunAmount);
            if(recoil > MaxRecoil)
            {
                Recoil = Quaternion.Euler(-MaxRecoil, 0f, wallRunAmount);
                recoil = MaxRecoil;
            }
            
            // Dampen towards the target rotation
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Recoil, Time.deltaTime * RecoilSpeed);
            recoil -= Time.deltaTime * RecoilDecayFactor;
        }
        else
        {
            recoil = 0f;
            if (!WallRunning)
            {
                // Dampen towards the target rotation
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime * RecoilSpeed / 2);
            }
            else
            {
                transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0f, 0f, wallRunAmount), Time.deltaTime * RecoilSpeed / 2);
            }
        }
    }

    public void AddWallVector(Vector3 WallNormal)
    {
        float leftDot = Vector3.Dot(WallNormal, -transform.right);
        float rightDot = Vector3.Dot(WallNormal, transform.right);

        if (leftDot < rightDot)
            wallRunAmount = -WallRunAngle;
        else
            wallRunAmount = WallRunAngle;
    }

    public void UpdateWallRunning(bool bWallRunning)
    {
        WallRunning = bWallRunning;
    }
}
