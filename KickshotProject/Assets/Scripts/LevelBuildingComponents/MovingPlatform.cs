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
    public float LinearZipperPauseFraction = 0.1f;
    float lastProgress;
    void Start() {
        // Prevent triggers from getting casted on Move
        foreach (Collider col in GetComponentsInChildren<Collider>()) {
            if (col.isTrigger) {
                if (col.GetComponent<Rigidbody> () == null) {
                    Debug.LogError ("Triggers on moving platforms don't work properly unless you add a rigidbody to them!\nUnfortunately I can't add it for you for various reasons.");
                    /*Rigidbody r = col.gameObject.AddComponent<Rigidbody> ();
                    r.useGravity = false;
                    r.constraints = RigidbodyConstraints.FreezeAll;*/
                }
            }
        }
        body = GetComponent<Rigidbody> ();
        body.isKinematic = true;
        body.useGravity = false;
        body.mass = 999999;
        if (Target1 == null || Target2 == null) {
            Debug.LogError ("You must specify target positions for platforms. Drag and drop any object into the Target1/2 slot.");
        }
    }
    void Move( Vector3 lastPos, Vector3 newPos ) {
        Vector3 dir = newPos - lastPos;
        float dist = dir.magnitude;
        velocity = dir/Time.deltaTime;
        dir = Vector3.Normalize (dir);
        float errorMargin = 0.5f;
        body.position = lastPos - dir*errorMargin;
        // Push away players that get hit, and cause a collision.
        foreach (RaycastHit hit in body.SweepTestAll(dir,dist+errorMargin,QueryTriggerInteraction.Ignore)) {
            SourcePlayer p = hit.collider.gameObject.GetComponent<SourcePlayer> ();
            if (p == null) {
                continue;
            }
            float fraction = (hit.distance-errorMargin) / (dist+errorMargin);
            Vector3 bodyPositionAtHit = lastPos * (1 - fraction) + newPos * fraction;
            Vector3 diff = p.transform.position - bodyPositionAtHit;
            Vector3 pointDiff = hit.point - bodyPositionAtHit;
            p.transform.position = newPos + diff;
            p.HandleCollision (gameObject, -hit.normal, newPos + pointDiff);
        }
        body.position = newPos;
    }
    void Update () {
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
            progress = (Mathf.Clamp (Mathf.Sin (timer * 2f * Mathf.PI / CycleLength) * 1.5f, -1f, 1f) + 1f) / 2f;
            break;
        case MFunc.LinearZipper:
            float cyc2 = Helper.fmod (timer, CycleLength);
            float cyc2Progress = cyc2 / CycleLength;
            if (cyc2Progress < LinearZipperPauseFraction || cyc2Progress > 1f - LinearZipperPauseFraction) {
                progress = 0f;
            } else if (cyc2Progress >= LinearZipperPauseFraction && cyc2Progress <= 0.5f - LinearZipperPauseFraction) {
                progress = (cyc2Progress - LinearZipperPauseFraction) / (0.5f - LinearZipperPauseFraction * 2f);
            } else if (cyc2Progress >= 0.5f - LinearZipperPauseFraction && cyc2Progress <= 0.5f + LinearZipperPauseFraction) {
                progress = 1f;
            } else {
                progress = 1f - (cyc2Progress - (0.5f + LinearZipperPauseFraction)) / (0.5f - LinearZipperPauseFraction * 2f);
            }
            break;
        }
        Vector3 lastPosition = Target1.position * lastProgress + Target2.position * (1f - lastProgress);
        Vector3 newPosition = Target1.position * progress + Target2.position * (1f - progress);
        Move (lastPosition, newPosition);
        lastProgress = progress;
    }
    void Reset()
    {
        if (transform.parent == null)
        {
            GameObject newRoot = new GameObject(name + " Root");
            newRoot.transform.position = transform.position;
            transform.parent = newRoot.transform;
        }

        Target1 = transform.parent.Find("PathTarget 1");
        if(Target1 == null)
        {
            Object t1 = Resources.Load("EditorGizmos/PathTarget 1");
            Target1 = (Instantiate(t1, transform.position, Quaternion.identity, transform.parent) as GameObject).transform;
            Target1.gameObject.name = "PathTarget 1";
        }
        Target2 = transform.parent.Find("PathTarget 2");
        if (Target2 == null)
        {
            Object t2 = Resources.Load("EditorGizmos/PathTarget 2");
            Target2 = (Instantiate(t2, transform.position + 5 * Vector3.right, Quaternion.identity, transform.parent) as GameObject).transform;
            Target2.gameObject.name = "PathTarget 2";
            Target2.GetComponent<PlatformPathGizmo>().OtherTarget = Target1;
        }
    }
}
