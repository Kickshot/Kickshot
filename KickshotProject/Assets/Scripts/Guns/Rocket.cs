using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {
    private Rigidbody body;
    public List<GameObject> explosions;
    public GameObject decal;
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
    }
    void Update() {
        //body.velocity = (inheritedVel*inheritPercentage + transform.forward * speed);
    }
    void OnCollisionEnter( Collision other ) {
        if (exploded) {
            return;
        }
        RaycastHit hit;
        Vector3 explosionPos;
        int rand = (int)Random.Range (0, explosions.Count);
        if (Physics.Raycast (transform.position, transform.forward, out hit, 2, layerMask, QueryTriggerInteraction.Ignore)) {
            explosionPos = hit.point;
            GameObject d = Instantiate(decal,explosionPos,Quaternion.LookRotation(-hit.normal)*Quaternion.AngleAxis(Random.Range(0,360),new Vector3(0,0,1)));
            //d.transform.SetParent (hit.transform);
            //Helper.DrawLine (hit.point, hit.point + hit.normal, Color.red, 10f);
            Instantiate (explosions [rand], explosionPos, Quaternion.LookRotation(-hit.normal)*Quaternion.AngleAxis(Random.Range(0,360),new Vector3(0,0,1)));
        } else {
            explosionPos = other.contacts [0].point;
            GameObject d = Instantiate(decal,explosionPos,Quaternion.LookRotation(-other.contacts [0].normal)*Quaternion.AngleAxis(Random.Range(0,360),new Vector3(0,0,1)));
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
        GameRules.RadiusDamage (100f, power, explosionPos, radius, true);
    }
}
