﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleGunConcept : GunBase
{

    public Transform gunBarrelFront;
    private LineRenderer linerender;
    private bool hitSomething = false;
    private Transform hitPosition;
    private float hitDist;
    private float fade = 0f;
    public float fadeTime = 1f;
    public float range = 12f;
    private Vector3 missStart;
    private Vector3 missEnd;
    private AudioSource shotSound;
    private float saveMaxAirSpeed;
	private float exhaust = 1f;
	private float exhaustBusy = 0f;
	public float exhaustBusyTime;

    public Rocket rocket;
    public Animator rocketLauncher;

    void Start()
    {
		exhaust = 1f;
		exhaustBusy = 0f;
        // Copy a transform for use.
        hitPosition = Transform.Instantiate(gunBarrelFront);
        linerender = GetComponent<LineRenderer>();
        linerender.enabled = false;
        shotSound = GetComponent<AudioSource>();
        //saveMaxAirSpeed = player.maxSpeed;
    }
    override public void OnEquip(GameObject Player)
    {
        base.OnEquip(Player);
        saveMaxAirSpeed = player.maxSpeed;
    }
    override public void OnUnequip(GameObject Player)
    {
        base.OnUnequip(Player);
        hitSomething = false;
        player.maxSpeed = saveMaxAirSpeed;
    }
	override public void OnReload() {
		base.OnReload();
		player.velocity += view.forward * exhaust;
		exhaust = 1f;
		exhaustBusy = exhaustBusyTime;
	}
    override public void Update()
    {
        base.Update();
		if (exhaustBusy > 0f) {
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
            dir = Vector3.Normalize(hitPosition.position - view.position);
            if (Vector3.Distance(hitPosition.position, view.position) > hitDist)
            {
                player.Accelerate(dir, 10f, 100f);
            }
            player.Accelerate(dir, 1f / Time.deltaTime, -Vector3.Dot(player.velocity, dir));
            linerender.SetPosition(0, gunBarrelFront.position);
            linerender.SetPosition(1, hitPosition.position);
            fade = fadeTime;
            missStart = gunBarrelFront.position;
            missEnd = hitPosition.position;
        }
        else
        {
            player.maxSpeed = saveMaxAirSpeed;
            if (fade > 0)
            {
                linerender.SetPosition(0, missStart);
                linerender.SetPosition(1, missEnd);
                fade -= Time.deltaTime;
            }
            else
            {
                linerender.enabled = false;
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
    public override void OnSecondaryFireRelease()
    {
        player.maxSpeed = saveMaxAirSpeed;
        hitSomething = false;
    }
    public override void OnSecondaryFire()
    {
		if (exhaustBusy > 0f) { 
			return;
		}
        RaycastHit hit;
        // We ignore player collisions.
        if (Physics.Raycast(view.position, view.forward, out hit, range, ~(1 << LayerMask.NameToLayer("Player"))))
        {
            hitPosition.SetParent(hit.collider.transform);
            hitPosition.position = hit.point;
            hitSomething = true;
            hitDist = hit.distance;
            linerender.SetPosition(0, gunBarrelFront.position);
            linerender.SetPosition(1, hit.point);
            player.maxSpeed = 1000f;
        }
        else
        {
            hitSomething = false;
            fade = fadeTime;
            missStart = gunBarrelFront.position;
            missEnd = view.position + view.forward * range;
            linerender.SetPosition(0, missStart);
            linerender.SetPosition(1, missEnd);
        }
        linerender.enabled = true;
        shotSound.Play();
    }

    public override void OnPrimaryFire()
    {
		if (exhaustBusy > 0f) { 
			return;
		}
		exhaust += 1f;
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