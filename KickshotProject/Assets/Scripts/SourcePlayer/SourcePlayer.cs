﻿// Mostly this is built directly from [source-sdk-2013](https://github.com/ValveSoftware/source-sdk-2013/blob/56accfdb9c4abd32ae1dc26b2e4cc87898cf4dc1/sp/src/game/shared/gamemovement.cpp)
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
	public float airDeccelerateMultiplier = 1f; // How fast the player can stop in mid-air or slow down.
	public float walkSpeed = 10f; // How fast the player runs.
	public float jumpSpeed = 8f; // The y velocity to set our character at when they jump.
	public float fallSoundThreshold = 8f; // How fast we must be falling before we make a thud.
	public float fallPunchThreshold = 10f; // How fast we must be falling before we shake the screen.
	public float maxSafeFallSpeed = 25f; // How fast we must be falling before we take damage.
	public float jumpSpeedBonus = 0.1f; // Speed boost from just jumping forward as a percentage.
	public float health = 100f;
	public CollisionSphere[] spheres =
		new CollisionSphere[2] {
		//new CollisionSphere(-.5f),
		new CollisionSphere(0f),
		new CollisionSphere(.5f),
	};

    private float frictionStun = 0f; // Timer to keep track of how long to disable friction.
	private float frictionStunPercentage = 0.5f; // How much we actually disable friction by.
	private const float overbounce = 2f; // How much to multiply incoming collision velocities, to keep us from getting stuck in moving objects.
	// We only collide with these layers.
	private int layerMask;
	private const float TinyTolerance = 0.05f; // How much to allow penetration.
	private const float buffer = 0.25f; // Distance to try and keep the character controller from touching anything. Because the character controller acts like it has infinite mass, and also because it acts funny with moving colliders.
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
		distToGround = controller.height / 2f;//GetComponent<Collider>().bounds.extents.y/2f;
		radius = controller.radius+buffer;
		stepSize = controller.stepOffset;
		TemporaryLayerIndex = LayerMask.NameToLayer(TemporaryLayer);
    }
	private bool CalculateGround( RaycastHit hit ) {
		// Check to see if it's valid solid ground.
		if ( Vector3.Angle(hit.normal, Vector3.up) > controller.slopeLimit ) {
			return false;
		}
		// Snap the player to where the spherecast hit.
		groundEntity = hit.collider.gameObject;
		groundNormal = hit.normal;
		Collider ccheck = groundEntity.GetComponent<Collider> ();
		if (ccheck != null) {
			groundFriction = ccheck.material.dynamicFriction;
		} else {
			groundFriction = 1f;
		}
		groundVelocity = Vector3.zero;
		Movable check = groundEntity.GetComponent<Movable> ();
		if (check != null) {
			groundVelocity = check.velocity;
		}
		Rigidbody cccheck = groundEntity.GetComponent<Rigidbody> ();
		if (cccheck != null) {
			groundVelocity = cccheck.GetPointVelocity(hit.point);
		}
		return true;
	}

	void Update() {
		bool hitGround = false;
		if (velocity.y <= 0) {
			foreach( RaycastHit hit in Physics.SphereCastAll (transform.position, radius, -transform.up, distToGround-radius+0.1f, layerMask, QueryTriggerInteraction.Ignore) ) {
				// This means that our initial sphere is already colliding with something
				// if our initial sphere is colliding with something, we don't get any useful information...
				if (hit.distance == 0) {
					// We have to do another separate raycast, this takes care of a corner case (literally).
					RaycastHit newHit;
					//Debug.Assert ("AAA");
					if (Physics.Raycast (transform.position, -transform.up, out newHit, distToGround+0.1f, layerMask, QueryTriggerInteraction.Ignore)) {
						hitGround = hitGround || CalculateGround (newHit);
						if (hitGround) { 
							break;
						}
					}
					continue;
				}
				hitGround = hitGround || CalculateGround (hit);
				if (hitGround) { 
					break;
				}
			}
		}

		if ( !hitGround ) {
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
	private Vector3 ClipVelocity( Vector3 vel, Vector3 normal) {
		float overbounce = 1.0f; // How much to bounce off the surface, 1.0 means we just slide normally. 2.0 would bounce us off.
		float	backoff;
		Vector3	change;
		float   angle;
		int		i, blocked;
		Vector3 outvel;

		angle = normal.y;
		// Determine how far along plane to slide based on incoming direction.
		backoff = Vector3.Dot(vel, normal) * overbounce;

		change = normal*backoff;
		outvel = vel - change;
		// iterate once to make sure we aren't still moving through the plane
		float adjust = Vector3.Dot( outvel, normal );
		if( adjust < 0.0f ) {
			outvel -= ( normal * adjust );
		}
		return outvel;
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

        // We landed on something solidly, if it has some velocity we need to subtract it from our own.
        // This makes our velocities match up again.
        if (groundVelocity.magnitude > 0)
        {
            velocity -= groundVelocity;
        }
		if (fallVelocity >= fallSoundThreshold) {
			float fvol = Mathf.Min ((fallVelocity - fallSoundThreshold) / (maxSafeFallSpeed - fallSoundThreshold), 1f);
			PlayerRoughLandingEffects (fvol, transform.position - new Vector3 (0f, distToGround, 0f), Vector3.up);
		}
		if ( fallVelocity >= fallPunchThreshold ) {

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

			float shakeIntensity = Mathf.Min ((fallVelocity - fallPunchThreshold) / (maxSafeFallSpeed - fallPunchThreshold), 1f);
			gameObject.SendMessage ("ShakeImpact", Vector3.down*shakeIntensity);

			if ( fallVelocity > maxSafeFallSpeed ) {
				//
				// If they hit the ground going this fast they may take damage (and die).
				//
				hardLand.Play ();
				//gameObject.SendMessage("Damage", (fallVelocity - maxSafeFallSpeed)*5f );
			}
			// Linearly scale the impact volume with how fast we hit.
		}

		// Clip our velocity, even if we landed on solid ground, we might gain or lose speed depending on the slope...
		// Jumping also clips our velocity, we only want to do it once.
		if (!Input.GetButton ("Jump")) {
			velocity = ClipVelocity (velocity, groundNormal);
		}
		//
		// Clear the fall velocity so the impact doesn't happen again.
		//
		fallVelocity = 0;
	}

	private void PlayerRoughLandingEffects(float volume, Vector3 hitpos, Vector3 hitnormal) {
        RaycastHit hit;
		if (Physics.Raycast(hitpos+hitnormal*0.1f, -hitnormal, out hit, 1f)) {
			AudioSource.PlayClipAtPoint (ImpactSounds.GetSound(Helper.getMaterial(hit)), hitpos, volume);
        }
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
			if (frictionStun > 0f) {
				friction = baseFriction * groundFriction * frictionStunPercentage;
				frictionStun -= Time.deltaTime;
			} else {
				friction = baseFriction * groundFriction;
			}

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
		float savedy = velocity.y;
		velocity.y = 0;
		if (velocity.magnitude > maxSpeed) {
			velocity = Vector3.Normalize (velocity) * maxSpeed;
		}
		velocity.y = savedy;
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
		foreach( RaycastHit hit in Physics.SphereCastAll (transform.position+controller.center, radius, -transform.up, distToGround + stepSize - radius + 0.1f, layerMask, QueryTriggerInteraction.Ignore)) {
			// Snap the player to where the spherecast hit.
			// This means that our initial sphere is already colliding with something
			// if our initial sphere is colliding with something, we don't get any useful information...
			if (hit.distance == 0) {
				continue;
			}
			if ( Vector3.Angle(hit.normal, Vector3.up) > controller.slopeLimit ) {
				continue;
			}
			controller.Move (new Vector3 (0, -hit.distance, 0));
			break;
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
			Accelerate (wishdir, wishspeed, velocity.magnitude*airDeccelerateMultiplier);
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
	private void HandleCollision( GameObject obj, Vector3 hitNormal, Vector3 hitPos ) {
		//Debug.Log ("Hello " + Time.time);
		if ((layerMask & (1<<obj.layer)) == 0) {
			//Debug.Log ("Ignoring collsion of object with " + obj.layer);
			return;
		}
		if (Vector3.Angle (hitNormal, Vector3.up) < controller.slopeLimit) {
			//Debug.Log ("Ignoring collsion because it's valid ground.");
			return;
		}
		float mag = velocity.magnitude;
		// If we walk off an edge, we won't inherit the ground velocity. So if we walk off an edge while moving fast
		// then hit a wall, this prevents us from infinitely being pushed into that wall from our inherited velocity.
		if (groundVelocity.magnitude > 0f) {
			velocity += groundVelocity;
			groundVelocity = Vector3.zero;
		}
		velocity = ClipVelocity (velocity, hitNormal);
		Movable check = obj.GetComponent<Movable> ();
		if (check != null) {
			frictionStun = 0.3f; // Stun our friction
			Vector3 vel = check.velocity;
			float d = Vector3.Dot (Vector3.Normalize (vel), hitNormal); // How similar is our velocity to our hitnormal (perp = 0, backwards = -1, same = 1)
			if ( d > 0 ) { // If the velocity should be applied
				velocity += vel * d * overbounce; // We apply it with some overbounce, to keep us from getting stuck.
			}
		}
		Rigidbody rigidcheck = obj.GetComponent<Rigidbody> ();
		if (rigidcheck != null) {
			frictionStun = 0.3f;
			Vector3 vel = rigidcheck.GetPointVelocity (hitPos);
			float d = Vector3.Dot (Vector3.Normalize (vel), hitNormal);
			if ( d > 0 ) {
				velocity += vel * d * overbounce;
			}
		}
		float change = Mathf.Abs (mag - velocity.magnitude);
		if (change > fallSoundThreshold) {
			float fvol = Mathf.Min (change / (maxSafeFallSpeed - fallSoundThreshold), 1f);
			PlayerRoughLandingEffects (fvol, hitPos, hitNormal);
		}
		if (change > fallPunchThreshold) {
			float shakeIntensity = Mathf.Min ((change - fallPunchThreshold) / (maxSafeFallSpeed - fallPunchThreshold), 1f);
			gameObject.SendMessage ("ShakeImpact", -hitNormal * shakeIntensity);
		}
	}
	void OnControllerColliderHit(ControllerColliderHit hit ) {
		HandleCollision (hit.gameObject, hit.normal, hit.point);
	}
	// This function makes sure we don't phase through other colliders. (Since character controller doesn't provide this functionality lmao).
	// I copied it from https://github.com/IronWarrior/SuperCharacterController
	// I changed it a bit, but SuperCharacterController is under the MIT license, meaning we can't use it without making our game also under the MIT license.
	// so TODO: Change this enough that we don't have to use the MIT license if we don't want to.
	private void RecursivePushback(int depth, int maxDepth) {
		bool contact = false;
		foreach (var sphere in spheres) {
			foreach (Collider col in Physics.OverlapSphere(SpherePosition(sphere), radius, layerMask, QueryTriggerInteraction.Ignore)) {
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
					col.gameObject.layer = TemporaryLayerIndex;
					// Retrieve the surface normal of the collided point
					RaycastHit normalHit;
					Physics.SphereCast(new Ray(position + v, contactPoint - (position + v)), TinyTolerance, out normalHit, 1 << TemporaryLayerIndex);
					col.gameObject.layer = layer;
					HandleCollision (col.gameObject, normalHit.normal, normalHit.point);
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