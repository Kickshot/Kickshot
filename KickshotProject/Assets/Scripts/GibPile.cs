using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GibPile : MonoBehaviour {
    public float noise = 0.2f;
    public float explodeForce = 0.8f;
    public float upForce = 2f;
    public GameObject head;
    public GameObject chest;
    public GameObject abs;
    public GameObject lleg;
    public GameObject rleg;
    public GameObject lfoot;
    public GameObject rfoot;
    public GameObject lforearm;
    public GameObject larm;
    public GameObject rforearm;
    public GameObject rarm;
    public GameObject lfist;
    public GameObject rfist;
    private void SetupPart( GameObject thing, Vector3 velocity, Animator body, HumanBodyBones bone ) {
        thing.transform.position = body.GetBoneTransform (bone).position;
        thing.transform.rotation = body.GetBoneTransform (bone).rotation;
        Vector3 dir = thing.transform.position - transform.position;
        thing.GetComponent<Rigidbody> ().velocity = velocity + new Vector3(Random.Range(-noise,noise),Random.Range(-noise,noise),Random.Range(-noise,noise)) + Vector3.Normalize(dir)*explodeForce/dir.magnitude + new Vector3(0,upForce,0);
        thing.GetComponent<Rigidbody> ().AddTorque(new Vector3(Random.Range(-noise,noise),Random.Range(-noise,noise),Random.Range(-noise,noise))*5f);
    }
    public void FitToPlayer( GameObject obj, Vector3 velocity ) {
        Animator body = obj.GetComponentInChildren<Animator> ();
        SetupPart (head, velocity, body, HumanBodyBones.Head);
        SetupPart (chest, velocity, body, HumanBodyBones.Chest);
        SetupPart (abs, velocity, body, HumanBodyBones.Hips);
        SetupPart (rleg, velocity, body, HumanBodyBones.RightUpperLeg);
        SetupPart (lfoot, velocity, body, HumanBodyBones.LeftFoot);
        SetupPart (rfoot, velocity, body, HumanBodyBones.RightFoot);
        SetupPart (rfist, velocity, body, HumanBodyBones.RightHand);
        SetupPart (lfist, velocity, body, HumanBodyBones.LeftHand);
        SetupPart (larm, velocity, body, HumanBodyBones.LeftUpperArm);
        SetupPart (rarm, velocity, body, HumanBodyBones.RightUpperArm);
        SetupPart (rforearm, velocity, body, HumanBodyBones.RightLowerArm);
        SetupPart (lforearm, velocity, body, HumanBodyBones.LeftLowerArm);
    }
}
