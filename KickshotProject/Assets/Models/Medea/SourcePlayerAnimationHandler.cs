using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourcePlayerAnimationHandler : MonoBehaviour {
	private SourcePlayer player;
	public Animator animator;
	Vector3 smoothcommand;
	// Use this for initialization
	void Start () {
		player = GetComponent<SourcePlayer> ();
	}

	// Update is called once per frame
	void Update () {
		Vector3 commandvel = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
		smoothcommand += (commandvel-smoothcommand) * Time.deltaTime * 10f;
		animator.SetBool ("isGrounded", player.groundEntity != null);
        animator.SetBool ("isCrouched", player.crouched || player.crouchTimer != 0f);
		animator.SetFloat ("Forward", smoothcommand.z);
		animator.SetFloat ("Strafe", smoothcommand.x);
		animator.SetFloat ("Speed", player.velocity.magnitude/10f);
		if (player.justJumped) {
			animator.SetTrigger ("Jump");
		}
		if (player.justTookFallDamage) {
			animator.SetTrigger ("FallDamage");
		}
	}
}
