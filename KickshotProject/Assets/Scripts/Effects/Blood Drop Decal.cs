using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodDropDecal : MonoBehaviour {

    public GameObject decal;
    public ParticleSystem bloodExplosion;

    List<ParticleCollisionEvent> collisionEvents;

	// Use this for initialization
	void Start () {
        collisionEvents = new List<ParticleCollisionEvent>();
	}

    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(bloodExplosion, other, collisionEvents);

        for (int i = 0; i < collisionEvents.Count; i++)
        {
            Decal (collisionEvents[i]);
        }

    }

    void Decal(ParticleCollisionEvent particleCollisionEvent)
    {
        Instantiate(decal, particleCollisionEvent.intersection, Quaternion.LookRotation(particleCollisionEvent.normal));
    }

    // Update is called once per frame
    void Update () {

	}
    //
    //void Decal(Vector3 position, Vector3 hitnormal)
    //{
       // if (collided)
        //{
          //  return;
        //}
        //Vector3 perp = new Vector3(-hitnormal.z, hitnormal.x, -hitnormal.y);
        //Instantiate(decal, position, Quaternion.LookRotation(perp, hitnormal));
    //}
}
