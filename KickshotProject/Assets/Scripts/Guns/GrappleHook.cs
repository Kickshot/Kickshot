using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleHook : GunBase {
    public Transform gunBarrelFront;
    private LineRenderer linerender;
    private bool hitSomething = false;
    private Transform hitPosition;
    private float hitDist;
    private float fade = 0f;
    public float fadeTime = 1f;
    public float range = 12f;
    public float GrappleStrength = 25f;
    private Vector3 missStart;
    private Vector3 missEnd;
    private AudioSource shotSound;
    private float saveMaxAirSpeed;
	private Vector3 grappleVelocity;
	private Vector3 targetPosition;
	private float firstLockedDistance; //D
	private bool firstShot;
    

    void Start()
    {
        firstShot = true;
        gunName = "Grapple Hook";
        // Copy a transform for use.
        hitPosition = Transform.Instantiate(gunBarrelFront);
        linerender = GetComponent<LineRenderer>();
        linerender.enabled = false;
        shotSound = GetComponent<AudioSource>();
		firstLockedDistance = 0;
        //saveMaxAirSpeed = player.maxSpeed;
    }
    override public void OnEquip(GameObject Player)
    {
        base.OnEquip(Player);
		Transform shoulderBone = Player.GetComponentInChildren<Animator> ().GetBoneTransform (HumanBodyBones.RightShoulder);
        saveMaxAirSpeed = player.maxSpeed;
		transform.position = shoulderBone.position + shoulderBone.right * 0.4f - shoulderBone.up * 0.4f;
    }
    override public void OnUnequip(GameObject Player)
    {
        base.OnUnequip(Player);
        player.maxSpeed = saveMaxAirSpeed;
    }
    override public void Update()
    {
        base.Update();
        if (!equipped)
        {
            return;
        }
        transform.rotation = view.rotation;

        if (hitSomething)
        { 
            // Keep us busy so we don't reload during grappling.
            busy = 1f;
			targetPosition = (player.transform.position - hitPosition.position).normalized * firstLockedDistance;
			Vector3 springAccelerate = (targetPosition - (player.transform.position - hitPosition.position)) * GrappleStrength * Time.deltaTime;

			//print (springAccelerate/Time.deltaTime);
			print(Vector3.Dot (springAccelerate, player.transform.position - hitPosition.position));
			if (Vector3.Dot (springAccelerate, player.transform.position - hitPosition.position) < 0) {
				player.velocity += springAccelerate;
			}   
            //print((desiredPosition - player.transform.position));
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
    }
    public override void OnPrimaryFireRelease()
    {
        player.maxSpeed = saveMaxAirSpeed;
        hitSomething = false;
        firstShot = true;
    }
    public override void OnPrimaryFire()
    {
        RaycastHit hit;
        // We ignore player collisions.
        if (Physics.Raycast(view.position, view.forward, out hit, range, ~(1 << LayerMask.NameToLayer("Player"))))
        {
            hitPosition.SetParent(hit.collider.transform);
            hitPosition.position = hit.point;
            hitSomething = true;
            hitDist = hit.distance;
			print (hitDist);
            linerender.SetPosition(0, gunBarrelFront.position);
            linerender.SetPosition(1, hit.point);
            player.maxSpeed = 1000f;
            
			if (firstShot)
			{
				firstLockedDistance = hit.distance;
				targetPosition = (player.transform.position - hitPosition.position).normalized * firstLockedDistance;
				//print (hit.distance);
				//print (targetPosition.magnitude);
				firstShot = false;
			}
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
}
