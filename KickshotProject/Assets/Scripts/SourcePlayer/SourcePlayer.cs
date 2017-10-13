// Mostly this is built directly from [source-sdk-2013](https://github.com/ValveSoftware/source-sdk-2013/blob/56accfdb9c4abd32ae1dc26b2e4cc87898cf4dc1/sp/src/game/shared/gamemovement.cpp)
// Though there's quite a few edits or adjustments to make it work with unity's character controller.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourcePlayer : MonoBehaviour {
	public GameObject deathSpawn;
	public Vector3 velocity;
	public Vector3 gravity = new Vector3(0,-20f,0); // gravity in meters per second per second.
	public float baseFriction = 6f; // A friction multiplier, higher means more friction.
	public float maxSpeed = 35f; // The maximum speed the player can move at.
	public float groundAccelerate = 10f; // How fast we accelerate while on solid ground.
	public float groundDecellerate = 10f; // How fast we deaccelerate on solid ground.
	public float airAccelerate = 1f; // How much air control the player has.
	public float airDeccelerate = 10f; // How fast the player can stop in mid-air or slow down.
	public float walkSpeed = 10f; // How fast the player runs.
	public float jumpSpeed = 8f; // The y velocity to set our character at when they jump.
	public float fallPunchThreshold = 8f; // How fast we must be falling before we shake the screen and make a thud.
	public float maxSafeFallSpeed = 15f; // How fast we must be falling before we take damage.
	public float jumpSpeedBonus = 0.1f; // Speed boost from just jumping forward as a percentage.
	public float health = 100f;
	public CollisionSphere[] spheres =
		new CollisionSphere[3] {
		new CollisionSphere(0.5f),
		new CollisionSphere(1.0f),
		new CollisionSphere(1.5f),
	};

	// We only collide with these layers.
	private int layerMask;
	private const float TinyTolerance = 0.01f;
	private float lastGrunt;
	private float stepSize = 0.5f;
	private float fallVelocity;
	private CharacterController controller;
	private GameObject groundEntity = null;
	private Vector3 groundNormal = new Vector3(0f,1f,0f);
	public Vector3 groundVelocity;
	private float groundFriction;
	private float distToGround;
	private float radius;
	private const string TemporaryLayer = "TempCast";
	private const int MaxPushbackIterations = 2;
	private int TemporaryLayerIndex;
	private AudioSource jumpGrunt;
	private AudioSource painGrunt;
	private AudioSource hardLand;
	void Start() {
		var aSources = GetComponents<AudioSource> ();
		jumpGrunt = aSources [0];
		painGrunt = aSources [1];
		hardLand = aSources [2];
		// This generates our layermask, making sure we only collide with stuff that's specified by the physics engine.
		int myLayer = gameObject.layer;
		layerMask = 0;
		for(int i = 0; i < 32; i++) {
			if(!Physics.GetIgnoreLayerCollision(myLayer, i))  {
				layerMask = layerMask | 1 << i;
			}
		}

		controller = GetComponent<CharacterController> ();
		distToGround = GetComponent<Collider>().bounds.extents.y/2f;
		radius = controller.radius;
		stepSize = controller.stepOffset;
		TemporaryLayerIndex = LayerMask.NameToLayer(TemporaryLayer);
	}
	void Update() {

		RaycastHit hit;
		if (velocity.y <= 0 && Physics.SphereCast (transform.position+controller.center, radius, -transform.up, out hit, distToGround + 0.1f, layerMask, QueryTriggerInteraction.Ignore)) {
			// Snap the player to where the spherecast hit.
			groundEntity = hit.collider.gameObject;
			groundNormal = hit.normal;
			Collider ccheck = groundEntity.GetComponent<Collider> ();
			if (ccheck != null) {
				groundFriction = ccheck.material.dynamicFriction;
			} else {
				groundFriction = 1f;
			}
			Movable check = groundEntity.GetComponent<Movable> ();
			if (check != null) {
				Rigidbody cccheck = groundEntity.GetComponent<Rigidbody> ();
				if (cccheck != null) {
					groundVelocity = cccheck.GetPointVelocity(hit.point);
				} else {
					groundVelocity = check.velocity;
				}
			} else {
				groundVelocity = Vector3.zero;
			}
		} else {
			groundEntity = null;
			groundFriction = 1f;
			groundNormal = Vector3.up;
			//groundVelocity = new Vector3 (0f, 0f, 0f); Shouldn't set this, need to remember how fast we were launched off of a moving object.
		}

		PlayerMove();
		// Push ourselves out of nearby objects.
		RecursivePushback(0, MaxPushbackIterations);
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
	private void Gravity() {
		velocity += gravity * Time.deltaTime;
	}
	private void CheckJump() {
		// Check to make sure we have a ground under us, and that it's stable ground.
		if ( Input.GetButton("Jump") && groundEntity && Vector3.Angle(groundNormal,new Vector3(0f,1f,0f)) < controller.slopeLimit ) {
			// Right before we jump, lets clip our velocity real quick. That way if we're jumping down a sloped surface, we go faster!
			velocity = ClipVelocity(velocity, groundNormal);
			// Play a grunt sound, but only so often.
			if (Time.time - lastGrunt > 0.3) {
				jumpGrunt.Play ();
				lastGrunt = Time.time;
			}
			velocity.y = jumpSpeed;
			groundEntity = null;
			velocity += groundVelocity;
			groundVelocity = Vector3.zero;
			// We give a certain percentage of the current forward movement as a bonus to the jump speed.  That bonus is clipped
			// to not accumulate over time
			Vector3 commandVel = GetCommandVelocity ();
			float flSpeedAddition = Mathf.Abs( commandVel.z * jumpSpeedBonus );
			float flMaxSpeed = maxSpeed + ( maxSpeed * jumpSpeedBonus );
			Vector3 flatvel = new Vector3( velocity.x, 0, velocity.z );
			float flNewSpeed = ( flSpeedAddition + flatvel.magnitude );
			// If we're over the maximum, we want to only boost as much as will get us to the goal speed
			if ( flNewSpeed > flMaxSpeed ) {
				flSpeedAddition -= flNewSpeed - flMaxSpeed;
			}
			if (commandVel.z < 0.0f) {
				flSpeedAddition *= -1.0f;
			}
			velocity += transform.forward * flSpeedAddition;
		}
		// We were standing on the ground, then suddenly are not.
		if (velocity.y >= jumpSpeed/2f) {
			// We don't inherit the groundVelocity here, because if we just were bumped slightly off of a moving ground
			// that would allow us to accelerate crazily by just being on unstable ground (like a rigidbody).
			groundEntity = null;
		}
	}
	private void CheckFalling() {
		//Debug.Log (fallVelocity);
		// this function really deals with landing, not falling, so early out otherwise
		if (groundEntity == null || Vector3.Angle (groundNormal, Vector3.up) > controller.slopeLimit || fallVelocity <= 0f) {
			return;
		}

		if ( fallVelocity >= fallPunchThreshold ) {
				
			//bool bAlive = true;
			//float fvol = 0.5f;

			// Scale it down if we landed on something that's floating...
			//if ( player->GetGroundEntity()->IsFloating() ) {
			//	player->m_Local.m_flFallVelocity -= PLAYER_LAND_ON_FLOATING_OBJECT;
			//}

			//
			// They hit the ground.
			//

			// Player landed on a descending object. Subtract the velocity of the ground entity.
			if (groundVelocity.y < 0f) {
				fallVelocity += groundVelocity.y;
				fallVelocity = Mathf.Max (0.1f, fallVelocity);
			}

			if ( fallVelocity > maxSafeFallSpeed ) {
				//
				// If they hit the ground going this fast they may take damage (and die).
				//
				hardLand.Play ();
				//gameObject.SendMessage("Damage", (fallVelocity - maxSafeFallSpeed)*5f );
				//fvol = 1.0f;
			} else if ( fallVelocity > maxSafeFallSpeed / 2 ) {
				//fvol = 0.85f;
			} else {
				//fvol = 0f;
			}

			// PlayerRoughLandingEffects( fvol );
		}

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
	private void PlayerMove() {
		CheckFalling();
		// If we are not on ground, store off how fast we are moving down
		if ( groundEntity == null ) {
			fallVelocity = -velocity.y;
		}
		// Was jump button pressed?
		CheckJump();
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

		Gravity();

		// If we are on ground, no downward velocity.
		if ( groundEntity != null ) {
			velocity.y = 0;
		}
	}
	private void Accelerate( Vector3 wishdir, float wishspeed, float accel )
	{
		//int i;
		float addspeed, accelspeed, currentspeed;

		// This gets overridden because some games (CSPort) want to allow dead (observer) players
		// to be able to move around.
		//if ( !CanAccelerate() )
		//	return;

		// Cap speed
		if (wishspeed > maxSpeed) {
			wishspeed = maxSpeed;
		}

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
	private void StayOnGround() {
		RaycastHit hit;
		if (Physics.SphereCast (transform.position+controller.center, radius, -transform.up, out hit, distToGround + stepSize + 0.1f, layerMask, QueryTriggerInteraction.Ignore)) {
			// Snap the player to where the spherecast hit.
			controller.Move (new Vector3 (0, -hit.distance, 0));
		}
	}
	private void WalkMove() {
		//int i;
		Vector3 wishvel;
		float spd;
		float fmove, smove;
		Vector3 wishdir;
		float wishspeed;

		//Vector3 dest;
		Vector3 forward, right, up;

		//AngleVectors (mv->m_vecViewAngles, &forward, &right, &up);  // Determine movement angles
		forward = transform.forward;
		right = transform.right;
		up = transform.up;

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
		//int			i;
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

		// If we're trying to stop, use airDeccelerate value (usually much larger value than airAccelerate)
		if (Vector3.Dot (velocity, wishdir) < 0) {
			Accelerate (wishdir, wishspeed, airDeccelerate);
		} else {
			Accelerate (wishdir, wishspeed, airAccelerate);
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
	void OnControllerColliderHit(ControllerColliderHit hit ) {
		if ((layerMask & (1<<hit.gameObject.layer)) == 0) {
			return;
		}
		if (Vector3.Angle (hit.normal, Vector3.up) < controller.slopeLimit) {
			return;
		}
		// If we walk off an edge, we won't inherit the ground velocity. So if we walk off an edge while moving fast
		// then hit a wall, this prevents us from infinitely being pushed into that wall from our inherited velocity.
		if (groundVelocity.magnitude > 0f) {
			velocity += groundVelocity;
			groundVelocity = Vector3.zero;
		}
		velocity = ClipVelocity (velocity, hit.normal);
	}
	// This function makes sure we don't phase through other colliders. (Since character controller doesn't provide this functionality lmao).
	// I copied it from https://github.com/IronWarrior/SuperCharacterController
	// I changed it a bit, but SuperCharacterController is under the MIT license, meaning we can't use it without making our game also under the MIT license.
	// so TODO: Change this enough that we don't have to use the MIT license if we don't want to.
	private void RecursivePushback(int depth, int maxDepth) {
		bool contact = false;
		foreach (var sphere in spheres) {
			foreach (Collider col in Physics.OverlapSphere(SpherePosition(sphere), controller.radius, layerMask, QueryTriggerInteraction.Ignore)) {
				if (col.isTrigger) {
					continue;
				}
				if (col.gameObject == gameObject) {
					continue;
				}
				Vector3 position = SpherePosition(sphere);
				Vector3 contactPoint;
				bool contactPointSuccess = SuperCollider.ClosestPointOnSurface(col, position, radius, out contactPoint);

				if (!contactPointSuccess) {
					return;
				}

				Vector3 v = contactPoint - position;
				if (v != Vector3.zero) {
					// Cache the collider's layer so that we can cast against it
					int layer = col.gameObject.layer;
					col.gameObject.layer = TemporaryLayerIndex;
					// Check which side of the normal we are on
					bool facingNormal = Physics.SphereCast(new Ray(position, v.normalized), TinyTolerance, v.magnitude + TinyTolerance, 1 << TemporaryLayerIndex, QueryTriggerInteraction.Ignore);
					col.gameObject.layer = layer;

					// Orient and scale our vector based on which side of the normal we are situated
					if (facingNormal) {
						if (Vector3.Distance(position, contactPoint) < radius) {
							v = v.normalized * (radius - v.magnitude) * -1;
						} else {
							// A previously resolved collision has had a side effect that moved us outside this collider
							continue;
						}
					} else {
						v = v.normalized * (radius + v.magnitude);
					}

					contact = true;
					transform.position += v;
				}
			}            
		}
		if (depth < maxDepth && contact) {
			RecursivePushback(depth + 1, maxDepth);
		}
	}
	public Vector3 SpherePosition(CollisionSphere sphere) {
		return transform.position + sphere.offset * transform.up;
	}
	void Damage( float damage ) {
		health -= damage;
		painGrunt.Play ();
		if (health <= 0f) {
			gameObject.SetActive (false);
			Instantiate (deathSpawn, transform.position, Quaternion.identity);
		}
	}
}

public class CollisionSphere
{
	public float offset;

	public CollisionSphere(float offset)
	{
		this.offset = offset;
	}
}