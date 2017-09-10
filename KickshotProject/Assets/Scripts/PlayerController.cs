using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float KickSpeed;
    public float StartGravity;

    Transform _view_camera;

	// Use this for initialization
	void Start () {
        _view_camera = transform.GetChild(0);
        Physics.gravity = new Vector3(0.0f, -StartGravity, 0.0f);
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetButtonDown("Fire1"))
        {
            FireProjectile();
            ApplyKick();
        }
        if (Input.GetButtonUp("Jump"))
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }



    private void OnCollisionStay(Collision collision)
    {
        if(Input.GetButton("Jump"))
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;

        }
    }

    // Launches the projectile
    void FireProjectile()
    {
        RaycastHit bulletHit = new RaycastHit();
        if (Physics.Raycast(_view_camera.position, _view_camera.forward, out bulletHit))
            Debug.Log("Bullet Hit at:" + bulletHit.point);
    }

    // Applies the recoil to this GameObject's Rigidbody
    void ApplyKick()
    {
        Vector3 kickVel = -1 * _view_camera.forward * KickSpeed;
        Vector3 currentVel = GetComponent<Rigidbody>().velocity;

        if (Vector3.Dot(kickVel, currentVel) < 0)
            currentVel = Vector3.zero;

        GetComponent<Rigidbody>().velocity = currentVel + kickVel;
    }
}
