using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float KickSpeed;
    public float StartGravity;
    public float ChargeSpeed;
    public int ForceStyle;

    Transform _view_camera;

    float m_charge;

	// Use this for initialization
	void Start () {
        m_charge = 0;

        _view_camera = transform.GetChild(0);
        Physics.gravity = new Vector3(0.0f, -StartGravity, 0.0f);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            FireProjectile();
            ApplyKick();
        }
        if (Input.GetButton("Fire2"))
        {
            ChargeWeapon();
        }
        else
        {
            ApplyKick(m_charge);
            m_charge = 0;
        }
            if (Input.GetButtonUp("Jump"))
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

    private void ChargeWeapon()
    {
        m_charge += ChargeSpeed * Time.deltaTime;
    }

    private void OnCollisionStay(Collision collision)
    {
        if(Input.GetButton("Jump") && collision.collider.tag != "Ground")
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

        if(ForceStyle == 0)
            GetComponent<Rigidbody>().velocity = currentVel + kickVel;
        else
            GetComponent<Rigidbody>().AddForce(kickVel, ForceMode.Impulse);
    }

    // Overloads ApplyKick with a custom 'Speed'
    void ApplyKick(float Speed)
    {
        Vector3 kickVel = -1 * _view_camera.forward * Speed;
        Vector3 currentVel = GetComponent<Rigidbody>().velocity;

        if (Vector3.Dot(kickVel, currentVel) < 0)
            currentVel = Vector3.zero;

        if (ForceStyle == 0)
            GetComponent<Rigidbody>().velocity = currentVel + kickVel;
        else
            GetComponent<Rigidbody>().AddForce(kickVel, ForceMode.Impulse);
    }
}
