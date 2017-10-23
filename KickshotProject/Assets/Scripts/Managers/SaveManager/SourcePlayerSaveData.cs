using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourcePlayerSaveData : SaveData {
    // Player
    private Vector3 pos;
    private Quaternion rot;
    private Vector3 velocity;
    private Vector3 gravity;
    private float baseFriction;
    private float maxSpeed;
    private float groundAccelerate;
    private float groundDecellerate;
    private float airAccelerate;
    private float airStrafeAccelerate;
    private float airSpeedBonus;
    private float walkSpeed;
    private float jumpSpeed;
    private float fallSoundThreshold;
    private float fallPunchThreshold;
    private float maxSafeFallSpeed;
    private float jumpSpeedBonus;
    private float health;
    private float mass;

    // Mouse
    private float headrotX;
    private float headrotY;

    // Guns
    private List<GunSaveData> guns;
    private int equippedGun;

    public SourcePlayerSaveData( GameObject obj ) {
        Save( obj );
    }

    public override void Save( GameObject obj ) {
        SourcePlayer p = obj.GetComponent<SourcePlayer> ();
        if (p == null) {
            throw new UnityException ("Tried to save a gameobject as a SourcePlayer, but it isn't one!");
        }
        this.pos = p.transform.position;
        this.rot = p.transform.rotation;
        this.velocity = p.velocity;
        this.gravity = p.gravity;
        this.baseFriction = p.baseFriction;
        this.maxSpeed = p.maxSpeed;
        this.groundAccelerate = p.groundAccelerate;
        this.groundDecellerate = p.groundDecellerate;
        this.airAccelerate = p.airAccelerate;
        this.airStrafeAccelerate = p.airStrafeAccelerate;
        this.airSpeedBonus = p.airSpeedBonus;
        this.walkSpeed = p.walkSpeed;
        this.jumpSpeed = p.jumpSpeed;
        this.fallSoundThreshold = p.fallSoundThreshold;
        this.fallPunchThreshold = p.fallPunchThreshold;
        this.maxSafeFallSpeed = p.maxSafeFallSpeed;
        this.jumpSpeedBonus = p.jumpSpeedBonus;
        this.health = p.health;
        this.mass = p.mass;

        MouseLook m = obj.GetComponent<MouseLook> ();
        if (m == null) {
            throw new UnityException ("Tried to save a gameobject as a SourcePlayer, but it isn't one!");
        }
        this.headrotX = m.rotX;
        this.headrotY = m.rotY;

        GunHolder g = obj.GetComponent<GunHolder> ();
        if (g == null) {
            throw new UnityException ("Tried to save a gameobject as a SourcePlayer, but it isn't one!");
        }
        guns = new List<GunSaveData> ();
        foreach (GunBase gun in g.Guns) {
            guns.Add (new GunSaveData (gun.gameObject));
        }
        this.equippedGun = g.Guns.IndexOf(g.EquippedGun);
    }
    public override GameObject Load() {
        GameObject obj = GameObject.Instantiate (ResourceManager.GetResource<GameObject>("Player"));
        SourcePlayer ps = obj.GetComponent<SourcePlayer> ();
        if (!ps) {
            throw new UnityException ("Tried to load a gameobject as a SourcePlayer, but it isn't one!");
        }
        ps.transform.position = this.pos;
        ps.transform.rotation = this.rot;
        ps.velocity = this.velocity;
        ps.gravity = this.gravity;
        ps.baseFriction = this.baseFriction;
        ps.maxSpeed = this.maxSpeed;
        ps.groundAccelerate = this.groundAccelerate;
        ps.groundDecellerate = this.groundDecellerate;
        ps.airAccelerate = this.airAccelerate;
        ps.airStrafeAccelerate = this.airStrafeAccelerate;
        ps.airSpeedBonus = this.airSpeedBonus;
        ps.walkSpeed = this.walkSpeed;
        ps.jumpSpeed = this.jumpSpeed;
        ps.fallSoundThreshold = this.fallSoundThreshold;
        ps.fallPunchThreshold = this.fallPunchThreshold;
        ps.maxSafeFallSpeed = this.maxSafeFallSpeed;
        ps.jumpSpeedBonus = this.jumpSpeedBonus;
        ps.health = this.health;
        ps.mass = this.mass;

        MouseLook m = obj.GetComponent<MouseLook> ();
        if (m == null) {
            throw new UnityException ("Tried to save a gameobject as a SourcePlayer, but it isn't one!");
        }
        m.SetRotation (Quaternion.Euler (this.headrotX, this.headrotY, 0));
        GunHolder g = obj.GetComponent<GunHolder> ();
        if (g == null) {
            throw new UnityException ("Tried to save a gameobject as a SourcePlayer, but it isn't one!");
        }
        // Recreate all our guns, with the same stats and place them into inventory.
        g.Guns.Clear();
        foreach (GunSaveData gundata in this.guns) {
            GameObject gun = gundata.Load ();
            GunBase realgun = gun.GetComponent<GunBase> ();
            realgun.player = obj.GetComponent<SourcePlayer>();
            realgun.equipped = false;
            g.Guns.Add (realgun);
        }
        if (this.equippedGun != -1 && this.equippedGun < g.Guns.Count) {
            g.EquippedGun = g.Guns [this.equippedGun];
            g.EquippedGun.OnEquip (obj);
            g.EquippedGun.equipped = true;
        }
        // Done!
        return obj;
    }
}
