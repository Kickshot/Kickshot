using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHolder : MonoBehaviour {
    public List<GunBase> Guns;
    public GunBase EquippedGun;
    private GunHolderSave startSave;
    public class GunHolderSave {
        private List<GunBase> Guns;
        private GunBase EquippedGun;
        public GunHolderSave( GunHolder g ) {
            this.Guns = new List<GunBase>(g.Guns);
            this.EquippedGun = g.EquippedGun;
        }
        public void Load( GunHolder g ) {
            g.Guns = new List<GunBase> (this.Guns);
            foreach( GunBase gun in g.Guns ) {
                gun.OnUnequip(g.gameObject);
                gun.equipped = false;
            }
            g.EquippedGun = EquippedGun;
            g.EquippedGun.OnEquip(g.gameObject);
            g.EquippedGun.equipped = true;
        }
    }
    void Start() {
        foreach( GunBase gun in Guns ) {
            gun.OnUnequip(gameObject);
            gun.equipped = false;
            gun.Resave();
        }
        if ( !Guns.Contains( EquippedGun ) ) {
            Guns.Add(EquippedGun);
        }
        EquippedGun.OnEquip(gameObject);
        EquippedGun.equipped = true;
        EquippedGun.Resave();
        startSave = new GunHolderSave (this);
    }
    void Reset() {
        if (startSave != null) {
            foreach( GunBase gun in Guns ) {
                gun.gameObject.SetActive(true);
                gun.gameObject.SendMessage ("Reset");
            }
            startSave.Load (this);
        }
    }
    void Update() {
        float d = Input.GetAxis ("Mouse ScrollWheel");
        if (d > 0f) {
            Switch (1);
        }
        if (d < 0f) {
            Switch (-1);
        }
    }
    public void Switch(int direction) {
        if (direction == 0) {
            return;
        }
        if (Guns.Count <= 1) {
            return;
        }
        if (EquippedGun == null) {
            EquippedGun = Guns [0];
            EquippedGun.OnEquip (gameObject);
            EquippedGun.equipped = true;
            return;
        }
        EquippedGun.equipped = false;
        EquippedGun.OnUnequip (gameObject);
        // Oh my god % is not actually modulo. Why would you do this C#???
        //EquippedGun = Guns [(Guns.IndexOf(EquippedGun)+direction)%Guns.Count];
        EquippedGun = Guns [(int)Helper.fmod(Guns.IndexOf(EquippedGun)+direction,Guns.Count)];
        EquippedGun.OnEquip (gameObject);
        EquippedGun.equipped = true;
    }
    void OnTriggerEnter( Collider other ) {
        GunBase gun = other.gameObject.GetComponent<GunBase> ();
        if (gun != null && !Guns.Contains(gun)) {
            Guns.Add (gun);
            if (EquippedGun == null) {
                EquippedGun = gun;
                EquippedGun.OnEquip (gameObject);
                EquippedGun.equipped = true;
            } else {
                other.gameObject.SetActive (false);
            }
        }
    }
}
