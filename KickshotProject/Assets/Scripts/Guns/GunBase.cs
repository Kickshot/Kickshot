using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBase : MonoBehaviour {
    public string gunName = "Gun"; // Name of the gun, not really important.
    public float reloadDelay = 2.0f; // How long in seconds it takes before the gun is fully reloaded.
    public float primaryFireCooldown = .8f; // How long in seconds before we can shoot the next bullet.
    public float secondaryFireCooldown = .8f;
    public float maxAmmo = 100f; // The max capacity of our pockets for ammo.
    public float curAmmo = 100f; // The current amount of ammo in our pockets.
    public float magSize = 5f; // The max capacity of our magazine.
    public float ammo = 5f; // The current amount of ammo in our magazine.
    public float primaryFireAmmoCost = 1f; // How much ammo does it take to primary fire?
    public float secondaryFireAmmoCost = 1f;
    public float switchBusyTime = 0.2f; // How long in seconds is the gun busy for during switch.
    public float busy = 0f; // Time in seconds we're stunned for.
    public bool reloading = false; // Reload doesn't complete until we're no longer busy, this tells the clip to fill up after we're no longer busy.
    public bool autoReload = true; // Checks if the gun is out of ammo and not busy, then reloads it automatically. Otherwise the player has to press the reload key.
    public bool autoFire = true; // Determines if the player can just hold down the mouse to fire, rather than spam clicks.

    public SourcePlayer player;
    public Transform view;
    public bool equipped = false; // This is used internally to turn on/off the actual gun stuff. If we're unequipped we're either on the floor in in someone's pockets.
    private bool pfiring = false; // These dumb booleans just keep track to make sure that OnPrimaryRelease doesn't get called before OnPrimaryFire.
    private bool sfiring = false;
    private GunSave startSave;

    public void Resave() {
        startSave = new GunSave(this);
    }

    public class GunSave {
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
        private SourcePlayer player;
        private Transform view;
        private bool equipped;
        private Vector3 pos;
        private Quaternion rot;
        private Transform parent;
        private int layer;
        public GunSave( GunBase g ) {
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
            this.player = g.player;
            this.view = g.view;
            this.equipped = g.equipped;
            this.rot = g.transform.rotation;
            this.pos = g.transform.position;
            this.parent = g.transform.parent;
            this.layer = g.gameObject.layer;
        }
        public void Load( GunBase g ) {
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
            g.player = this.player;
            g.view = this.view;
            g.equipped = this.equipped;
            g.transform.rotation = this.rot;
            g.transform.position = this.pos;
            g.transform.parent = this.parent;
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
        }
    }

    void Start() {
        startSave = new GunSave( this );
    }

    void Reset() {
        if ( startSave != null ) {
            startSave.Load( this );
        }
    }

    virtual public void OnGUI() {
        if (!equipped) {
            return;
        }
        GUIStyle style = GUIStyle.none;
        style.normal.textColor = Color.black;
        style.fontSize = 24;
        GUI.Label (new Rect (Screen.width+1f-300f, Screen.height-50f, 250, 50), gunName, style);
        GUI.Label (new Rect (Screen.width+1f-300f, Screen.height-100f, 250, 50), ammo + "/" + curAmmo, style);
        style.normal.textColor = Color.red;
        GUI.Label (new Rect (Screen.width-300f, Screen.height-50f, 250, 50), gunName, style);
        GUI.Label (new Rect (Screen.width-300f, Screen.height-100f, 250, 50), ammo + "/" + curAmmo, style);
    }

    virtual public void OnEquip (GameObject Player) {
        foreach (Collider col in gameObject.GetComponentsInChildren<Collider> ()) {
            col.enabled = false;
        }
        // It's now part of the player, make sure we don't collide with rockets and the like.
        gameObject.layer = LayerMask.NameToLayer ("Player");
        // Switch time
        busy = switchBusyTime;
        player = Player.GetComponent<SourcePlayer> ();
        view = Player.GetComponent<MouseLook> ().view;
        gameObject.SetActive (true);
        transform.SetParent (Player.transform);
        transform.position = Player.transform.position + Player.transform.right * .5f - Player.transform.up * .3f;
        transform.rotation = Quaternion.identity;
    }
    virtual public void OnPrimaryFire() {}
    virtual public void OnPrimaryFireRelease() {}
    virtual public void OnSecondaryFire () {}
    virtual public void OnSecondaryFireRelease () {}
    virtual public void OnReload () {}
    virtual public void OnUnequip (GameObject Player) {
        foreach (Collider col in gameObject.GetComponentsInChildren<Collider> ()) {
            col.enabled = true;
        }
        gameObject.layer = LayerMask.NameToLayer ("Default");
        gameObject.SetActive (false);
    }

    virtual public void Update() {
        if (!equipped) {
            return;
        }
        // We can reload even while we're busy.
        if (!reloading && ((Input.GetButtonDown ("Reload") && ammo < magSize) || ( autoReload && ammo <= 0f ))) {
            busy = reloadDelay;
            reloading = true;
            OnReload ();
        }
        // We've released the fire button, run our hook.
        if (Input.GetButtonUp("Fire1") && pfiring) {
            pfiring = false;
            OnPrimaryFireRelease ();
        }
        if (Input.GetButtonUp ("Fire2") && sfiring) {
            sfiring = false;
            OnSecondaryFireRelease ();
        }
        // If we're busy, reduce the timer and exit.
        if (busy > 0f) {
            busy -= Time.deltaTime;
            return;
            // If we're not busy, and our reload flag is set, that means we've completed a reload.
        } else if ( reloading ) {
            curAmmo = Mathf.Max (curAmmo - magSize+ammo, 0f);
            ammo = Mathf.Min (curAmmo, magSize);
            reloading = false;
            return;
        }
        // We check if we should fire, meaning we're not busy (line 49), we've just clicked (!autoFire) or we're holding down the fire button (autofire), and that we have ammo available to fire.
        if (((Input.GetButtonDown ("Fire1") && !autoFire) || (Input.GetButton("Fire1") && autoFire)) && ammo >= primaryFireAmmoCost) {
            ammo -= primaryFireAmmoCost;
            // Make ourselves busy for primaryFireCooldown seconds.
            busy = primaryFireCooldown;
            pfiring = true;
            OnPrimaryFire ();
        }
        if (((Input.GetButtonDown ("Fire2") && !autoFire) || (Input.GetButton("Fire2") && autoFire)) && ammo >= secondaryFireAmmoCost) {
            ammo -= secondaryFireAmmoCost;
            busy = secondaryFireCooldown;
            sfiring = true;
            OnSecondaryFire ();
        }
    }
}
