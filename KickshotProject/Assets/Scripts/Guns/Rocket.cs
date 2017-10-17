using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {
    private Rigidbody body;
    public List<GameObject> explosions;
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
        int rand = (int)Random.Range (0, explosions.Count);
        Instantiate (explosions [rand], transform.position, Quaternion.identity);//Quaternion.LookRotation(other.contacts[0].normal));
        Vector3 explosionPos = other.contacts[0].point;
        Destroy(gameObject);
        foreach (Collider hit in Physics.OverlapSphere(explosionPos, radius)) {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.AddExplosionForce (power, explosionPos, radius, 3.0F);
            }
            SourcePlayer player = hit.GetComponent<SourcePlayer>();
            if (player != null) {
                Vector3 direction = player.transform.position - explosionPos;
                player.velocity += Vector3.Normalize(direction) * power;
            }
        }
    }
}
