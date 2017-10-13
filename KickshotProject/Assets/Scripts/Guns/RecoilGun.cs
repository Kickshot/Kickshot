using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilGun : GunBase {
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
	}
	public override void OnPrimaryFire() {
		blam.Play ();
		player.velocity -= view.forward * strength;
	}
}
