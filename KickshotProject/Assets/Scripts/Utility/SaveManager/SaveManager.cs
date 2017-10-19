using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveManager {
    public static GameObject playerPrefab;
    public static GameObject rocketLauncherPrefab;
    public static GameObject tractorGrapplePrefab;
    public static GameObject recoilGunPrefab;
    private static List<SaveData> data;
    private static bool loadedPrefabs = false;
    // Unfortunately unity doesn't like static scripts having prefabs in them, and SaveData can't have them because we want to write them to disk.
    // So we have to load our prefabs separately.
    private static void LoadPrefabs() {
        playerPrefab = Resources.Load<GameObject> ("Prefabs/Characters/SourcePlayer");
        rocketLauncherPrefab = Resources.Load<GameObject> ("Prefabs/Weapons/RocketLauncher");
        tractorGrapplePrefab = Resources.Load<GameObject> ("Prefabs/Weapons/TractorGrapple");
        recoilGunPrefab = Resources.Load<GameObject> ("Prefabs/Weapons/RecoilGun");
        loadedPrefabs = true;
    }
    // Saves the state of everything that matters, and stores it inside data.
    public static void Save() {
        if (!loadedPrefabs) {
            LoadPrefabs ();
        }
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
