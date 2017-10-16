using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ImpactSounds {
	private static bool generatedList;
	private static Dictionary<string, List<AudioClip>> soundLookup; // Sound Lookup for playing sounds based on matrial names.
	public static AudioClip GetSound(Material mat) {
		if (!generatedList) {
			GenerateList ();
		}
		if (soundLookup.ContainsKey (mat.name)) {
			List<AudioClip> clips = soundLookup [mat.name];
			return clips [Random.Range (0, clips.Count-1)];
		}
		List<AudioClip> otherclips = soundLookup ["default"];
		return otherclips[Random.Range (0, otherclips.Count-1)];
	}
	private static void GenerateList() {
		if (generatedList) {
			return;
		}
		soundLookup = new Dictionary<string, List<AudioClip>> ();
		// Load our sounds
		List<AudioClip> dirt = new List<AudioClip> ();
		dirt.Add (Resources.Load<AudioClip> ("Sounds/dirt_impact1"));
		dirt.Add (Resources.Load<AudioClip> ("Sounds/dirt_impact2"));
		dirt.Add (Resources.Load<AudioClip> ("Sounds/dirt_impact3"));
		List<AudioClip> wood = new List<AudioClip> ();
		wood.Add (Resources.Load<AudioClip> ("Sounds/wood_impact1"));
		wood.Add (Resources.Load<AudioClip> ("Sounds/wood_impact2"));
		wood.Add (Resources.Load<AudioClip> ("Sounds/wood_impact3"));
		List<AudioClip> wet = new List<AudioClip> ();
		wet.Add (Resources.Load<AudioClip> ("Sounds/wet_impact1"));
		wet.Add (Resources.Load<AudioClip> ("Sounds/wet_impact2"));
		wet.Add (Resources.Load<AudioClip> ("Sounds/wet_impact3"));
		List<AudioClip> stone = new List<AudioClip> ();
		stone.Add (Resources.Load<AudioClip> ("Sounds/stone_impact1"));
		stone.Add (Resources.Load<AudioClip> ("Sounds/stone_impact2"));

		// Setup our lookup table
		soundLookup["default"] = dirt; //Don't delete this,
		soundLookup["Snow (Instance)"] = wet;
		soundLookup["Wood (Instance)"] = wood;
		soundLookup["Sand (Instance)"] = dirt;
		soundLookup["StoneWall (Instance)"] = stone;
	}
}
