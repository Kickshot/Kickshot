using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveManager {
    private static List<SaveData> data;
    // Saves the state of everything that matters, and stores it inside data.
    public static void Save() {
        data = new List<SaveData> ();
        foreach( SourcePlayer player in UnityEngine.Object.FindObjectsOfType<SourcePlayer>() ) {
            data.Add (new SourcePlayerSaveData (player.gameObject));
        }
        foreach( GunBase gun in UnityEngine.Object.FindObjectsOfType<GunBase>() ) {
            if (gun.player == null) { // We don't want to save guns that a player is carrying, they're already saved by SourcePlayerSaveData
                data.Add (new GunSaveData (gun.gameObject));
            }
        }
    }

    // Destroys everything in the world (that has save data), and recreates them with saved parameters.
    public static void Load() {
        foreach( SourcePlayer player in UnityEngine.Object.FindObjectsOfType<SourcePlayer>() ) {
            GameObject.Destroy (player.gameObject);
        }
        foreach( GunBase gun in UnityEngine.Object.FindObjectsOfType<GunBase>() ) {
            GameObject.Destroy (gun.gameObject);
        }
        foreach (SaveData sd in data) {
            sd.Load ();
        }
    }
}
