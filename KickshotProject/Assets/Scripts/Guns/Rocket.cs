using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {
    private Rigidbody body;
    public List<GameObject> explosions;
    public GameObject decal;
    public GameObject owner = null;
    public float speed = 10f;
    public float power = 10f;
    public float damage = 50f;
    public float radius = 5f;
    public float inheritPercentage = .5f;
    public Vector3 inheritedVel;
    private bool exploded = false;
    private int layerMask;
    void Start() {
        body = GetComponent<Rigidbody> ();
        
		if (inheritedVel.magnitude > speed) {
			body.velocity = (transform.forward * inheritedVel.magnitude);
		} else {
			body.velocity = (inheritedVel * inheritPercentage + transform.forward * speed);
		}

        layerMask = Helper.GetLayerMask (gameObject);
        Destroy (gameObject, 15f);
    }
    void Update() {
        if (exploded) {
            return;
        }
        //if (body.velocity != (inheritedVel * inheritPercentage + transform.forward * speed)) {
        //    body.velocity += ((inheritedVel * inheritPercentage + transform.forward * speed) - body.velocity) * Time.deltaTime * 3f;
        //}
        //foreach (Collider col in Physics.OverlapSphere(transform.position,GetComponent<SphereCollider>().radius+0.01f,layerMask,QueryTriggerInteraction.Ignore)) {
            
        //}
    }
    void Explode( Vector3 position, Vector3 hitnormal ) {
        if (exploded) {
            return;
        }
        Vector3 perp = new Vector3 (-hitnormal.z, hitnormal.x, -hitnormal.y);
        Instantiate(decal,position,Quaternion.LookRotation(perp,hitnormal));
        Instantiate (explosions [(int)Random.Range (0, explosions.Count)], position, Quaternion.LookRotation(-hitnormal)*Quaternion.AngleAxis(Random.Range(0,360),new Vector3(0,0,1)));
        Destroy(gameObject.transform.Find("Trail").gameObject);
        Destroy(gameObject.transform.Find("rocket").gameObject);
        Destroy(gameObject.transform.Find("CloudTrail").gameObject,1f);
        gameObject.transform.Find ("CloudTrail").gameObject.GetComponent<ParticleSystem> ().Stop ();
        Destroy (gameObject.GetComponent<SphereCollider> ());
        Destroy (gameObject.GetComponent<Rigidbody> ());
        Destroy (gameObject.GetComponent<AudioSource> ());
        Destroy (gameObject, 1f);
        GameRules.RadiusDamage (damage, power, position, radius, true, owner);
        exploded = true;
    }
    void OnCollisionEnter( Collision other ) {
        if (exploded) {
            return;
        }
        if (other.contacts [0].otherCollider.gameObject.GetComponent<SourcePlayer> () != null) {
            Explode (transform.position, -transform.forward);
            return;
        }
        RaycastHit hit;
        Vector3 explosionPos;
        if (Physics.Raycast (transform.position, transform.forward, out hit, 2, layerMask, QueryTriggerInteraction.Ignore) && Vector3.Dot(transform.forward,transform.position-other.contacts[0].point) > 0f) {
            Explode (hit.point, hit.normal);
        } else {
            Explode (other.contacts [0].point, other.contacts [0].normal);
        }
    }
}
