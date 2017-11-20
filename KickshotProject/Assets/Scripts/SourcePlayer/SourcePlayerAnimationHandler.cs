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
        Vector3 commandvel = player.wishDir;
		smoothcommand += (commandvel-smoothcommand) * Time.deltaTime * 10f;
		animator.SetBool ("grounded", player.groundEntity != null);
        animator.SetBool ("crouched", player.crouched);
		animator.SetFloat ("forward", smoothcommand.z);
		animator.SetFloat ("strafe", smoothcommand.x);
        Vector3 flatvel = new Vector3 (player.velocity.x, 0, player.velocity.z);
        if (commandvel.magnitude == 0) {
            flatvel = Vector3.zero;
        }
		animator.SetFloat ("speed", flatvel.magnitude/10f);
		if (player.justJumped) {
			animator.SetTrigger ("jump");
		}
		//if (player.justTookFallDamage) {
			//animator.SetTrigger ("FallDamage");
		//}
	}
}
