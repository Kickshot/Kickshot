using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SourcePlayerAnimationHandler : MonoBehaviour {
	public SourcePlayer player;
	private Animator animator;
    public Transform view;
    float smoothIKWeight;
	Vector3 smoothcommand;
	// Use this for initialization
	void Start () {
        animator = GetComponent <Animator>();
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
    void OnAnimatorIK() {
        // Set the look target position, if one has been assigned
        if (view != null) {
            animator.SetLookAtWeight (1);
            animator.SetLookAtPosition (view.position + view.forward);
        }
        if (animator && player.groundEntity != null && (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("Crouch Idle")) && !animator.IsInTransition(0)) {
            RaycastHit hit;
            if (Physics.Raycast (animator.GetBoneTransform (HumanBodyBones.RightFoot).position+Vector3.up, Vector3.down, out hit, 2f, 1 << LayerMask.NameToLayer ("Default"), QueryTriggerInteraction.Ignore)) {
                animator.SetIKPositionWeight (AvatarIKGoal.RightFoot, smoothIKWeight);
                animator.SetIKRotationWeight (AvatarIKGoal.RightFoot, smoothIKWeight);
                animator.SetIKPosition (AvatarIKGoal.RightFoot, hit.point+hit.normal*0.14f);
                animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.FromToRotation(transform.up, hit.normal)*Quaternion.AngleAxis(45f,Vector3.up)*transform.rotation);
            }
            if (Physics.Raycast (animator.GetBoneTransform (HumanBodyBones.LeftFoot).position+Vector3.up, Vector3.down, out hit, 2f, 1 << LayerMask.NameToLayer ("Default"), QueryTriggerInteraction.Ignore)) {
                animator.SetIKPositionWeight (AvatarIKGoal.LeftFoot, smoothIKWeight);
                animator.SetIKRotationWeight (AvatarIKGoal.LeftFoot, smoothIKWeight);
                animator.SetIKPosition (AvatarIKGoal.LeftFoot, hit.point+hit.normal*0.14f);
                animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.FromToRotation(transform.up, hit.normal)*Quaternion.AngleAxis(20f,Vector3.up)*transform.rotation);
            }
            smoothIKWeight += (1 - smoothIKWeight) * Time.deltaTime * 10f;
        } else {
            smoothIKWeight += (0 - smoothIKWeight) * Time.deltaTime * 10f;
            animator.SetIKRotationWeight (AvatarIKGoal.RightFoot, smoothIKWeight);
            animator.SetIKPositionWeight (AvatarIKGoal.RightFoot, smoothIKWeight);
            animator.SetIKPositionWeight (AvatarIKGoal.LeftFoot, smoothIKWeight);
            animator.SetIKRotationWeight (AvatarIKGoal.LeftFoot, smoothIKWeight);
        }
    }
}
