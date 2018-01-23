using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GunSaveData : SaveData {
    public enum GunType {
        RocketLauncher,
        TractorGrapple,
        RecoilGun,
        GroundReloadRecoilGun,
        GrappleHook,
        Minigun,
        SequentialNadeLauncher,
		DoubleGun
    }
    public GunType type;
    public string gunName;
    public float reloadDelay;
    public float primaryFireCooldown;
    public float secondaryFireCooldown;
    public float maxAmmo;
    public float curAmmo;
    public float magSize;
    public float ammo;
    public float primaryFireAmmoCost;
    public float secondaryFireAmmoCost;
    public float switchBusyTime;
    public float busy;
    public bool reloading;
    public bool autoReload;
    public bool autoFire;
    public bool equipped;
    public Vector3 pos;
    public Quaternion rot;
    public int layer;
    public bool enabled;

    public GunSaveData(GameObject obj) {
        Save (obj);
    }

    public override void Save (GameObject obj)
    {
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
        this.enabled = g.isActiveAndEnabled;
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
        } else if (g.GetType () == typeof(GroundReloadRecoilGun)) {
            type = GunType.GroundReloadRecoilGun;
        } else if (g.GetType () == typeof(GrappleHook)) {
            type = GunType.GrappleHook;
        } else if (g.GetType () == typeof(MiniGun)) {
            type = GunType.Minigun;
        } else if (g.GetType () == typeof(SequentialNadeLauncher)) {
            type = GunType.SequentialNadeLauncher;
        }
		else if (g.GetType () == typeof(DoubleGun)) {
			type = GunType.DoubleGun;
		}
    }
    public override GameObject Load () {
        GameObject obj = null;
        GunBase g = null;
        switch (type) {
        case GunType.RocketLauncher:
            obj = GameObject.Instantiate (ResourceManager.GetResource<GameObject>("RocketLauncher"));
            g = obj.GetComponent<RocketLauncher> ();
            break;
        case GunType.RecoilGun:
            obj = GameObject.Instantiate (ResourceManager.GetResource<GameObject>("RecoilGun"));
            g = obj.GetComponent<RecoilGun> ();
            break;
        case GunType.TractorGrapple:
            obj = GameObject.Instantiate (ResourceManager.GetResource<GameObject>("TractorGrapple"));
            g = obj.GetComponent<TractorGrapple> ();
            break;
        case GunType.GroundReloadRecoilGun:
            obj = GameObject.Instantiate (ResourceManager.GetResource<GameObject>("GroundReloadRecoilGun"));
            g = obj.GetComponent<GroundReloadRecoilGun> ();
            break;
        case GunType.GrappleHook:
            obj = GameObject.Instantiate (ResourceManager.GetResource<GameObject>("GrappleHook"));
            g = obj.GetComponent<GrappleHook> ();
            break;
        case GunType.Minigun:
            obj = GameObject.Instantiate (ResourceManager.GetResource<GameObject>("Minigun"));
            g = obj.GetComponent<MiniGun> ();
            break;
        case GunType.SequentialNadeLauncher:
            obj = GameObject.Instantiate (ResourceManager.GetResource<GameObject>("SequentialNadeLauncher"));
            g = obj.GetComponent<SequentialNadeLauncher> ();
            break;
		case GunType.DoubleGun:
			obj = GameObject.Instantiate (ResourceManager.GetResource<GameObject>("DoubleGun"));
			g = obj.GetComponent<DoubleGun> ();
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
        g.gameObject.SetActive (this.enabled);
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

    public override object Deserialize(string json)
    {
        return JsonUtility.FromJson<GunSaveData>(json);
    }
}
