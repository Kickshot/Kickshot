/**/﻿

using UnityEngine;
using System.Collections;

public class CPMPlayer : MonoBehaviour {
	public Transform  View;
	public Vector3 velocity;
	public Vector3 ViewOffset = new Vector3(0f,0.6f,0f);
	public float xMouseSensitivity = 30.0f;
	public float yMouseSensitivity = 30.0f;
	public float gravity  = 20.0f;
	public float friction  = 6f;
	public float moveSpeed = 7f;
	public float GroundAcceleration = 14f;
	public float GroundDeacceleration = 10f;
	public float AirAcceleration = 10f;
	public float jumpSpeed = 8.0f;

	private float distToGround;
	private float Overclip = 1.001f;
	private CharacterController controller;
	private Vector3 groundNormal;
	private float rotX;
	private float rotY;

	void Start() {
		controller = GetComponent<CharacterController> ();
		distToGround = GetComponent<Collider>().bounds.extents.y - GetComponent<Collider>().bounds.center.y;
	}

	void Update() {
		if (Cursor.lockState == CursorLockMode.None) {
			if (Input.GetMouseButtonDown (0)) {
				Cursor.lockState = CursorLockMode.Locked;
			}
		}
		rotX -= Input.GetAxis("Mouse Y") * xMouseSensitivity * 0.02f;
		rotY += Input.GetAxis("Mouse X") * yMouseSensitivity * 0.02f;
		if (rotX < -90) {
			rotX = -90;
		} else if (rotX > 90) {
			rotX = 90;
		}
		this.transform.rotation = Quaternion.Euler(0, rotY, 0); // Rotates the collider
		View.rotation = Quaternion.Euler(rotX, rotY, 0); // Rotates the camera

		Vector3 command = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));

		RaycastHit hit;
		if (Physics.Raycast(transform.position, -transform.up, out hit, distToGround + 0.2f)){
			groundNormal = hit.normal;
		}
		if (controller.isGrounded) {
			WalkMove (command);
		} else {
			AirMove (command);
		}
		controller.Move (velocity * Time.deltaTime);
		View.position = transform.position+ViewOffset;
	}

	// Slide off of impacting surface
	private Vector3 ClipVelocity( Vector3 vel, Vector3 impactNormal, float overbounce ) {
		float backoff;
		backoff = Vector3.Dot (vel, impactNormal);

		if ( backoff < 0 ) {
			backoff *= overbounce;
		} else {
			backoff /= overbounce;
		}
		return vel - (impactNormal*backoff);
	}

	// Handles friction
	private void Friction() {
		float	newspeed, control;

		Vector3 vel = new Vector3(velocity.x,0,velocity.z);
		float speed = vel.magnitude;

		float drop = 0;
		// apply ground friction
		if (controller.isGrounded) {
			control = speed < GroundDeacceleration ? GroundDeacceleration : speed;
			drop += control * friction * Time.deltaTime;
		}

		// scale the velocity
		newspeed = speed - drop;
		if (newspeed < 0) {
			newspeed = 0;
		}
		if (speed > 0) {
			newspeed /= speed;
		}

		velocity.x *= newspeed;
		velocity.z *= newspeed;
	}

	// Handles user intended acceleration
	private void Accelerate( Vector3 wishdir, float wishspeed, float accel ) {
		float addspeed, accelspeed, currentspeed;

		currentspeed = Vector3.Dot (velocity, wishdir);
		addspeed = wishspeed - currentspeed;
		if (addspeed <= 0) {
			return;
		}
		accelspeed = accel*Time.deltaTime*wishspeed;
		if (accelspeed > addspeed) {
			accelspeed = addspeed;
		}

		velocity.x += accelspeed * wishdir.x;
		velocity.z += accelspeed * wishdir.z;
	}

	// Prevent players from suffering sqrt(2) distortions in speed.
	private float CursePythagoras(Vector3 command)
	{
		int max;
		float total;
		float scale;

		max = (int)Mathf.Abs(command.z);
		if(Mathf.Abs(command.x) > max)
			max = (int)Mathf.Abs(command.x);
		if(max <= 0)
			return 0;

		total = Mathf.Sqrt(command.z * command.z + command.x * command.x);
		scale = moveSpeed * max / total;

		return scale;
	}

	// Handles air-borne movement.
	private void AirMove(Vector3 command) {
		Vector3		wishvel;
		float		fmove, smove;
		Vector3		wishdir;
		float		wishspeed;
		float		scale;

		Friction();

		fmove = command.z;
		smove = command.x;
		scale = CursePythagoras( command );

		// project moves down to flat plane
		Vector3 forward = transform.forward;
		forward.y = 0;
		Vector3 right = transform.right;
		right.y = 0;
		right = Vector3.Normalize (right);
		forward = Vector3.Normalize (forward);

		wishvel = forward * fmove + right * smove;
		wishvel.y = 0;

		wishdir = new Vector3(wishvel.x,wishvel.y,wishvel.z);
		wishspeed = wishdir.magnitude;
		wishspeed *= scale;

		// not on ground, so little effect on velocity
		Accelerate (wishdir, wishspeed, AirAcceleration);

		// we may have a ground plane that is very steep, even
		// though we don't have a groundentity
		// slide along the steep plane
		//if ( pml.groundPlane ) {
			//ClipVelocity (pm->ps->velocity, pml.groundTrace.plane.normal, pm->ps->velocity, OVERCLIP );
		//}

		velocity.y -= gravity * Time.deltaTime;
	}

	// Handles jumping.
	private bool CheckJump() {
		if (controller.isGrounded && Input.GetButtonDown ("Jump")) {
			velocity.y = jumpSpeed;
			return true;
		}
		return false;
	}

	// Handle walking/running on the ground.
	private void WalkMove( Vector3 command ) {
		Vector3		wishvel;
		float		fmove, smove;
		Vector3		wishdir;
		float		wishspeed;
		float		scale;
		float		vel;

		if ( CheckJump() ) {
			AirMove(command);
			return;
		}

		Friction ();

		fmove = command.z;
		smove = command.x;
		scale = CursePythagoras( command );

		// project moves down to flat plane
		Vector3 forward = transform.forward;
		forward.y = 0;
		Vector3 right = transform.right;
		right.y = 0;
		right = Vector3.Normalize (right);
		forward = Vector3.Normalize (forward);

		// project the forward and right directions onto the ground plane
		forward = ClipVelocity (forward, groundNormal, Overclip );
		right = ClipVelocity (right, groundNormal, Overclip );

		forward = Vector3.Normalize (forward);
		right = Vector3.Normalize (right);

		wishvel = forward * fmove + right * smove;

		wishdir = new Vector3 (wishvel.x,wishvel.y,wishvel.z);
		wishspeed = wishdir.magnitude;
		wishspeed *= scale;

		Accelerate (wishdir, wishspeed, GroundAcceleration);

		vel = velocity.magnitude;

		// slide along the ground plane
		velocity = ClipVelocity (velocity, groundNormal, Overclip );

		// don't decrease velocity when going up or down a slope
		velocity = Vector3.Normalize(velocity)*vel;
	}
}
