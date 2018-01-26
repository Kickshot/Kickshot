using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
[RequireComponent(typeof(Animation))]
public class RopeSim : MonoBehaviour {
    public Transform start;
    public Transform end;
    public float boneDensity = 1f;
	public float timeStep = 1 / 60f;
	private float timeStepTimer = 0f;
    [HideInInspector]
    public List<Transform> bones = new List<Transform>();
    private List<Vector3> accel;
    private List<Vector3> vel;
	private List<bool> stuck;
	public bool sticky = false;
	[HideInInspector]
    public float distanceBetweenBones;
    public float strength = 100f;
	public bool staticStart = true;
	public bool staticEnd = true;
    public Vector3 gravity = new Vector3(0,-9.81f,0);
    public float damping = 0.05f;
    public float noise = 0.08f;
    public Vector3 windDirection = Vector3.forward;
    public float windAmount = 0.3f;
    public float maxVel = 100f;
	public float slack = -0.5f;
    public bool recordAnimation = false;
    public float settleTime = 2f;
    public float animationLength = 10f;
    public float animationFPS = 30f;
	public LayerMask collidesWith;
    private bool generated = false;
    public bool sane {
        get { return generated && !transform.hasChanged; }
    }
    private float stretchDistance;
    private float animationTimer;
    private bool savedAnimation = false;
    private AnimationClip clip;
    private List<List<Keyframe>> cx;
    private List<List<Keyframe>> cy;
    private List<List<Keyframe>> cz;
    void Awake() {
        generated = false;
        if ( start == null || end == null || start.position == end.position ) {
            return;
        }
        if ( recordAnimation ) {
            animationTimer = 0f;
            savedAnimation = false;
            clip = new AnimationClip();
            cx = new List<List<Keyframe>>();
            cy = new List<List<Keyframe>>();
            cz = new List<List<Keyframe>>();
        }
        transform.localScale = new Vector3( 1f, 1f, 1f );
        transform.position = Vector3.zero;
        foreach( Transform bone in bones ) {
            if ( bone != null && bone.gameObject != null ) {
                DestroyImmediate( bone.gameObject, true );
            }
        }
        bones.Clear();
        accel = new List<Vector3>();
        vel = new List<Vector3>();
		stuck = new List<bool>();
        int parts = (int)(Vector3.Distance(start.position, end.position)*boneDensity);
		if (parts == 0) {
			return;
		}
        distanceBetweenBones = Vector3.Distance(start.position, end.position)/parts;
        for( int i=0;i<=parts;i++ ) {
            Transform bone = new GameObject("Bone" + i).transform;
            bone.parent = transform;
            bone.localRotation = Quaternion.identity;
            bone.localPosition = new Vector3( 0, distanceBetweenBones*i, 0 );
            bones.Add( bone );
            accel.Add( Vector3.zero );
            vel.Add( Vector3.zero );
			stuck.Add (false);
            if ( recordAnimation ) {
                cx.Add( new List<Keyframe>() );
                cy.Add( new List<Keyframe>() );
                cz.Add( new List<Keyframe>() );
            }
        }
        transform.position = start.position;
        transform.up = Vector3.Normalize(end.position-start.position);
        transform.hasChanged = false;
        start.hasChanged = false;
        end.hasChanged = false;
		distanceBetweenBones += distanceBetweenBones * slack;
        generated = true;
    }
#if UNITY_EDITOR
    void OnDrawGizmosSelected() {
        if ( !sane ) {
            return;
        }
        for( int i=1;i<bones.Count;i++ ) {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(bones[i-1].position, bones[i].position);
        }
        Gizmos.DrawSphere(start.position,0.1f);
        Gizmos.DrawSphere(end.position,0.1f);
    }
#endif
    public void Regenerate() {
		generated = false;
		transform.hasChanged = true;
		Awake();
	}

