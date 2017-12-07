using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilGun : GunBase {
    private AudioSource blam;
    public float strength;
    void Start() {
        blam = GetComponent<AudioSource> ();
    }
    override public void Update() {
        base.Update ();
        if (!equipped) {
            return;
        }
        transform.rotation = view.rotation;
    }
    public override void OnPrimaryFire() {

		blam.Play();

		if (player.wallRunning)
		{
			float mag = Vector3.ProjectOnPlane(player.velocity, Vector3.up).magnitude;
			Vector3 newdir = Vector3.ProjectOnPlane(view.forward, Vector3.up).normalized;

			player.velocity = newdir * mag + new Vector3(0, player.velocity.y, 0);
		}
		player.velocity += -1 * view.forward * strength + view.up * strength/2f;

		// Old primary fire.
        /*blam.Play ();
        
        player.velocity -= view.forward * strength + view.up * strength/2f;
        */
    }
    public override void OnSecondaryFire() {
        // Old Secondary fire.
        blam.Play();

        if (player.wallRunning)
        {
            float mag = Vector3.ProjectOnPlane(player.velocity, Vector3.up).magnitude;
            Vector3 newdir = Vector3.ProjectOnPlane(view.forward, Vector3.up).normalized;

            player.velocity = newdir * mag + new Vector3(0, player.velocity.y, 0);
        }
        player.velocity += view.forward * strength + view.up * strength / 2f;
    }
}
