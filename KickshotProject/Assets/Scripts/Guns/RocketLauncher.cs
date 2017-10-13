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
			transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(-view.forward),Quaternion.LookRotation(view.forward),busy/reloadDelay);
		}
	}
	public override void OnPrimaryFire() {
		RaycastHit hit;
		Vector3 hitpos = view.forward * 1000f;
		// We ignore player collisions.
		if (Physics.Raycast (view.position, view.forward, out hit, 1000f, ~(1 << LayerMask.NameToLayer ("Player")))) {
			hitpos = hit.point;
		}
		Rocket r = Instantiate (rocket, gunBarrelFront.position, Quaternion.LookRotation (hitpos-gunBarrelFront.position));
		r.inheritedVel = player.velocity+player.groundVelocity;
	}
	public override void OnSecondaryFire() {
		RaycastHit hit;
		Vector3 hitpos = -view.forward * 1000f;
		// We ignore player collisions.
		if (Physics.Raycast (view.position, -view.forward, out hit, 1000f, ~(1 << LayerMask.NameToLayer ("Player")))) {
			hitpos = hit.point;
		}
		Rocket r = Instantiate (rocket, gunBarrelBack.position, Quaternion.LookRotation (hitpos-gunBarrelBack.position));
		r.inheritedVel = player.velocity+player.groundVelocity;
	}
}
