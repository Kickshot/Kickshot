using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameRules {
    public static void RadiusDamage( float damage, float knockBack, Vector3 vecSrcIn, float radius, bool ignoreWorld ) {
        float        adjustedDamage, falloff;
        Vector3      vecSpot;
        Vector3      vecToTarget;
        Vector3      vecEndPos;

        Vector3 vecSrc = vecSrcIn;

        if (radius != 0f) {
            falloff = damage / radius;
        } else {
            falloff = 1.0f;
        }

        //vecSrc.z += 1;// in case grenade is lying on the ground

        // iterate on all entities in the vicinity.
        foreach ( Collider other in Physics.OverlapSphere( vecSrc, radius ) ) {
            if (other.isTrigger) {
                continue;
            }

            // radius damage can only be blocked by the world
            if (other is CapsuleCollider || other is BoxCollider || other is SphereCollider) {
                vecSpot = other.ClosestPoint (vecSrc);
            } else {
                vecSpot = other.ClosestPointOnBounds (vecSrc);
            }

            bool bHit = false;

            if( ignoreWorld ) {
                vecEndPos = vecSpot;
                bHit = true;
            } else {
                RaycastHit hit;
                bool inSight = Physics.Raycast (vecSrc, Vector3.Normalize (vecSpot - vecSrc), out hit, (vecSpot - vecSrc).magnitude + 0.1f);

                vecEndPos = hit.point;

                if( inSight && hit.collider == other ) {
                    bHit = true;
                }
            }

            if ( bHit ) {
                // the explosion can 'see' this entity, so hurt them!
                //vecToTarget = ( vecSrc - vecEndPos );
                vecToTarget = ( vecEndPos - vecSrc );

                // decrease damage for an ent that's farther from the bomb.
                adjustedDamage = vecToTarget.magnitude * falloff;
                adjustedDamage = damage - adjustedDamage;

                if ( adjustedDamage > 0f ) {

                    Vector3 dir = Vector3.Normalize (vecToTarget);

                    // Assume the force passed in is the maximum force. Decay it based on falloff.
                    float flForce = knockBack * falloff;
                    Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
                    if (rb != null) {
                        rb.AddForceAtPosition (flForce * dir, vecEndPos);
                    }
                    SourcePlayer player = other.gameObject.GetComponent<SourcePlayer>();
                    if (player != null) {
                        player.velocity += flForce * dir;
                        player.StunAirBrake (0.25f);
                    }

                    //pEntity->TakeDamage( adjustedInfo );
                }
            }
        }
    }
}
