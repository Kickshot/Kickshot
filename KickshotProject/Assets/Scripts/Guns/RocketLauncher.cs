using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : GunBase {
	public Rocket rocket;
	private SourcePlayer player;
	void Start() {
		gunName = "Rocket Launcher";
	}
	override public void Update() {
		base.Update ();
		if (!equipped) {
			return;
		}
		transform.rotation = player.view.rotation;
		if ( reloading ) {
			transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(-player.view.forward),Quaternion.LookRotation(player.view.forward),busy/reloadDelay);
		}
	}
	public override void OnPrimaryFire() {
		RaycastHit hit;
		Vector3 hitpos = player.view.forward * 1000f;
		if (Physics.Raycast (player.view.position, player.view.forward, out hit, 1000f)) {
			hitpos = hit.point;
		}
		Rocket r = Instantiate (rocket, gunBarrel.position, Quaternion.LookRotation (hitpos-gunBarrel.position));
		r.inheritedVel = player.velocity+player.groundVelocity;
	}
	public override void OnEquip (GameObject Player)
	{
		gameObject.GetComponent<Collider> ().enabled = false;
		player = Player.GetComponent<SourcePlayer> ();
		// Switch time;
		busy = 0.5f;
		gameObject.SetActive (true);
		transform.SetParent (Player.transform);
		transform.position = Player.transform.position-new Vector3(0f,.3f,0f);
		transform.rotation = Quaternion.identity;
		// You'd play some animations here probably.
	}
	public override void OnUnequip (GameObject Player)
	{
		gameObject.GetComponent<Collider> ().enabled = true;
		gameObject.SetActive (false);
	}
}
