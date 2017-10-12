using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBase : MonoBehaviour {
	public string gunName = "Gun";
	public float reloadDelay = 2.0f;
	public float primaryFireCooldown = .8f;
	public float secondaryFireCooldown = .8f;
	public float maxAmmo = 100f;
	public float curAmmo = 100f;
	public float magSize = 5f;
	public float ammo = 5f;
	public float primaryFireAmmoCost = 1f;
	public float secondaryFireAmmoCost = 1f;
	public bool equipped = false;
	public float busy = 0f;
	public bool reloading = false;
	public bool autoReload = true;
	public bool autoFire = true;
	public Transform gunBarrel;
	private bool pfiring = false;
	private bool sfiring = false;

	virtual public void OnGUI() {
		if (!equipped) {
			return;
		}
		GUIStyle style = GUIStyle.none;
		style.normal.textColor = Color.black;
		style.fontSize = 24;
		GUI.Label (new Rect (Screen.width+1f-125f, Screen.height-50f, 250, 100), gunName, style);
		style.normal.textColor = Color.red;
		GUI.Label (new Rect (Screen.width-125f, Screen.height-50f, 250, 100), gunName, style);
	}

	virtual public void OnEquip (GameObject Player) {}
	virtual public void OnPrimaryFire() {}
	virtual public void OnPrimaryFireRelease() {}
	virtual public void OnSecondaryFire () {}
	virtual public void OnSecondaryFireRelease () {}
	virtual public void OnReload () {}
	virtual public void OnUnequip (GameObject Player) {}

	virtual public void Update() {
		if (!equipped) {
			return;
		}
		if (busy > 0f) {
			busy -= Time.deltaTime;
			return;
		} else if ( reloading ) {
			ammo = Mathf.Min (curAmmo, magSize);
			curAmmo = Mathf.Max (curAmmo - magSize, 0f);
			reloading = false;
			return;
		}

		if (((Input.GetButtonDown ("Fire1") && !autoFire) || (Input.GetButton("Fire1") && autoFire)) && ammo >= primaryFireAmmoCost) {
			ammo -= primaryFireAmmoCost;
			OnPrimaryFire ();
			busy = primaryFireCooldown;
			pfiring = true;
		}
		if (Input.GetButtonUp ("Fire1") && pfiring) {
			OnPrimaryFireRelease ();
			pfiring = false;
		}
		if (((Input.GetButtonDown ("Fire2") && !autoFire) || (Input.GetButton("Fire2") && autoFire)) && ammo >= secondaryFireAmmoCost) {
			ammo -= secondaryFireAmmoCost;
			OnSecondaryFire ();
			busy = secondaryFireCooldown;
			sfiring = true;
		}
		if (Input.GetButtonUp ("Fire2") && sfiring) {
			OnSecondaryFireRelease ();
			sfiring = false;
		}
		if ((Input.GetButtonDown ("Reload") && ammo < magSize) || ( autoReload && ammo <= 0f )) {
			OnReload ();
			busy = reloadDelay;
			reloading = true;
		}
	}
}
