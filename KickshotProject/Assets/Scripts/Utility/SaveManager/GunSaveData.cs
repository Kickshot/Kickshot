using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSaveData : SaveData {
    private enum GunType {
        RocketLauncher,
        TractorGrapple,
        RecoilGun
    }
    private GunType type;
    private string gunName;
    private float reloadDelay;
    private float primaryFireCooldown;
    private float secondaryFireCooldown;
    private float maxAmmo;
    private float curAmmo;
    private float magSize;
    private float ammo;
    private float primaryFireAmmoCost;
    private float secondaryFireAmmoCost;
    private float switchBusyTime;
    private float busy;
    private bool reloading;
    private bool autoReload;
    private bool autoFire;
    private bool equipped;
    private Vector3 pos;
    private Quaternion rot;
    private int layer;

    public GunSaveData(GameObject obj) {
        Save (obj);
    }

    public override void Save (GameObject obj) {
        GunBase g = obj.GetComponent<GunBase> ();
        this.gunName = g.gunName;
        this.reloadDelay = g.reloadDelay;
        this.primaryFireCooldown = g.primaryFireCooldown;
        this.secondaryFireCooldown = g.secondaryFireCooldown;
        this.maxAmmo = g.maxAmmo;
        this.curAmmo = g.curAmmo;
        this.magSize = g.magSize;
        this.ammo = g.ammo;
        this.primaryFireAmmoCost = g.primaryFireAmmoCost;
        this.secondaryFireAmmoCost = g.secondaryFireAmmoCost;
        this.switchBusyTime = g.switchBusyTime;
        this.busy = g.busy;
        this.reloading = g.reloading;
        this.autoReload = g.autoReload;
        this.autoFire = g.autoFire;
        this.equipped = g.equipped;
        this.rot = g.transform.rotation;
        this.pos = g.transform.position;
        this.layer = g.gameObject.layer;
        /*switch (g.GetType ()) { // C# doesn't support switches on types :v
        case RocketLauncher:
            type = GunType.RocketLauncher;
            break;
        case TractorGrapple:
            type = GunType.TractorGrapple;
            break;
        case RecoilGun:
            type = GunType.RecoilGun;
        }*/
        if (g.GetType () == typeof(RocketLauncher)) {
            type = GunType.RocketLauncher;
        } else if (g.GetType () == typeof(TractorGrapple)) {
            type = GunType.TractorGrapple;
        } else if (g.GetType () == typeof(RecoilGun)) {
            type = GunType.RecoilGun;
        }
    }
    public override GameObject Load () {
        GameObject obj = null;
        GunBase g = null;
        switch (type) {
        case GunType.RocketLauncher:
            obj = GameObject.Instantiate (SaveManager.rocketLauncherPrefab);
            g = obj.GetComponent<RocketLauncher> ();
            break;
        case GunType.RecoilGun:
            obj = GameObject.Instantiate (SaveManager.recoilGunPrefab);
            g = obj.GetComponent<RecoilGun> ();
            break;
        case GunType.TractorGrapple:
            obj = GameObject.Instantiate (SaveManager.tractorGrapplePrefab);
            g = obj.GetComponent<TractorGrapple> ();
            break;
        }
        if (obj == null || g == null) {
            throw new UnityException ("Whoa nelly this shouldn't happen, fix up your prefabs boy.");
        }
        g.gunName = this.gunName;
        g.reloadDelay = this.reloadDelay;
        g.primaryFireCooldown = this.primaryFireCooldown;
        g.secondaryFireCooldown = this.secondaryFireCooldown;
        g.maxAmmo = this.maxAmmo;
        g.curAmmo = this.curAmmo;
        g.magSize = this.magSize;
        g.ammo = this.ammo;
        g.primaryFireAmmoCost = this.primaryFireAmmoCost;
        g.secondaryFireAmmoCost = this.secondaryFireAmmoCost;
        g.switchBusyTime = this.switchBusyTime;
        g.busy = this.busy;
        g.reloading = this.reloading;
        g.autoReload = this.autoReload;
        g.autoFire = this.autoFire;
        g.equipped = this.equipped;
        g.transform.rotation = this.rot;
        g.transform.position = this.pos;
        g.gameObject.layer = this.layer;
        if ( g.gameObject.layer != LayerMask.NameToLayer("Player") ) {
            foreach (Collider col in g.gameObject.GetComponentsInChildren<Collider> ()) {
                col.enabled = true;
            }
        } else {
            foreach (Collider col in g.gameObject.GetComponentsInChildren<Collider> ()) {
                col.enabled = false;
            }
        }
        return obj;
    }
}
