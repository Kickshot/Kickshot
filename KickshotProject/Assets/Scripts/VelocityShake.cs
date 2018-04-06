using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityShake : MonoBehaviour
{
    public SourcePlayer _player;
    public float _startSpeedThreshold = 20f;
    public float _maxSpeed = 50f;

    SmartCamera _cam;

    private void Start()
    {
        _player = GameObject.Find("SourcePlayer").GetComponent<SourcePlayer>();
        _cam = GameObject.Find("Eye Camera").GetComponent<SmartCamera>();
    }


    void Update()
    {
        float speed = _player.velocity.magnitude;
        float whooshScale = Mathf.Clamp01((speed - _startSpeedThreshold) / (_maxSpeed - _startSpeedThreshold));

        // Played around with rotating emitter with velocity, but doesn't feel right
        //transform.rotation = Quaternion.FromToRotation(Vector3.forward, _player.velocity.normalized);

        _cam.SetShake(whooshScale);
    }
}
