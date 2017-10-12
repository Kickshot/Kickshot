using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TractorGrapple: GunBase {
	public Transform gunBarrelFront;
	private LineRenderer linerender;
	private SourcePlayer player;
	private bool hitSomething = false;
	private Transform hitPosition;
	private float hitDist;
	private Vector3 lastPosition;
	private float fade = 0f;
	public float range = 13f;
	private Vector3 missStart;
	private Vector3 missEnd;
	void Start() {
		hitPosition = Transform.Instantiate (gunBarrelFront);
		gunName = "Tractor Grapple";
		linerender = GetComponent<LineRenderer> ();
	}
	override public void Update() {
		base.Update ();
		if (!equipped) {
			return;
		}
		transform.rotation = player.view.rotation;
		if (hitSomething) {
			// Keep us busy so we don't reload during grappling.
			busy = 1f;
			//player.transform.position = hitPosition.position - player.view.forward * hitDist;
			Vector3 desiredPosition = hitPosition.position - player.view.forward * hitDist;
			player.velocity = (desiredPosition - player.transform.position)/Time.deltaTime;
			lastPosition = player.transform.position;
			linerender.SetPosition (0, gunBarrelFront.position);
			linerender.SetPosition (1, hitPosition.position);
		}
		if (fade > 0) {
			linerender.SetPosition (0, missStart);
			linerender.SetPosition (1, missEnd);
			fade -= Time.deltaTime;
		}
	}

	public override void OnPrimaryFire() {
		RaycastHit hit;
		// We ignore player collisions.
		if (Physics.Raycast (player.view.position, player.view.forward, out hit, range, ~(1 << LayerMask.NameToLayer ("Player")))) {
			hitPosition.SetParent (hit.collider.transform);
			hitPosition.position = hit.point;
			hitSomething = true;
			hitDist = hit.distance;
			lastPosition = player.transform.position;
			linerender.SetPosition (0, gunBarrelFront.position);
			linerender.SetPosition (1, hit.point);
		} else {
			hitSomething = false;
			fade = 1.0f;
			missStart = gunBarrelFront.position;
			missEnd = player.view.position + player.view.forward*range;
			linerender.SetPosition (0, missStart);
			linerender.SetPosition (1, missEnd);
		}
	}

	public override void OnPrimaryFireRelease() {
		hitSomething = false;
	}
	public override void OnEquip (GameObject Player) {
		// It's now part of the player, make sure we don't collide with rockets and the like.
		gameObject.layer = LayerMask.NameToLayer ("Player");

		player = Player.GetComponent<SourcePlayer> ();
		// Switch time;
		busy = 0.5f;
		gameObject.SetActive (true);
		transform.SetParent (Player.transform);
		transform.position = Player.transform.position-new Vector3(0f,.3f,0f);
		transform.rotation = Quaternion.identity;
		// You'd play some animations here probably.
	}
	// We were either dropped or put into a pocket.
	public override void OnUnequip (GameObject Player) {
		gameObject.layer = LayerMask.NameToLayer ("Default");
		gameObject.GetComponent<Collider> ().enabled = true;
		gameObject.SetActive (false);
	}
}
