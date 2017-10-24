using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SourcePlayerSaveData : SaveData
{
    // Player
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 velocity;
    public Vector3 gravity;
    public float baseFriction;
    public float maxSpeed;
    public float groundAccelerate;
    public float groundDecellerate;
    public float airAccelerate;
    public float airStrafeAccelerate;
    public float airSpeedBonus;
    public float airSpeedPunish;
    public float airBreak;
    public float walkSpeed;
    public float jumpSpeed;
    public float fallSoundThreshold;
    public float fallPunchThreshold;
    public float maxSafeFallSpeed;
    public float jumpSpeedBonus;
    public float health;
    public float mass;

    // Mouse
    public float headrotX;
    public float headrotY;

    // Guns
    public GunSaveData[] guns;
    public int equippedGun;

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
        this.airSpeedPunish = p.airSpeedPunish;
        this.airBreak = p.airBreak;
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
        guns = new GunSaveData[g.Guns.Count];

        int i = 0;
        foreach (GunBase gun in g.Guns) {
            guns[i] = new GunSaveData(gun.gameObject);
            i++;
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
        ps.airSpeedPunish = this.airSpeedPunish;
        ps.airBreak = this.airBreak;
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

    public override object Deserialize(string json)
    {
        return JsonUtility.FromJson<SourcePlayerSaveData>(json);
    }
}
