using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatterWall : MonoBehaviour
{
    [SerializeField]
    private GameObject ShatteredWallObj;
    [SerializeField]
    private float CenterToObjVelScale;
    [SerializeField]
    private float ForwardVelScale;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Projectile")
        {
            Rigidbody projectileRigid = collision.gameObject.GetComponent<Rigidbody>();

            if(ShatteredWallObj != null)
            {
                GameObject wall = Instantiate(ShatteredWallObj, this.transform.position, this.transform.rotation) as GameObject;
                Rigidbody[] allChildren = wall.GetComponentsInChildren<Rigidbody>();
           
                foreach (Rigidbody child in allChildren)
                {
                    float angle = Vector3.Dot(child.transform.position.normalized, collision.contacts[0].point.normalized);
                    
                    child.velocity = ((child.transform.position.normalized - collision.contacts[0].point.normalized)* CenterToObjVelScale + (collision.gameObject.transform.forward)* angle * ForwardVelScale);
                }
                Destroy(gameObject);
            }
        }
    }
}
