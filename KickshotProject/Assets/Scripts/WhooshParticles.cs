using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class WhooshParticles : MonoBehaviour {
    public SourcePlayer _player;
    public float _startSpeedThreshold = 20f;
    public float _maxSpeed = 50f;

    ParticleSystem _particles;

    private void Start()
    {
        _particles = GetComponent<ParticleSystem>();
    }


	void Update () {
        float speed = _player.velocity.magnitude;
        float whooshScale = Mathf.Clamp01((speed - _startSpeedThreshold) / (_maxSpeed - _startSpeedThreshold));

        // Played around with rotating emitter with velocity, but doesn't feel right
        //transform.rotation = Quaternion.FromToRotation(Vector3.forward, _player.velocity.normalized);

        Color c = _particles.main.startColor.color;
        var m = _particles.main;
        m.startColor = new Color(c.r, c.g, c.b, whooshScale * whooshScale * Mathf.Max(Vector3.Dot(Vector3.Normalize(_player.velocity), _player.GetComponent<MouseLook>().view.forward),0f));
	}
}
