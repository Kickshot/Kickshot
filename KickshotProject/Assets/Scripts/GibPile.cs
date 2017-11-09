﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GibPile : MonoBehaviour {
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
    public void FitToPlayer( GameObject obj ) {
        SourcePlayer player = obj.GetComponent<SourcePlayer> ();
        if (player == null) {
            Debug.LogError ("Tried to fit gibs to unknown object!");
        }
        Animator body = obj.GetComponentInChildren<Animator> ();
        head.transform.position = body.GetBoneTransform (HumanBodyBones.Head).position;
        head.transform.rotation = body.GetBoneTransform (HumanBodyBones.Head).rotation;
        head.GetComponent<Rigidbody> ().velocity = player.velocity;

        chest.transform.position = body.GetBoneTransform (HumanBodyBones.Chest).position;
        chest.transform.rotation = body.GetBoneTransform (HumanBodyBones.Chest).rotation;
        chest.GetComponent<Rigidbody> ().velocity = player.velocity;

        abs.transform.position = body.GetBoneTransform (HumanBodyBones.Hips).position;
        abs.transform.rotation = body.GetBoneTransform (HumanBodyBones.Hips).rotation;
        abs.GetComponent<Rigidbody> ().velocity = player.velocity;

        lleg.transform.position = body.GetBoneTransform (HumanBodyBones.LeftUpperLeg).position;
        lleg.transform.rotation = body.GetBoneTransform (HumanBodyBones.LeftUpperLeg).rotation;
        lleg.GetComponent<Rigidbody> ().velocity = player.velocity;

        rleg.transform.position = body.GetBoneTransform (HumanBodyBones.RightUpperLeg).position;
        rleg.transform.rotation = body.GetBoneTransform (HumanBodyBones.RightUpperLeg).rotation;
        rleg.GetComponent<Rigidbody> ().velocity = player.velocity;

        lfoot.transform.position = body.GetBoneTransform (HumanBodyBones.LeftFoot).position;
        lfoot.transform.rotation = body.GetBoneTransform (HumanBodyBones.LeftFoot).rotation;
        lfoot.GetComponent<Rigidbody> ().velocity = player.velocity;

        rfoot.transform.position = body.GetBoneTransform (HumanBodyBones.RightFoot).position;
        rfoot.transform.rotation = body.GetBoneTransform (HumanBodyBones.RightFoot).rotation;
        rfoot.GetComponent<Rigidbody> ().velocity = player.velocity;

        rfist.transform.position = body.GetBoneTransform (HumanBodyBones.RightHand).position;
        rfist.transform.rotation = body.GetBoneTransform (HumanBodyBones.RightHand).rotation;
        rfist.GetComponent<Rigidbody> ().velocity = player.velocity;

        lfist.transform.position = body.GetBoneTransform (HumanBodyBones.LeftHand).position;
        lfist.transform.rotation = body.GetBoneTransform (HumanBodyBones.LeftHand).rotation;
        lfist.GetComponent<Rigidbody> ().velocity = player.velocity;

        larm.transform.position = body.GetBoneTransform (HumanBodyBones.LeftUpperArm).position;
        larm.transform.rotation = body.GetBoneTransform (HumanBodyBones.LeftUpperArm).rotation;
        larm.GetComponent<Rigidbody> ().velocity = player.velocity;

        rarm.transform.position = body.GetBoneTransform (HumanBodyBones.RightUpperArm).position;
        rarm.transform.rotation = body.GetBoneTransform (HumanBodyBones.RightUpperArm).rotation;
        rarm.GetComponent<Rigidbody> ().velocity = player.velocity;

        rforearm.transform.position = body.GetBoneTransform (HumanBodyBones.RightLowerArm).position;
        rforearm.transform.rotation = body.GetBoneTransform (HumanBodyBones.RightLowerArm).rotation;
        rforearm.GetComponent<Rigidbody> ().velocity = player.velocity;

        lforearm.transform.position = body.GetBoneTransform (HumanBodyBones.LeftLowerArm).position;
        lforearm.transform.rotation = body.GetBoneTransform (HumanBodyBones.LeftLowerArm).rotation;
        lforearm.GetComponent<Rigidbody> ().velocity = player.velocity;
    }
}