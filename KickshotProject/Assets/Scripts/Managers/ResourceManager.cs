using System.Collections;
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
        resources.Add ("dirtImpact",            new string[]{"Sounds/dirt_impact1", "Sounds/dirt_impact2", "Sounds/dirt_impact3"});
        resources.Add ("woodImpact",            new string[]{"Sounds/wood_impact1", "Sounds/wood_impact2", "Sounds/wood_impact3"});
        resources.Add ("wetImpact",             new string[]{"Sounds/wet_impact1", "Sounds/wet_impact2", "Sounds/wet_impact3"});
        resources.Add ("stoneImpact",           new string[]{"Sounds/stone_impact1", "Sounds/stone_impact2"});
        resources.Add ("metalImpact",           new string[]{"Sounds/metal_impact1", "Sounds/metal_impact2"});
        resources.Add ("wireImpact",            new string[]{"Sounds/chickenwire_impact1"});
        resources.Add ("glassImpact",           new string[]{"Sounds/glass_impact1"});
        resources.Add ("Player",                new string[]{"Prefabs/Characters/SourcePlayer"});
        resources.Add ("RocketLauncher",        new string[]{"Prefabs/Weapons/RocketLauncher"});
        resources.Add ("TractorGrapple",        new string[]{"Prefabs/Weapons/TractorGrapple"});
		resources.Add ("DoubleGun",        		new string[]{"Prefabs/Weapons/DoubleGun"});
        resources.Add ("RecoilGun",             new string[]{"Prefabs/Weapons/RecoilGun"});
        resources.Add ("GroundReloadRecoilGun", new string[]{"Prefabs/Weapons/Testing/GroundReloadRecoilGun"});
        resources.Add ("GrappleHook",           new string[]{"Prefabs/Weapons/Grapple Hook"});
        resources.Add ("Minigun",               new string[]{"Prefabs/Weapons/Minigun"});
        resources.Add ("SequentialNadeLauncher",new string[]{"Prefabs/Weapons/SequentialNadeLauncher"});
        resources.Add ("AceGrunt",              new string[]{"Sounds/AceGrunt1", "Sounds/AceGrunt2"});
        resources.Add ("AcePainGrunt",          new string[]{"Sounds/AceGrunt2"});
        resources.Add ("MedeaGrunt",            new string[]{"Sounds/jumpgrunt"});
        resources.Add ("MedeaPainGrunt",        new string[]{"Sounds/hurtgrunt"});
        resources.Add ("BoneSnap",              new string[]{"Sounds/bonecrack"});

        // Whomp Fortress music.
        resources.Add ("MarioMusic",        new string[]{"Sounds/Super Mario 64 - Main Theme Music - Bob-Omb Battlefield"});
        // DK Mountain music
        resources.Add ("DKMusic",           new string[]{"Sounds/Mario Kart Double Dash - DK Mountain Music"});
        // Luis' level music
        resources.Add ("drum&bass",           new string[]{"Sounds/Paul SG & Carter - A Lot To Talk About"});
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
