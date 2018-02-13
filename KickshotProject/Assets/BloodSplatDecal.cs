using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatDecal : MonoBehaviour
{

    public GameObject decal;
    public ParticleSystem bloodExplosion;

    List<ParticleCollisionEvent> collisionEvents;

    // Use this for initialization
    void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(bloodExplosion, other, collisionEvents);

        for (int i = 0; i < collisionEvents.Count; i++)
        {
            PaintDecal(collisionEvents[i]);
        }

    }

    void PaintDecal(ParticleCollisionEvent particleCollisionEvent)
    {
        GameObject bloodDecal = Instantiate(decal, particleCollisionEvent.intersection, Quaternion.LookRotation(particleCollisionEvent.normal));
        bloodDecal.transform.up = particleCollisionEvent.normal;
    }

    // Update is called once per frame
    void Update()
    {

    }

}
