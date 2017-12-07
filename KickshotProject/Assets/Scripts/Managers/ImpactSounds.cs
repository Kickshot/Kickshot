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

        // Sounds generated for WhompFortress
        soundLookup["30E48A81_c (Instance)"] = "stoneImpact";
        soundLookup["C2DAC08_c (Instance)"] = "stoneImpact";
        soundLookup["2C725067_c (Instance)"] = "stoneImpact";
        soundLookup["59A41B70_c (Instance)"] = "stoneImpact";
        soundLookup["29730C45_c (Instance)"] = "stoneImpact";
        soundLookup["2CF33CF6_c (Instance)"] = "stoneImpact";
        soundLookup["710C9EF6_c (Instance)"] = "stoneImpact";
        soundLookup["4399CB9D_c (Instance)"] = "woodImpact";
        soundLookup["422EF87F_c (Instance)"] = "stoneImpact";
        soundLookup["6DAF90F6_c (Instance)"] = "wetImpact";
        soundLookup["18F70B20_c (Instance)"] = "metalImpact";
        soundLookup["2A04AD32_c (Instance)"] = "stoneImpact";
        soundLookup["60892055_c (Instance)"] = "stoneImpact";
        soundLookup["5AEE262F_c (Instance)"] = "metalImpact";

        // Sounds generated for jump_elephant
        soundLookup["rockwall001 (Instance)"] = "dirtImpact";
        soundLookup["rockground003 (Instance)"] = "dirtImpact";
        soundLookup["grain_elevator_facade_06 (Instance)"] = "woodImpact";
        soundLookup["grain_elevator_facade_14a (Instance)"] = "woodImpact";
        soundLookup["wood_floor01 (Instance)"] = "woodImpact";
        soundLookup["woodtrim001 (Instance)"] = "woodImpact";
        soundLookup["woodwall_brownslats001 (Instance)"] = "woodImpact";
        soundLookup["fence_alpha (Instance)"] = "wireImpact";
        soundLookup["wood_wall004 (Instance)"] = "woodImpact";
        soundLookup["water_water_well (Instance)"] = "wetImpact";
        soundLookup["grain_elevator_facade_08 (Instance)"] = "woodImpact";
        soundLookup["glasswindow001a (Instance)"] = "glassImpact";
        soundLookup["gravelgrayred001 (Instance)"] = "dirtImpact";
        soundLookup["concretefloor007b (Instance)"] = "stoneImpact";
        soundLookup["flat_wall_02 (Instance)"] = "stoneImpact";
        soundLookup["hyro_flat_outdoors_01 (Instance)"] = "stoneImpact";

        // Sounds generated for DKMountain
        soundLookup["dk.3SS06 (Instance)"] = "woodImpact";
        soundLookup["dk.3SS12 (Instance)"] = "stoneImpact";
        soundLookup["dk.3SS04 (Instance)"] = "dirtImpact";
        soundLookup["dk.3SS17 (Instance)"] = "wireImpact";

        // Sounds generated for plr_hightower
        soundLookup["wall020b (Instance)"] = "woodImpact";
        soundLookup["wood_beam03 (Instance)"] = "woodImpact";
        soundLookup["wood_bridge001 (Instance)"] = "woodImpact";
        soundLookup["grain_elevator_facade_14a (Instance)"] = "woodImpact";
        soundLookup["steeldoor001 (Instance)"] = "metalImpact";
        soundLookup["wall007a (Instance)"] = "woodImpact";
        soundLookup["glasswindow001a (Instance)"] = "glassImpact";
        soundLookup["wood_wall002b (Instance)"] = "woodImpact";
        soundLookup["wood_wall002 (Instance)"] = "woodImpact";
        soundLookup["wall028 (Instance)"] = "woodImpact";
        soundLookup["Chicken_Wire001 (Instance)"] = "wireImpact";
        soundLookup["wall025 (Instance)"] = "metalImpact";
        soundLookup["Ibeam001 (Instance)"] = "metalImpact";
        soundLookup["wall011g (Instance)"] = "metalImpact";
        soundLookup["wall019 (Instance)"] = "metalImpact";
        soundLookup["wall011 (Instance)"] = "metalImpact";
        soundLookup["wood_floor002 (Instance)"] = "woodImpact";
        soundLookup["rockwall013 (Instance)"] = "dirtImpact";
        soundLookup["rockwall012 (Instance)"] = "dirtImpact";
        soundLookup["wall011d (Instance)"] = "metalImpact";
        soundLookup["wall011g (Instance)"] = "metalImpact";
        soundLookup["Imetal005 (Instance)"] = "metalImpact";
        soundLookup["concretewall011a (Instance)"] = "stoneImpact";
        soundLookup["wall016d (Instance)"] = "metalImpact";
        soundLookup["Ibeam002 (Instance)"] = "metalImpact";
        soundLookup["metaldoor01_192 (Instance)"] = "metalImpact";

        // Sounds generated for CrouchUserStory
        soundLookup["carpetfloor003a (Instance)"] = "dirtImpact";
        soundLookup["woodwall_darkbrown001 (Instance)"] = "woodImpact";
        soundLookup["concretefloor031a (Instance)"] = "stoneImpact";
        soundLookup["metal02 (Instance)"] = "metalImpact";
        soundLookup["glasswindow001a (Instance)"] = "glassImpact";
        soundLookup["metalstainless01 (Instance)"] = "metalImpact";
        soundLookup["concretefloor014a (Instance)"] = "stoneImpact";

        // Sounds generated for 
        soundLookup["wood_wall006 (Instance)"] = "woodImpact";
        soundLookup["carpetfloor003a (Instance)"] = "dirtImpact";
        soundLookup["metaltrack001 (Instance)"] = "metalImpact";

        // Sounds generated for MovementTutorial
        soundLookup["tilefloor021a (Instance)"] = "stoneImpact";
        soundLookup["carpetfloor001a (Instance)"] = "dirtImpact";
        soundLookup["concretefloor011a (Instance)"] = "stoneImpact";
        soundLookup["concretewall074a (Instance)"] = "stoneImpact";
        soundLookup["nuke_metalgrate_01 (Instance)"] = "metalImpact";
        soundLookup["metaldoor046a (Instance)"] = "metalImpact";
        soundLookup["dev_hazzardstripe01a (Instance)"] = "metalImpact";
        soundLookup["rubberfloor004a (Instance)"] = "metalImpact";
        soundLookup["metalgrate013b (Instance)"] = "metalImpact";
        soundLookup["wall026 (Instance)"] = "metalImpact";
        soundLookup["train_metalceiling_01 (Instance)"] = "metalImpact";
        soundLookup["metaltruss012b (Instance)"] = "metalImpact";
        soundLookup["wall011g (Instance)"] = "metalImpact";
        soundLookup["carpet-sn03 (Instance)"] = "dirtImpact";
        soundLookup["TexturesCom_WoodPlanksBare0464_5_seamless_S (Instance)"] = "woodImpact";
        soundLookup["wood_wall002 (Instance)"] = "woodImpact";
        soundLookup["wallpaper02a (Instance)"] = "dirtImpact";
        soundLookup["stonecolumn001a (Instance)"] = "stoneImpact";
        soundLookup["Floor_Planks (Instance)"] = "woodImpact";
        soundLookup["TexturesCom_WoodPlanksBare0464_5_seamless_S (Instance)"] = "woodImpact";

        // Sounds generated for Daniel's level 2
        soundLookup["concretefloor005a (Instance)"] = "stoneImpact";
        soundLookup["StoneWall (Instance)"] = "stoneImpact";
        soundLookup["metalroof006a (Instance)"] = "metalImpact";
        soundLookup["spinning_blade (Instance)"] = "metalImpact";
        soundLookup["rockLayeredMeshes_MAT (Instance)"] = "stoneImpact";
        soundLookup["Wood (Instance)"] = "woodImpact";
        soundLookup["TexturesCom_WoodPlanksBare0464_5_seamless_S (Instance)"] = "woodImpact";
        soundLookup["tilefloor021a (Instance)"] = "stoneImpact";

        // Sounds generated for Ramp jump 2
        soundLookup["concretefloor037a_color (Instance)"] = "stoneImpact";
        soundLookup["blacktop_ext_01_color (Instance)"] = "stoneImpact";
        soundLookup["brickwall045d_color (Instance)"] = "stoneImpact";
        soundLookup["brickwall045a_color (Instance)"] = "stoneImpact";
        soundLookup["hr_concrete_floor_04_color (Instance)"] = "stoneImpact";

        generatedList = true;
    }
}
