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
    void Start() {
        body = GetComponent<Rigidbody> ();
        body.velocity = (inheritedVel*inheritPercentage + transform.forward * speed);
    }
    void Update() {
        //body.velocity = (inheritedVel*inheritPercentage + transform.forward * speed);
    }
    void OnCollisionEnter( Collision other ) {
        RaycastHit hit;
        Vector3 explosionPos;
        if (Physics.Raycast (transform.position, transform.forward, out hit, 2f)) {
            explosionPos = hit.point;
            Instantiate(decal,explosionPos-hit.normal,Quaternion.LookRotation(-hit.normal));
            //Helper.DrawLine (hit.point, hit.point + hit.normal, Color.red, 10f);
        } else {
            explosionPos = other.contacts [0].point;
            Instantiate(decal,explosionPos-other.contacts[0].normal,Quaternion.LookRotation(-other.contacts[0].normal));
        }
        int rand = (int)Random.Range (0, explosions.Count);
        Instantiate (explosions [rand], explosionPos, Quaternion.LookRotation(-hit.normal));//Quaternion.LookRotation(other.contacts[0].normal));
        Destroy(gameObject);
        GameRules.RadiusDamage (100f, power, explosionPos, radius, true);
    }
}
