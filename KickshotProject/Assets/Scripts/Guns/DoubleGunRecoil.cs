using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleGunRecoil : GunBase
{

    public float strength;
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

    void Start()
    {
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
        Player.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.RightUpperArm).localScale = new Vector3(0, 0, 0);
        Player.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.LeftUpperArm).localScale = new Vector3(0, 0, 0);
    }
    override public void OnUnequip(GameObject Player)
    {
        base.OnUnequip(Player);
        hitSomething = false;
        player.maxSpeed = saveMaxAirSpeed;
        Player.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.RightUpperArm).localScale = new Vector3(1, 1, 1);
        Player.GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.LeftUpperArm).localScale = new Vector3(1, 1, 1);
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
    }
    public override void OnSecondaryFireRelease()
    {
        player.maxSpeed = saveMaxAirSpeed;
        hitSomething = false;
    }
    public override void OnSecondaryFire()
    {
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
        
    }

    public override void OnPrimaryFire()
    {
        if (player.wallRunning)
        {
            float mag = Vector3.ProjectOnPlane(player.velocity, Vector3.up).magnitude;
            Vector3 newdir = Vector3.ProjectOnPlane(view.forward, Vector3.up).normalized;

            player.velocity = newdir * mag + new Vector3(0, player.velocity.y, 0);
        }
        player.velocity += view.forward * strength + view.up * strength / 2f;
        shotSound.Play();
    }
}
