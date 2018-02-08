using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravLift : MonoBehaviour
{

    public Vector3 Velocity;
    public float Cooldown;

    public bool NoCoolDown;
    public bool AdditiveVelocity;

    private float HitTime;
    // Use this for initialization
    void Start()
    {
        HitTime = Cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (HitTime <= Cooldown && !NoCoolDown)
        {
            HitTime += Time.deltaTime;
        }

    }

    void OnTriggerStay(Collider other)
    {
        if (NoCoolDown)
            return;

        bool AllowedToJump = HitTime >= Cooldown;
        print(AllowedToJump);
        if (AllowedToJump)
        {
            SourcePlayer player = other.gameObject.GetComponent<SourcePlayer>();
            if (AdditiveVelocity)
                player.velocity += Velocity;
            else
                player.velocity = Velocity;
            HitTime = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (NoCoolDown)
        {
            SourcePlayer player = other.gameObject.GetComponent<SourcePlayer>();
            if(AdditiveVelocity)
                player.velocity += Velocity;
            else
                player.velocity = Velocity;
        }
    }
}