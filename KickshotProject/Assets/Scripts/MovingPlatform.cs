using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Moves a GameObject between two points over a period of CycleLength seconds
[RequireComponent( typeof(Rigidbody) )]
public class MovingPlatform : Movable {
    public enum MFunc {
        Linear,
        Sin,
        SinZipper,
        LinearZipper
    };
    public float TimerOffset;
    public MFunc MovementFunction = MFunc.Sin;
    private Rigidbody body;
    public Transform Target1;
    public Transform Target2;
    public float CycleLength = 3;
    private Vector3 lastPosition;
    void Start() {
        body = GetComponent<Rigidbody> ();
        body.useGravity = false;
        body.mass = 999999;
        if (Target1 == null || Target2 == null) {
            Debug.LogError ("You must specify target positions for platforms. Drag and drop any object into the Target1/2 slot.");
        }
    }
    void Update () {
        lastPosition = body.position;
        float timer = Time.timeSinceLevelLoad + TimerOffset;
        float progress = 0f;
        switch (MovementFunction) {
        case MFunc.Sin:
            progress = (Mathf.Sin (timer * 2f * Mathf.PI / CycleLength) + 1f) / 2f;
            break;
        case MFunc.Linear:
            float cyc = Helper.fmod (timer, CycleLength);
            if (cyc / CycleLength < 0.5f) {
                progress = (cyc / CycleLength) * 2f;
            } else {
                progress = 1f - (cyc / CycleLength - .5f) * 2f;
            }
            break;
        case MFunc.SinZipper:
            progress = Mathf.Clamp ((Mathf.Sin (timer * 2f * Mathf.PI / CycleLength) + 1f)/1.8f, 0f, 1f);
            break;
        case MFunc.LinearZipper:
            float cyc2 = Helper.fmod (timer, CycleLength);
            float cyc2Progress = cyc2 / CycleLength;
            if (cyc2Progress < 0.1f || cyc2Progress > 0.9f) {
                progress = 0f;
            } else if (cyc2Progress >= 0.1f && cyc2Progress <= 0.4f) {
                progress = (cyc2Progress-0.1f)/0.3f;
            } else if (cyc2Progress >= 0.4f && cyc2Progress <= 0.6f) {
                progress = 1f;
            } else {
                progress = 1f - (cyc2Progress - .6f)/0.3f;
            }
            break;
        }
        body.MovePosition (Target1.position * progress + Target2.position * (1f - progress));
        velocity = (body.position - lastPosition)/Time.deltaTime;
    }
}
