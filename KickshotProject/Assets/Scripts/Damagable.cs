using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour {
    public float health = 100f;
    public void Damage( float amount ) {
        health -= amount;
        if (health < 0) {
            Destroy (gameObject);
        }
    }
}
