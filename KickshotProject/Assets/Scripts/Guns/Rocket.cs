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
    public float radius = 5f;
    public float inheritPercentage = .5f;
    public Vector3 inheritedVel;
    private bool exploded = false;
    private int layerMask;
    void Start() {
        body = GetComponent<Rigidbody> ();
        body.velocity = (inheritedVel*inheritPercentage + transform.forward * speed);
        layerMask = Helper.GetLayerMask (gameObject);
        Destroy (gameObject, 15f);
    }
    void Update() {
        if (exploded) {
            return;
        }
        if (body.velocity != (inheritedVel * inheritPercentage + transform.forward * speed)) {
            body.velocity += ((inheritedVel * inheritPercentage + transform.forward * speed) - body.velocity) * Time.deltaTime * 3f;
        }
    }
    void OnCollisionEnter( Collision other ) {
        if (exploded) {
            return;
        }
        RaycastHit hit;
        Vector3 explosionPos;
        int rand = (int)Random.Range (0, explosions.Count);
        if (Physics.Raycast (transform.position, transform.forward, out hit, 2, layerMask, QueryTriggerInteraction.Ignore) && Vector3.Dot(transform.forward,transform.position-other.contacts[0].point) > 0f) {
            explosionPos = hit.point;
            Vector3 perp = new Vector3 (-hit.normal.z, hit.normal.x, -hit.normal.y);
            Instantiate(decal,explosionPos-hit.normal,Quaternion.LookRotation(perp,hit.normal));
            Instantiate (explosions [rand], explosionPos, Quaternion.LookRotation(-hit.normal)*Quaternion.AngleAxis(Random.Range(0,360),new Vector3(0,0,1)));
        } else {
            explosionPos = other.contacts [0].point;
            Vector3 perp = new Vector3 (-other.contacts [0].normal.z, other.contacts [0].normal.x, -other.contacts [0].normal.y);
            Instantiate(decal,other.contacts [0].point - other.contacts[0].normal, Quaternion.LookRotation(perp,other.contacts[0].normal));
            //d.transform.SetParent (other.contacts [0].otherCollider.gameObject.transform);
            Instantiate (explosions [rand], explosionPos, Quaternion.LookRotation(-other.contacts [0].normal)*Quaternion.AngleAxis(Random.Range(0,360),new Vector3(0,0,1)));
        }
        Destroy(gameObject.transform.Find("Trail").gameObject);
        Destroy(gameObject.transform.Find("rocket").gameObject);
        Destroy(gameObject.transform.Find("CloudTrail").gameObject,1f);
        gameObject.transform.Find ("CloudTrail").gameObject.GetComponent<ParticleSystem> ().Stop ();
        Destroy (gameObject.GetComponent<SphereCollider> ());
        Destroy (gameObject.GetComponent<Rigidbody> ());
        Destroy (gameObject.GetComponent<AudioSource> ());
        Destroy (gameObject, 1f);
        GameRules.RadiusDamage (100f, power, explosionPos, radius, true, owner);
        exploded = true;
    }
}
