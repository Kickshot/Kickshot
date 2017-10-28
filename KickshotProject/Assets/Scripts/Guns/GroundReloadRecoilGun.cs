using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundReloadRecoilGun : GunBase {
    private AudioSource blam;
    public float strength;
    void Start() {
        blam = GetComponent<AudioSource> ();
    }
    override public void Update() {
        base.Update ();
        if (!equipped) {
            return;
        }
        transform.rotation = view.rotation;

        // Super hacky workaround for testing purposes
        if (player.groundEntity != null && !Input.GetButtonDown("Fire1") && !Input.GetButtonDown("Fire2"))
            ammo = magSize;

    }
    public override void OnPrimaryFire() {
        blam.Play ();
        player.velocity -= view.forward * strength;
    }
    public override void OnSecondaryFire()
    {
        blam.Play();
        player.velocity += view.forward * strength;
    }
}
