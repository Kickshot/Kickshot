using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactSoundEmitter : MonoBehaviour {
	//                       Intensity
	// MaterialName  		0		1		2
	//              Sand    s0.wav	s1.wav	s2.wav
	//				Wood	w0.wav	w1.wav	w2.wav
	public List<Dictionary<string, AudioSource>> soundMatrix;
	private SourcePlayer player;
	private Vector3 lastVel;

	void Start() {
		player = gameObject.GetComponent<SourcePlayer> ();
	}

	void Update() {
		if (player == null) {
			return;
		}
		float mag = (lastVel - player.velocity).magnitude;
		if (mag > player.fallPunchThreshold) {
			// intensity scales from 0 to 1, based on fallPunchThreshold.
			float intensity = (player.maxSafeFallSpeed - mag)/(player.maxSafeFallSpeed-player.fallPunchThreshold);
		}
	}

	/// <summary>
	/// Play a sound at the specified intensity
	/// </summary>
	/// <param name="intensity">A float, 0 through 1 describing how hard the impact is.</param>
	/// /// <param name="material">Which material we're impacting</param>
	public void Impact(float intensity, Material material) {
		int column = (int)Mathf.Floor(intensity*soundMatrix.Count);
		string row = material.name;
		if (!soundMatrix [column].ContainsKey (row)) {
			Debug.Log ("No impact sound for intensity " + intensity + " and material " + material.name);
		}
		soundMatrix [column] [row].volume = intensity;
		soundMatrix [column] [row].Play ();
	}
}