    void Update() {
		if (timeStepTimer < timeStep && timeStep > 0f) {
			timeStepTimer += Time.deltaTime;
			return;
		}
		timeStepTimer -= timeStep;
#if UNITY_EDITOR
        if ( !EditorApplication.isPlaying ) {
            if ( transform.hasChanged || start.hasChanged || end.hasChanged ) {
                generated = false;
                transform.hasChanged = true;
                Awake();
                return;
            }
            if ( !generated ) {
                Awake();
            }
            return;
        }
#endif
        if (bones.Count <= 0) {
			return;
		}
        bones[0].position = start.position;
        bones[bones.Count-1].position = end.position;
        for( int i=0;i<bones.Count;i++ ) {
			if (stuck [i] && sticky) {
				continue;
			}
            // We don't move end bones
			Vector3 dir;
            accel[i] = gravity;
            vel[i] += accel[i]*Time.deltaTime;
            // Classic spring constraints
			if (i != 0) {
				dir = Vector3.Normalize (bones [i].position - bones [i - 1].position);
				if (dir.magnitude == 0) {
					dir = bones [i - 1].forward;
				}
				vel [i] += ((bones [i - 1].position + dir * distanceBetweenBones) - bones [i].position) * Time.deltaTime * strength;
			}
			if (i != bones.Count - 1) {
				dir = Vector3.Normalize (bones [i].position - bones [i + 1].position);
				if (dir.magnitude == 0) {
					dir = -bones [i + 1].forward;
				}
				vel [i] += ((bones [i + 1].position + dir * distanceBetweenBones) - bones [i].position) * Time.deltaTime * strength;
				bones [i].up = Vector3.Normalize (bones [i + 1].position - bones [i].position);
			} else {
				bones [i].up = -Vector3.Normalize (bones [i - 1].position - bones [i].position);
			}
            vel[i] += new Vector3( Random.Range(-noise,noise),Random.Range(-noise,noise),Random.Range(-noise,noise) );
            vel[i] += windDirection * windAmount * 0.6f * (Mathf.Sin(Time.time*1.4f)+1f)/2f;
            vel[i] += windDirection * windAmount * 0.2f * (Mathf.Sin(Time.time*5f)+1f)/2f;
            vel[i] += windDirection * windAmount * 0.2f * Mathf.Sin(Time.time*4f);
            vel[i] -= vel[i]*damping;
            if ( vel[i].magnitude > maxVel ) {
                vel[i] = Vector3.Normalize(vel[i])*maxVel;
            }
        }
			
        for( int i=0;i<bones.Count;i++ ) {
            animationTimer += Time.deltaTime;
            if ( recordAnimation && animationTimer > 1f/animationFPS && Time.time > settleTime && Time.time < animationLength+settleTime ) {
                animationTimer = 0f;
                cx[i].Add(new Keyframe(Time.time, bones[i].localPosition.x));
                cy[i].Add(new Keyframe(Time.time, bones[i].localPosition.y));
                cz[i].Add(new Keyframe(Time.time, bones[i].localPosition.z));
            }
			if (stuck [i] && sticky) {
				continue;
			}
			if (i == 0 && staticStart || i == bones.Count-1 && staticEnd) {
				continue;
			}
			bones [i].position += vel [i] * Time.deltaTime;
        }
		start.position = bones [0].position;
		end.position = bones [bones.Count - 1].position;

		if (sticky) {
			for (int i = 0; i < bones.Count; i++) {		
				if (stuck [i]) { 
					continue;
				}
				foreach (Collider col in Physics.OverlapSphere(bones[i].position,0.05f,collidesWith,QueryTriggerInteraction.Ignore)) {
					stuck [i] = true;
					break;
				}
			}
		}

        if ( recordAnimation && Time.time > animationLength+settleTime && !savedAnimation ) {
            savedAnimation = true;
            clip.legacy = true;
            for ( int i=0;i<bones.Count;i++ ) {
                clip.SetCurve("Bone"+i, typeof(Transform), "localPosition.x", new AnimationCurve(cx[i].ToArray()));
                clip.SetCurve("Bone"+i, typeof(Transform), "localPosition.y", new AnimationCurve(cy[i].ToArray()));
                clip.SetCurve("Bone"+i, typeof(Transform), "localPosition.z", new AnimationCurve(cz[i].ToArray()));
            }
            clip.wrapMode = WrapMode.PingPong;
            clip.name = gameObject.name;
            clip.frameRate = 30;
            AssetDatabase.CreateAsset(clip, "Assets/"+gameObject.name+".anim");
            AssetDatabase.SaveAssets();
			Animation ani = GetComponent<Animation> ();
			ani.clip = clip;
        }
        transform.hasChanged = false;
    }
}
