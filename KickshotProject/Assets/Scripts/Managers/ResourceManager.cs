﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceManager {
    private static Dictionary<string, string[]> resources;
    private static bool generated = false;
    // Generates a list of static resources.
    private static void GenerateResourceList() {
        if (generated) {
            return;
        }
        resources = new Dictionary<string, string[]> ();
        resources.Add ("dirtImpact",            new string[]{"Sounds/landing"});
        resources.Add ("woodImpact",            new string[]{"Sounds/landing"});
        resources.Add ("wetImpact",             new string[]{"Sounds/landing"});
        resources.Add ("stoneImpact",           new string[]{"Sounds/landing"});
        resources.Add ("metalImpact",           new string[]{"Sounds/landing"});
        resources.Add ("wireImpact",            new string[]{"Sounds/landing"});
        resources.Add ("glassImpact",           new string[]{"Sounds/landing"});
        resources.Add ("Player",                new string[]{"Prefabs/Characters/SourcePlayer"});
        resources.Add ("RocketLauncher",        new string[]{"Prefabs/Weapons/RocketLauncher"});
        resources.Add ("TractorGrapple",        new string[]{"Prefabs/Weapons/TractorGrapple"});
		resources.Add ("DoubleGun",        		new string[]{"Prefabs/Weapons/DoubleGun"});
        resources.Add ("RecoilGun",             new string[]{"Prefabs/Weapons/RecoilGun"});
        resources.Add ("GroundReloadRecoilGun", new string[]{"Prefabs/Weapons/Testing/GroundReloadRecoilGun"});
        resources.Add ("GrappleHook",           new string[]{"Prefabs/Weapons/Grapple Hook"});
        resources.Add ("Minigun",               new string[]{"Prefabs/Weapons/Minigun"});
        resources.Add ("SequentialNadeLauncher",new string[]{"Prefabs/Weapons/SequentialNadeLauncher"});
        resources.Add ("AceGrunt",              new string[]{"Sounds/jump"});
        resources.Add ("AcePainGrunt",          new string[]{"Sounds/AceGrunt2"});
        resources.Add ("MedeaGrunt",            new string[]{"Sounds/jumpgrunt"});
        resources.Add ("MedeaPainGrunt",        new string[]{"Sounds/hurtgrunt"});
        resources.Add ("BoneSnap",              new string[]{"Sounds/landing"});
        resources.Add("ShatterWall",            new string[] { "Prefabs/WorldObjects/ShatterWall" });

        // Whomp Fortress music.
        resources.Add ("MarioMusic",        new string[]{"Sounds/Super Mario 64 - Main Theme Music - Bob-Omb Battlefield"});
        // DK Mountain music
        resources.Add ("DKMusic",           new string[]{"Sounds/Mario Kart Double Dash - DK Mountain Music"});
        // Luis' level music
        resources.Add ("Menu",              new string[]{ "Sounds/Music/Nomyn - Above The Clouds" });
        resources.Add("I01", new string[] { "Sounds/Music/IslandCandidates/01" });
        resources.Add("I02", new string[] { "Sounds/Music/IslandCandidates/02" });
        resources.Add("I03", new string[] { "Sounds/Music/IslandCandidates/03" });
        resources.Add("I04", new string[] { "Sounds/Music/IslandCandidates/04" });
        resources.Add("I05", new string[] { "Sounds/Music/IslandCandidates/05" });
        resources.Add("I06", new string[] { "Sounds/Music/IslandCandidates/06" });
        resources.Add("I07", new string[] { "Sounds/Music/IslandCandidates/07" });
        resources.Add("I08", new string[] { "Sounds/Music/IslandCandidates/08" });
        resources.Add("I09", new string[] { "Sounds/Music/IslandCandidates/09" });
        resources.Add("I10", new string[] { "Sounds/Music/IslandCandidates/05" });
        resources.Add("G01", new string[] { "Sounds/Music/IslandCandidates/04" });
        resources.Add("G02", new string[] { "Sounds/Music/IslandCandidates/07" });
        resources.Add("G03", new string[] { "Sounds/Music/IslandCandidates/06" });
        resources.Add("D01", new string[] { "Sounds/Music/DesertCandidates/01" });
        resources.Add("D02", new string[] { "Sounds/Music/DesertCandidates/02" });
        resources.Add("D03", new string[] { "Sounds/Music/DesertCandidates/03" });
        resources.Add("D04", new string[] { "Sounds/Music/DesertCandidates/01" });
        generated = true;
    }
    // Randomly returns a resource with the given name.
    public static T GetResource<T>(string name) where T : class {
        if (!generated) {
            GenerateResourceList ();
        }
        if (!resources.ContainsKey (name)) {
            // TODO: Return some default materials/sounds here, like purple checkboard, giant flashing error models kinda thing.
            throw new UnityException("Resource not found: "+name);
        }
        string[] array = resources [name];
        int rand = Random.Range (0, array.Length - 1);
        T resource = Resources.Load(array [rand]) as T;
        if (resource == null) {
            throw new UnityException ("Resource specified not found in Resource folder: " + array [rand]);
        }
        return resource;
    }
    // Checks all the specified resources to make sure they're not missing or wrongly specified. Throws an exception otherwise.
    // Takes up a lot of memory and shouldn't be ran during normal gameplay.
    public static void Verify() {
        if (!Application.isEditor) {
            throw new UnityException ("Don't run Verify() in the real game please...");
        }
        foreach (KeyValuePair<string,string[]> resource in resources) {
            foreach (string s in resource.Value) {
                if (Resources.Load (s) == null) {
                    throw new UnityException ("Resource " + s + " not found from resource bundle " + resource.Key);
                }
            }
        }
        Debug.Log ("All good!");
    }
}
