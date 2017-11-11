using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : GunBase {
    public Rocket rocket;
    public Transform gunBarrelFront;
    public Transform gunBarrelBack;
    override public void Update() {
        base.Update ();
        if (!equipped) {
            return;
        }
        transform.rotation = view.rotation;
        if ( reloading ) {
            float progress = 1f - busy / reloadDelay;
            if (progress < 0.5f) {
                transform.rotation = Quaternion.Lerp (Quaternion.LookRotation (view.forward), Quaternion.LookRotation (view.right), progress / 0.5f);
            } else {
                transform.rotation = Quaternion.Lerp (Quaternion.LookRotation (view.right), Quaternion.LookRotation (view.forward), (progress-0.5f)/0.5f);
            }
        }
    }
    public override void OnPrimaryFire()
    {
        RaycastHit hit;

        Vector3 hitpos = view.position + view.forward * 1000f;
        // We ignore player collisions.
        if (Physics.Raycast (view.position, view.forward, out hit, 1000f, Helper.GetHitScanLayerMask())) {
            hitpos = hit.point;
        }
        Rocket r = Instantiate (rocket, gunBarrelFront.position, Quaternion.LookRotation (hitpos-gunBarrelFront.position));
        r.owner = player.gameObject;
        r.transform.Rotate(childView.localRotation.eulerAngles);
        r.inheritedVel = player.velocity+player.groundVelocity;

        Camera.main.GetComponent<SmartCamera>().AddShake(.4f);
        Camera.main.GetComponent<SmartCamera>().AddRecoil(3f);
    }
    public override void OnSecondaryFire() {
        RaycastHit hit;
        Vector3 hitpos = view.position - view.forward * 1000f;
        // We ignore player collisions.
        if (Physics.Raycast (view.position, -view.forward, out hit, 1000f, Helper.GetHitScanLayerMask())) {
            hitpos = hit.point;
        }
        Rocket r = Instantiate (rocket, gunBarrelBack.position, Quaternion.LookRotation (hitpos-gunBarrelBack.position));
        r.owner = player.gameObject;
        r.inheritedVel = player.velocity+player.groundVelocity;
    }
}
