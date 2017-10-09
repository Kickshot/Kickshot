// Mostly this is built directly from [source-sdk-2013](https://github.com/ValveSoftware/source-sdk-2013/blob/56accfdb9c4abd32ae1dc26b2e4cc87898cf4dc1/sp/src/game/shared/gamemovement.cpp)
// Though there's quite a few edits or adjustments to make it work with unity's character controller.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourcePlayer : MonoBehaviour {
	public AudioClip jumpGrunt;
	public AudioClip painGrunt;
	public Vector3 velocity;
	public Transform  view;
	public Vector3 viewOffset = new Vector3(0f,0.6f,0f);
	public float xMouseSensitivity = 30.0f;
	public float yMouseSensitivity = 30.0f;
	public Vector3 gravity = new Vector3(0,-20f,0);
	public float baseFriction = 6f;
	public float maxSpeed = 30f;
	public float groundAccelerate = 10f;
	public float groundDecellerate = 10f;
	public float airAccelerate = 1f;
	public float airDeccelerate = 10f;
	public float walkSpeed = 10f;
	public float jumpSpeed = 8f;
	public float fallPunchThreshold = 8f;
	public float maxSafeFallSpeed = 15f;

	private float lastGrunt;
	private float stepSize = 0.5f;
	private float rotX;
	private float rotY;
	private float fallVelocity;
	private CharacterController controller;
	private GameObject groundEntity = null;
	private Vector3 groundNormal = new Vector3(0f,1f,0f);
	private Vector3 groundVelocity;
	private float groundFriction;
	private float distToGround;
	private float radius;
	void Start() {
		controller = GetComponent<CharacterController> ();
		distToGround = GetComponent<Collider>().bounds.extents.y/2f;
		radius = controller.radius;
		stepSize = controller.stepOffset;
	}
	void Update() {
		if (Cursor.lockState == CursorLockMode.None) {
			if (Input.GetMouseButtonDown (0)) {
				Cursor.lockState = CursorLockMode.Locked;
			}
		}
		rotX -= Input.GetAxis("Mouse Y") * xMouseSensitivity * 0.02f;
		rotY += Input.GetAxis("Mouse X") * yMouseSensitivity * 0.02f;
		if (rotX < -90f) {
			rotX = -90f;
		} else if (rotX > 90f) {
			rotX = 90f;
		}
		this.transform.rotation = Quaternion.Euler(0, rotY, 0); // Rotates the collider
		view.rotation = Quaternion.Euler(rotX, rotY, 0); // Rotates the camera

		RaycastHit hit;
		if (Physics.SphereCast (transform.position+controller.center, radius, -transform.up, out hit, distToGround + 0.1f)) {
			// Snap the player to where the spherecast hit.
			groundEntity = hit.collider.gameObject;
			groundNormal = hit.normal;
			GroundEntity check = groundEntity.GetComponent<GroundEntity> ();
			if (check != null) {
				groundVelocity = check.velocity;
				groundFriction = check.frictionMultiplier;
			} else {
				groundFriction = 1f;
				groundVelocity = new Vector3 (0f, 0f, 0f);
			}
		} else {
			groundEntity = null;
			groundFriction = 1f;
			groundNormal = new Vector3 (0f, 1f, 0f);
			//groundVelocity = new Vector3 (0f, 0f, 0f);
		}

		PlayerMove();
	}
	// Slide off of impacting surface
	private Vector3 ClipVelocity( Vector3 vel, Vector3 impactNormal) {
		float backoff;
		backoff = Vector3.Dot (vel, impactNormal);

		if ( backoff < 0 ) {
			backoff *= 1.001f;
		} else {
			backoff /= 1.001f;
		}
		return vel - (impactNormal*backoff);
	}
	// This command, in a nutshell, scales player input in order to take into account sqrt(2) distortions
	// from walking diagonally. It also multiplies the answer by the walkspeed for convenience.
	private Vector3 GetCommandVelocity() {
		float max;
		float total;
		float scale;
		Vector3 command = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));

		max = Mathf.Max (Mathf.Abs(command.z), Mathf.Abs(command.x));
		if (max <= 0) {
			return new Vector3 (0f, 0f, 0f);
		}

		total = Mathf.Sqrt(command.z * command.z + command.x * command.x);
		scale = max / total;

		return command*scale*walkSpeed;
	}
	// Ask valve why they split up the gravity calls like this
	private void StartGravity() {
		velocity += gravity * Time.deltaTime * 0.5f;
	}
	private void FinishGravity() {
		velocity += gravity * Time.deltaTime * 0.5f;
	}
	private void CheckJumpButton() {
		if (groundEntity && Vector3.Angle(groundNormal,new Vector3(0f,1f,0f)) < controller.slopeLimit ) {
			if (Time.time - lastGrunt > 0.3) {
				AudioSource.PlayClipAtPoint (jumpGrunt, transform.position);
				lastGrunt = Time.time;
			}
			velocity.y = jumpSpeed;
			groundEntity = null;
		}
		//TODO: Gotta implement forward speed bonuses: https://github.com/ValveSoftware/source-sdk-2013/blob/56accfdb9c4abd32ae1dc26b2e4cc87898cf4dc1/sp/src/game/shared/gamemovement.cpp#L2468
	}
	private void CheckFalling() {
		//Debug.Log (fallVelocity);
		// this function really deals with landing, not falling, so early out otherwise
		if ( !controller.isGrounded || fallVelocity <= 0f )
			return;

		if ( fallVelocity >= fallPunchThreshold ) {
			bool bAlive = true;
			float fvol = 0.5f;

			// Scale it down if we landed on something that's floating...
			//if ( player->GetGroundEntity()->IsFloating() ) {
			//	player->m_Local.m_flFallVelocity -= PLAYER_LAND_ON_FLOATING_OBJECT;
			//}

			//
			// They hit the ground.
			//
			//if( player->GetGroundEntity()->GetAbsVelocity().z < 0.0f )
			//{
				// Player landed on a descending object. Subtract the velocity of the ground entity.
			//	player->m_Local.m_flFallVelocity += player->GetGroundEntity()->GetAbsVelocity().z;
			//	player->m_Local.m_flFallVelocity = MAX( 0.1f, player->m_Local.m_flFallVelocity );
			//}

			if ( fallVelocity > maxSafeFallSpeed ) {
				//
				// If they hit the ground going this fast they may take damage (and die).
				//
				//bAlive = MoveHelper( )->PlayerFallingDamage();
				AudioSource.PlayClipAtPoint (painGrunt, transform.position);
				GetComponent<Damagable>().Damage( (fallVelocity - maxSafeFallSpeed)*5f );
				fvol = 1.0f;
			} else if ( fallVelocity > maxSafeFallSpeed / 2 ) {
				fvol = 0.85f;
			} else {
				fvol = 0f;
			}

			// PlayerRoughLandingEffects( fvol );

			//if (bAlive) {
			//	MoveHelper( )->PlayerSetAnimation( PLAYER_WALK );
			//}
		}

		// let any subclasses know that the player has landed and how hard
		//OnLand(player->m_Local.m_flFallVelocity);

		//
		// Clear the fall velocity so the impact doesn't happen again.
		//
		fallVelocity = 0;
	}
	private void Friction() {
		float	speed, newspeed, control;
		float	friction;
		float	drop;

		// Calculate speed
		speed = velocity.magnitude;

		// If too slow, return
		if (speed < 0.001f) {
			return;
		}

		drop = 0;
		// apply ground friction
		if (groundEntity != null && !Input.GetButton("Jump")) { // On an entity that is the ground
			friction = baseFriction * groundFriction;

			// Bleed off some speed, but if we have less than the bleed
			//  threshold, bleed the threshold amount.
			control = (speed < groundDecellerate) ? groundDecellerate : speed;

			// Add the amount to the drop amount.
			drop += control*friction*Time.deltaTime;
		}

		// scale the velocity
		newspeed = speed - drop;
		if (newspeed < 0) {
			newspeed = 0;
		}

		if ( newspeed != speed ) {
			// Determine proportion of old speed we are using.
			newspeed /= speed;
			// Adjust velocity according to proportion.
			velocity *= newspeed;
		}

		 // mv->m_outWishVel -= (1.f-newspeed) * mv->m_vecVelocity; // ???
	}
	private void CheckVelocity() {
		int i;
		for (i=0; i < 3; i++) {
			// See if it's bogus.
			if (float.IsNaN(velocity[i])){
				Debug.Log ("Got a NaN velocity.");
				velocity[i] = 0;
			}
		}
		if (velocity.magnitude > maxSpeed) {
			velocity = Vector3.Normalize (velocity) * maxSpeed;
		}
	}
	private void FullWalkMove() {
		StartGravity();
		// Was jump button pressed?
		if (Input.GetButton("Jump")) {
			CheckJumpButton();
		}
		// Make sure we're standing on solid ground
		if (Vector3.Angle (groundNormal, new Vector3 (0f, 1f, 0f)) > controller.slopeLimit) {
			groundEntity = null;
		}
		// Friction is handled before we add in any base velocity. That way, if we are on a conveyor, 
		//  we don't slow when standing still, relative to the conveyor.
		if (groundEntity != null) {
			velocity.y = 0;
			Friction();
		}

		// Make sure velocity is valid.
		CheckVelocity();

		if (groundEntity != null) {
			WalkMove();
		} else {
			AirMove();  // Take into account movement when in air.
		}

		// Set final flags.
		//CategorizePosition();

		// Make sure velocity is valid.
		CheckVelocity();

		// Add any remaining gravitational component.
		FinishGravity();

		// If we are on ground, no downward velocity.
		if ( groundEntity != null ) {
			velocity.y = 0;
		}
		CheckFalling();
	}
	private void AirAccelerate( Vector3 wishdir, float wishspeed, float accel ) {
		int i;
		float addspeed, accelspeed, currentspeed;
		float wishspd;

		wishspd = wishspeed;

		// Cap speed
		if (wishspd > maxSpeed) {
			wishspd = maxSpeed;
		}

		// Determine veer amount
		currentspeed = Vector3.Dot(velocity, wishdir);

		// See how much to add
		addspeed = wishspd - currentspeed;

		// If not adding any, done.
		if (addspeed <= 0) {
			return;
		}

		// Determine acceleration speed after acceleration
		accelspeed = accel * wishspeed * Time.deltaTime * groundFriction;

		// Cap it
		if (accelspeed > addspeed) {
			accelspeed = addspeed;
		}

		velocity += accelspeed * wishdir;
	}
	private void StayOnGround() {
		RaycastHit hit;
		if (Physics.SphereCast (transform.position+controller.center, radius, -transform.up, out hit, distToGround + stepSize + 0.1f)) {
			// Snap the player to where the spherecast hit.
			controller.Move (new Vector3 (0, -hit.distance, 0));
		}
	}
	private void Accelerate( Vector3 wishdir, float wishspeed, float accel )
	{
		int i;
		float addspeed, accelspeed, currentspeed;

		// This gets overridden because some games (CSPort) want to allow dead (observer) players
		// to be able to move around.
		//if ( !CanAccelerate() )
		//	return;

		// See if we are changing direction a bit
		currentspeed = Vector3.Dot(velocity, wishdir);

		// Reduce wishspeed by the amount of veer.
		addspeed = wishspeed - currentspeed;

		// If not going to add any speed, done.
		if (addspeed <= 0) {
			return;
		}

		// Determine amount of accleration.
		accelspeed = accel * Time.deltaTime * wishspeed * groundFriction;

		// Cap at addspeed
		if (accelspeed > addspeed) {
			accelspeed = addspeed;
		}

		velocity += accelspeed * wishdir;
	}
	private void WalkMove() {
		int i;
		Vector3 wishvel;
		float spd;
		float fmove, smove;
		Vector3 wishdir;
		float wishspeed;

		Vector3 dest;
		Vector3 forward, right, up;

		//AngleVectors (mv->m_vecViewAngles, &forward, &right, &up);  // Determine movement angles
		forward = transform.forward;
		right = transform.right;
		up = transform.up;

		GameObject oldground;
		oldground = groundEntity;

		// Copy movement amounts
		Vector3 command = GetCommandVelocity();
		fmove = command.z; // Forward/backward
		smove = command.x; // Left/right

		// Zero out z components of movement vectors
		forward.y = 0;
		right.y   = 0;

		forward = Vector3.Normalize (forward);  // Normalize remainder of vectors.
		right = Vector3.Normalize (right);    // 

		// Determine x and y parts of velocity
		wishvel = forward * fmove + right * smove;
		wishvel.y = 0;             // Zero out z part of velocity

		wishdir = new Vector3 (wishvel.x, wishvel.y, wishvel.z); // Determine maginitude of speed of move
		wishspeed = wishdir.magnitude;
		wishdir = Vector3.Normalize(wishdir);

		//
		// Clamp to server defined max speed
		//
		if ((wishspeed != 0.0f) && (wishspeed > maxSpeed))
		{
			wishvel *= maxSpeed / wishspeed;
			wishspeed = maxSpeed;
		}

		// Set pmove velocity
		velocity.y = 0;
		Accelerate ( wishdir, wishspeed, groundAccelerate );
		velocity.y = 0;

		// Add in any base velocity to the current velocity.
		//VectorAdd (mv->m_vecVelocity, player->GetBaseVelocity(), mv->m_vecVelocity );
		velocity += groundVelocity;

		spd = velocity.magnitude;

		if ( spd < 0.01f ) {
			velocity = new Vector3 (0, 0, 0);
			// Now pull the base velocity back out.   Base velocity is set if you are on a moving object, like a conveyor (or maybe another monster?)
			// VectorSubtract( mv->m_vecVelocity, player->GetBaseVelocity(), mv->m_vecVelocity );
			return;
		}

		controller.Move (velocity * Time.deltaTime);
		// Now pull the base velocity back out.   Base velocity is set if you are on a moving object, like a conveyor (or maybe another monster?)
		// VectorSubtract( mv->m_vecVelocity, player->GetBaseVelocity(), mv->m_vecVelocity );
		velocity -= groundVelocity;

		StayOnGround();
	}
	private void AirMove() {
		int			i;
		Vector3		wishvel;
		float		fmove, smove;
		Vector3		wishdir;
		float		wishspeed;
		Vector3 	forward, right, up;

		//AngleVectors (mv->m_vecViewAngles, &forward, &right, &up);  // Determine movement angles
		forward = transform.forward;
		right = transform.right;
		up = transform.up;

		// Copy movement amounts
		Vector3 command = GetCommandVelocity();
		fmove = command.z; // Forward/backward
		smove = command.x; // Left/right

		// Zero out up/down components of movement vectors
		forward.y = 0;
		right.y = 0;
		Vector3.Normalize(forward);  // Normalize remainder of vectors
		Vector3.Normalize(right);    // 

		wishvel = forward * fmove + right * smove;
		wishvel.y = 0;             // Zero out up/down part of velocity

		wishdir = new Vector3 (wishvel.x, wishvel.y, wishvel.z);
		wishspeed = wishdir.magnitude;
		wishdir = Vector3.Normalize(wishdir);

		//
		// clamp to server defined max speed
		//
		if ( wishspeed != 0 && (wishspeed > maxSpeed)) {
			wishvel = wishvel * maxSpeed/wishspeed;
			wishspeed = maxSpeed;
		}

		if (Vector3.Dot (velocity, wishdir) < 0) {
			AirAccelerate (wishdir, wishspeed, airDeccelerate);
		} else {
			AirAccelerate (wishdir, wishspeed, airAccelerate);
		}

		// Add in any base velocity to the current velocity.
		//VectorAdd(mv->m_vecVelocity, player->GetBaseVelocity(), mv->m_vecVelocity );
		velocity += groundVelocity;

		//TryPlayerMove();
		controller.Move(velocity * Time.deltaTime);

		velocity -= groundVelocity;

		// Now pull the base velocity back out.   Base velocity is set if you are on a moving object, like a conveyor (or maybe another monster?)
		//VectorSubtract( mv->m_vecVelocity, player->GetBaseVelocity(), mv->m_vecVelocity );
	}
	private void PlayerMove() {
		//CheckParameters();

		// clear output applied velocity
		//mv->m_outWishVel.Init();
		//mv->m_outJumpVel.Init();

		//MoveHelper( )->ResetTouchList();                    // Assume we don't touch anything

		//ReduceTimers();

		//AngleVectors (mv->m_vecViewAngles, &m_vecForward, &m_vecRight, &m_vecUp );  // Determine movement angles

		// If we are not on ground, store off how fast we are moving down
		if ( groundEntity == null && velocity.y < 0 ) {
			fallVelocity = -velocity.y;
		}

		// Handle movement modes.
		FullWalkMove();
	}

	void OnControllerColliderHit(ControllerColliderHit hit ) {
		// Works with stairs, because it ignores the capsule's round bottom. But then little cracks act like you've slammed into them sideways.
		// Feels really bad on everything except stairs.
		/* RaycastHit check;
		if (Physics.Raycast (transform.position, Vector3.Normalize (hit.point - transform.position), out check, controller.height*2f)) {
			// If the hit is something that seems like it might be a stair in a staircase, ignore it.
			Debug.Log( Mathf.Abs(Vector3.Angle(check.normal, new Vector3 (0f, 1f, 0f))-90f) );
			if ((hit.point - transform.position).y < stepSize && Mathf.Abs(Vector3.Angle(check.normal, new Vector3 (0f, 1f, 0f))-90f) < 0.01) {
				return;
			}
			// If it's something that we don't consider solid ground, clip our velocity on it.
			if (Vector3.Angle (check.normal, new Vector3 (0f, 1f, 0f)) > controller.slopeLimit) {
				velocity = ClipVelocity (velocity, check.normal);
			}
		} */

		if (Vector3.Angle (hit.normal, new Vector3 (0f, 1f, 0f)) > controller.slopeLimit) {
			velocity = ClipVelocity (velocity, hit.normal);
		}
	}
}
