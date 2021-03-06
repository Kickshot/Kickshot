﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DoubleGun : GunBase
{
	public Transform gunBarrelFront;
    public GameObject grappleHitCloud;
    private bool hitSomething = false;
	private Transform hitPosition;
	private float hitDist;
	public float range = 12f;
	private Vector3 missStart;
	private Vector3 missEnd;
    public List<AudioClip> grappleHit;
    private float saveMaxAirSpeed;
	private float exhaust = 1f;
	internal float exhaustBusy = 0f;
	
	private List<RopeSim> ropes;
	public AudioSource airBurst;
    public AudioSource sound;
	public GameObject ropePrefab;
	private RopeSim rope;

	public Rocket rocket;
	public Animator rocketLauncher;

    private Crosshair crosshairImage;

    [Header("Grapple Hook")]
    public float GrappleUsePercentage = 50f;
    public float UngroundedRecoveryPercentage = 10f;
    public float GroundedRecoveryPercentage = 50f;
    internal float energy;
    public bool checkTag;
    public AudioClip grappleHookClip;
    Transform grappleShoulderBone;

    [Header("Rocket Launcher")]
    public AudioSource OverheatClip;
    public float RocketUsePercentage = 35f;
    public float DecayPercentage = 10f;
    public float MaxVentVelocity = 50f;
    public float VentPenalty = 1f;
    public float OverheatPenalty = 2f;
    public float NoHeatVentPercentage = 10f; //Percentage of MaxVentVelocity attained when heat = 0.
    internal float heat;
    internal bool isOverheating;

    void Start()
	{
        crosshairImage = GameObject.Find("Crosshair").GetComponent<Crosshair>();
        if (crosshairImage == null)
            Debug.LogError("Could Not Find Crosshair in Gun.");

       // CrosshairMat = GameObject.Find("Crosshair").GetComponent<Material>();
        //CrosshairMat.color = Color.red;
        
        energy = 100f;
        heat = 0f;
		ropes = new List<RopeSim> ();
		exhaust = 1f;
		exhaustBusy = 0f;
		// Copy a transform for use.
		hitPosition = Transform.Instantiate(gunBarrelFront);
        //saveMaxAirSpeed = player.maxSpeed;
    }
	override public void OnEquip(GameObject Player)
	{
		base.OnEquip(Player);
		saveMaxAirSpeed = player.maxSpeed;
        grappleShoulderBone = player.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.LeftUpperArm);
    }
	override public void OnUnequip(GameObject Player)
	{
		base.OnUnequip(Player);
		hitSomething = false;
		player.maxSpeed = saveMaxAirSpeed;
	}
	override public void OnReload()
    {
        if (exhaustBusy > 0f)
            return;

        base.OnReload();
		player.velocity += view.forward * ( (heat == 0f ? NoHeatVentPercentage : heat)/100f) * MaxVentVelocity;
        heat = 0f;
		exhaust = 1f;
        rocketLauncher.SetTrigger("Boost");
		airBurst.Play ();
		exhaustBusy = VentPenalty;
		Camera.main.GetComponent<SmartCamera>().AddShake(.4f);
		Camera.main.GetComponent<SmartCamera>().AddRecoil(3f);
	}
	override public void Update()
	{
        //print("energy = " + energy + " heat = " + heat);
        //Grapple Hook energy system.
        if (player == null)
            return;

	rocketLauncher.SetFloat("Vel", Mathf.Clamp(player.velocity.magnitude/5,1,10));
	rocketLauncher.SetFloat("Speed", Mathf.Clamp(player.velocity.magnitude/10,0,1));
	rocketLauncher.SetBool("OnGround", player.controller.isGrounded);
        if(player.controller.isGrounded) {
            energy += GroundedRecoveryPercentage * Time.deltaTime;
        } else {
            energy += UngroundedRecoveryPercentage * Time.deltaTime;
	}

        energy = Mathf.Clamp(energy, 0f, 100f);

        //Rocket Laucher heat system.
        heat -= DecayPercentage * Time.deltaTime;
        heat = Mathf.Clamp(heat, 0f , float.MaxValue);
        if(heat > 100f)
        {
            Overheat();
        }

        base.Update();
		if (ropes.Count > 10) {
			RopeSim temp = ropes [0];
			Destroy (temp.gameObject);
			ropes.Remove (temp);
		}
		if (exhaustBusy > 0f)
        {
			exhaustBusy -= Time.deltaTime;
		}
		if (!equipped)
		{
			return;
		}
        

		transform.rotation = view.rotation;
		if (hitSomething)
		{
            
			// Keep us busy so we don't reload during grappling.
			busySecondary = 0.8f;
			//player.transform.position = hitPosition.position - player.view.forward * hitDist;
			//Vector3 desiredPosition = hitPosition.position - view.forward * hitDist;
			//player.velocity = (desiredPosition - player.transform.position) / Time.deltaTime;
			Vector3 dir;
			dir = Vector3.Normalize (hitPosition.position - view.position);
			//if (Vector3.Distance (hitPosition.position, view.position) > hitDist) {
				//player.Accelerate (dir, 5f, 100f);
			//}
			if (player.velocity.magnitude != 0f) {
				rope.start.position = grappleShoulderBone.position;
				rope.end.position = hitPosition.position;
				if (Vector3.Distance (hitPosition.position, view.position) > hitDist) {
					player.Accelerate (dir, 1f / Time.deltaTime, -Vector3.Dot (player.velocity, dir));
				}
				missStart = grappleShoulderBone.position;
				missEnd = hitPosition.position;
			}
		}
		else
		{
			if (rope != null) {
				rope.start.position = grappleShoulderBone.position;
			}
			player.maxSpeed = saveMaxAirSpeed;
		}



        if (hitSomething)
        {
            crosshairImage.switchImage(2);

            if (Physics.Raycast(view.position, view.forward, range, ~(1 << LayerMask.NameToLayer("Player"))))
            {
                crosshairImage.switchImage(3);
            }
        }
        else
        {

            if (Physics.Raycast(view.position, view.forward, range, ~(1 << LayerMask.NameToLayer("Player"))))
            {
                crosshairImage.switchImage(1);
            }
            else
            {
                crosshairImage.switchImage(0);
            }
        }

        // Rocket Update
        transform.rotation = view.rotation;
		if (reloading)
		{
			float progress = 1f - busy / reloadDelay;
			if (progress < 0.5f)
			{
				transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(view.forward), Quaternion.LookRotation(view.right), progress / 0.5f);
			}
			else
			{
				transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(view.right), Quaternion.LookRotation(view.forward), (progress - 0.5f) / 0.5f);
			}
		}
	}

    public void Overheat()
    {
        isOverheating = true;
        OverheatClip.Play();
        exhaustBusy = OverheatPenalty;
        heat = 0f;

        StartCoroutine(ReleaseOverheat());
    }

    IEnumerator ReleaseOverheat()
    {
        yield return new WaitForSeconds(OverheatPenalty);
        isOverheating = false;
    }

	public override void OnSecondaryFireRelease()
	{
        if (hitSomething)
            busySecondary = 0;

        if (rope != null) {
            rope.staticStart = false;
            rope.sticky = true;
            rope = null;
        }
		player.maxSpeed = saveMaxAirSpeed;
		hitSomething = false;
	}
	public override void OnSecondaryFire()
	{
		if (exhaustBusy > 0f) { 
			return;
		}

        if(energy - GrappleUsePercentage < 0)
        {
            return;
        }
        energy -= GrappleUsePercentage;
        sound.clip = grappleHookClip;
        sound.Play();
        //AudioSource.PlayClipAtPoint(grappleHookClip, transform.position);
		RaycastHit hit;
		GameObject ropeRoot = Instantiate (ropePrefab,Vector3.zero,Quaternion.identity);
		rope = ropeRoot.GetComponentInChildren<RopeSim> ();
		ropes.Add (rope);
		rope.sticky = false;

        missStart = gunBarrelFront.position;
        missEnd = view.position + view.forward * range;
        // We ignore player collisions.
        if (Physics.Raycast(view.position, view.forward, out hit, range, ~(1 << LayerMask.NameToLayer("Player"))))
        {
            if (hit.collider && (!checkTag || hit.collider.CompareTag("Grappleable")))
            {
                hitPosition.SetParent(hit.collider.transform);
                hitPosition.position = hit.point;
                hitSomething = true;
                hitDist = Mathf.Max(hit.distance, 1f);
                rope.start.position = gunBarrelFront.position;
                rope.end.position = hit.point;
                rope.Regenerate();
                player.maxSpeed = 1000f;
                sound.clip = grappleHit[Random.Range(0, grappleHit.Count)];
                sound.Play();
                //AudioSource.PlayClipAtPoint(grappleHit[Random.Range(0, grappleHit.Count)], hit.point);
                Destroy(Instantiate(grappleHitCloud, hit.point, Quaternion.LookRotation(hit.normal)), 1f);
                return;
            } else {
                missEnd = hit.point;
            }
        }
		hitSomething = false;
		rope.start.position = missStart;
		rope.end.position = missEnd;
		rope.Regenerate ();
		rope.staticEnd = false;
		
		// We ignore player collisions.
		if (Physics.Raycast(view.position, view.forward, out hit, range, ~(1 << LayerMask.NameToLayer("Player"))))
		{
			hitPosition.SetParent(hit.collider.transform);
			hitPosition.position = hit.point;
			hitSomething = true;
			hitDist = Mathf.Max(hit.distance,1f);
            
            rope.start.position = grappleShoulderBone.position;
			rope.end.position = hit.point;
			rope.Regenerate ();
			player.maxSpeed = 1000f;
            sound.clip = grappleHit[Random.Range(0, grappleHit.Count)];
            sound.Play();
            //AudioSource.PlayClipAtPoint(grappleHit[Random.Range(0,grappleHit.Count)], hit.point);
            Destroy(Instantiate(grappleHitCloud, hit.point, Quaternion.LookRotation(hit.normal) ), 1f);
		}
		else
		{
			hitSomething = false;
			missStart = gunBarrelFront.position;
			missEnd = view.position + view.forward * range;
			rope.start.position = missStart;
			rope.end.position = missEnd;
			rope.Regenerate ();
			rope.staticEnd = false;
		}
	}

	public override void OnPrimaryFire()
	{
		if (exhaustBusy > 0f)
        { 
			return;
		}
		exhaust += 1f;
		if (exhaust > 5f) {
			exhaust = 5f;
		}
        heat += RocketUsePercentage;

		RaycastHit hit;
		rocketLauncher.SetTrigger("Fire");
		Vector3 hitpos = view.position + view.forward * 1000f;
		// We ignore player collisions.
		if (Physics.Raycast(view.position, view.forward, out hit, 1000f, Helper.GetHitScanLayerMask()))
		{
			hitpos = hit.point;
		}
		Rocket r = Instantiate(rocket, gunBarrelFront.position, Quaternion.LookRotation(hitpos - gunBarrelFront.position));
		r.owner = player.gameObject;
		r.transform.Rotate(childView.localRotation.eulerAngles);
		r.inheritedVel = player.velocity + player.groundVelocity;

		Camera.main.GetComponent<SmartCamera>().AddShake(.4f);
		Camera.main.GetComponent<SmartCamera>().AddRecoil(3f);
	}

}