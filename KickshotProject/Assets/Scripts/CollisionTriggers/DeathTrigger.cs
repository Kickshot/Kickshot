using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        other.SendMessage ("Damage", 99999, SendMessageOptions.DontRequireReceiver);
    }
    void OnCollisionEnter(Collision other)
    {
        other.gameObject.SendMessage ("Damage", 99999, SendMessageOptions.DontRequireReceiver);
    }
}
