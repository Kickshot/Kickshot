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
        generatedList = true;
    }
}
