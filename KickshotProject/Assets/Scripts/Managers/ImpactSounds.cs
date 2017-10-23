using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ImpactSounds {
    private static bool generatedList;
    private static Dictionary<string, string> soundLookup; // Sound Lookup for playing sounds based on matrial names.
    public static AudioClip GetSound(Material mat) {
        if (!generatedList) {
            GenerateList ();
        }
        if (mat != null && soundLookup.ContainsKey (mat.name)) {
            return ResourceManager.GetResource<AudioClip>(soundLookup[mat.name]);
        }
        return ResourceManager.GetResource<AudioClip>(soundLookup["default"]);
    }
    private static void GenerateList() {
        if (generatedList) {
            return;
        }
        soundLookup = new Dictionary<string, string> ();
        // Setup our lookup table
        soundLookup["default"] = "dirtImpact"; //Don't delete this,
        soundLookup["Snow (Instance)"] = "wetImpact";
        soundLookup["Wood (Instance)"] = "woodImpact";
        soundLookup["Sand (Instance)"] = "dirtImpact";
        soundLookup["StoneWall (Instance)"] = "stoneImpact";
        soundLookup["CobbleStone (Instance)"] = "stoneImpact";

        // Whomp Fortress
        soundLookup["30E48A81_c (Instance)"] = "stoneImpact";
        soundLookup["53DCECCE_c (Instance)"] = "stoneImpact";
        soundLookup["66E3247E_c (Instance)"] = "stoneImpact";
        soundLookup["59A41B70_c (Instance)"] = "stoneImpact";
        soundLookup["60892055_c (Instance)"] = "stoneImpact";
        soundLookup["710C9EF6_c (Instance)"] = "stoneImpact";
        soundLookup["2A04AD32_c (Instance)"] = "stoneImpact";
        soundLookup["2CF33CF6_c (Instance)"] = "stoneImpact";
        soundLookup["29730C45_c (Instance)"] = "stoneImpact";
        soundLookup["6DAF90F6_c (Instance)"] = "wetImpact";
        soundLookup["4D3B21B2_c (Instance)"] = "dirtImpact";
        soundLookup["4399CB9D_c (Instance)"] = "woodImpact";
        generatedList = true;
    }
}
