// Mostly this is built directly from [source-sdk-2013](https://github.com/ValveSoftware/source-sdk-2013/blob/56accfdb9c4abd32ae1dc26b2e4cc87898cf4dc1/sp/src/game/shared/gamemovement.cpp)
// Though there's quite a few edits or adjustments to make it work with unity's character controller.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SourcePlayer : MonoBehaviour {
    // Accessible because it's configurable
    public Transform body;
    public GameObject deathSpawn;
    public Vector3 velocity;
    public Vector3 gravity = new Vector3 (0, -24f, 0); // gravity in meters per second per second.
    public float wallGravity = -300; // Gravity is weird on wall runs because of clip velocity.
    public float baseFriction = 6f; // A friction multiplier, higher means more friction.
    public float maxSpeed = 100f; // The maximum speed the player can move at. (CURRENTLY UNUSED)
    public float groundAccelerate = 5f; // How fast we accelerate while on solid ground.
    public float groundDecellerate = 10f; // How fast we deaccelerate on solid ground.
    public float airAccelerate = 2f; // How much the player can influence increasing speed in the air, mesured in meters/sec^2.
    public float airStrafeAccelerate = 10f; // How much the player can influence speed in the air while air-strafing, mesured in meters/sec^2.
    public float wallAccelerate = 1.0f;
    [HideInInspector]
    public float airSpeedBonus = 0.08f; // How much the player is rewarded for strafe-jumping. Great way to gain speed.
    [HideInInspector]
    public float airSpeedPunish = 1f;// How much we decelerate the player for trying to take turns too quickly while strafe jumping.
    public float airBreak = 3f;// Acceleration multiplier for trying to stop with the backwards key. (typically S).
    public float walkSpeed = 16f;// We stop applying standard acceleration when the player is this speed on the ground.
    public float flySpeed = 12f;// We stop applying standard acceleration when the player is this speed in the air.
    public float jumpSpeed = 10f;// The y velocity to set our character at when they jump.
    public float wallSpeed = 5f;
    public float fallSoundThreshold = 5f;// How fast we must be falling before we make a thud.
    public float fallPunchThreshold = 10f;// How fast we must be falling before we shake the screen.
    public float maxSafeFallSpeed = 25f;// How fast we must be falling before we take damage.
    public float jumpSpeedBonus = 0.1f;// Speed boost from just jumping forward as a percentage. Stops boosting when the player is already at or beyond flySpeed.
    public float health = 100f;
    public float mass = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 6f;
    public float crouchJumpSpeed = 6f;
    public float stepSize = 0.5f;
    public float crouchTime = 0.3f;
    public bool autoBhop = false;
    public float crouchAcceleration = 8f;
    // Time in seconds it takes to crouch

    // Accessible because it's useful
    [HideInInspector]
    public bool justJumped = false;
    [HideInInspector]
    public bool justTookFallDamage = false;
    [HideInInspector]
    public Vector3 groundVelocity;
    [HideInInspector]
    public GameObject groundEntity = null;
    [HideInInspector]
    public bool wantCrouch = false;
    [HideInInspector]
    public bool crouched = false;

    [HideInInspector]
    public GameObject wallEntity = null;
    [HideInInspector]
    public bool wallRunning = false;


    // Shouldn't need to access these, probably
    private List<ContactPoint> contacts;
    private Vector3 wallNormal = new Vector3 (0f, 0f, 0f);
    private Vector3 wallHitPoint = new Vector3 (0f, 0f, 0f);
    private bool queueHop = false;
    private float crouchTimer = 0f;
    private bool changedSpeed = false;
    private Vector3 originalBodyPosition;
    private float originalHeight;
    private CollisionSphere[] spheres;
    private bool ignoreCollisions = false;
    private bool ignoreFootCollisions = false;
    private const float overbounce = 1f;
    // How much to multiply incoming collision velocities, to keep us from getting stuck in moving objects.
    private int layerMask;
    // We only collide with these layers.
    private const float TinyTolerance = 0.05f;
    // How much to allow penetration.
    private float lastGrunt;
    private float fallVelocity;
    private CharacterController controller;
    private Vector3 groundNormal = new Vector3 (0f, 1f, 0f);
    private float groundFriction;
    private float distToGround;
    private float radius;
    private const string TemporaryLayer = "TempCast";
    private const int MaxPushbackIterations = 2;
    private int TemporaryLayerIndex;
    private AudioSource jumpGrunt;
    private AudioSource painGrunt;
    private AudioSource hardLand;
    new private CapsuleCollider collider;

    void Awake () {
        // Not sure how audio is supposed to work in unity, I just have a list of them on the player to have the jump, pain, and break sounds.
        contacts = new List<ContactPoint>();
        var aSources = GetComponents<AudioSource> ();
        jumpGrunt = aSources [0];
        painGrunt = aSources [1];
        hardLand = aSources [2];

        // This generates our layermask, making sure we only collide with stuff that's specified by the physics engine.
        // This makes it so that if we specify in-engine layers to not collide with the player, that we actually abide to it.
        int myLayer = gameObject.layer;
        layerMask = 0;
        for (int i = 0; i < 32; i++) {
            if (!Physics.GetIgnoreLayerCollision (myLayer, i)) {
                layerMask = layerMask | 1 << i;
            }
        }

        controller = GetComponent<CharacterController> ();
        controller.detectCollisions = false; // The default collision resolution for character controller vs rigidbody is analogus to unstoppable infinite mass vs paper. We don't want that.
        controller.enableOverlapRecovery = true;
        controller.stepOffset = 0f; // We use our own step logic.

        originalHeight = controller.height;
        RepositionHitboxes ();
        if (stepSize > radius - 0.05f) {
            Debug.LogError ("Player step size can't be higher or equal to the radius of the capsule, automatically reducing...");
            stepSize = radius - 0.05f;
        }
        // We use this layer to quickly do collision tests with singular objects.
        TemporaryLayerIndex = LayerMask.NameToLayer (TemporaryLayer);
        originalBodyPosition = body.localPosition;
        collider = GetComponent<CapsuleCollider> ();
    }

    private bool TryJump(bool Jumping = false) {
        if (!autoBhop && Input.GetButtonDown ("Jump")) {
            queueHop = true;
        }
        return (autoBhop && Input.GetButton ("Jump")) || queueHop;
    }

    private bool RaycastForGround (out RaycastHit resultHit) {
        // Loop through everything in our make-shift cylinder cast, checking for if there's a ground below us.
        Vector3 castPos = transform.position;
        float castLength = distToGround+stepSize/2f;
        Vector3 halfExtents = new Vector3 (radius, 0.1f, radius);
        for ( float i = 0; i < radius; i += radius/4f ) { // Since we can hit something under us, but have it report as outside of our "cylinder" randomly, we have to try multiple times with different radiuses.
            foreach (RaycastHit hit in Physics.BoxCastAll(castPos, halfExtents, -transform.up, transform.rotation, castLength, layerMask, QueryTriggerInteraction.Ignore)) {
                // This means that our initial box is already colliding with something
                // If our initial cast position is colliding with something, we don't get any useful information, so we skip it!
                // TODO: This would also indicate that the player is "stuck" inside of something. But this should be impossible since we actively teleport the player outside of geometry.
                if (hit.distance == 0) {
                    continue;
                }
                // Get the distance (ignoring the y axis) of the hit point and our "cylinder", if it's outside of our cylinder we ignore it.
                float flathitDist = Mathf.Sqrt ((transform.position.x - hit.point.x) * (transform.position.x - hit.point.x) + (transform.position.z - hit.point.z) * (transform.position.z - hit.point.z));
                if (flathitDist > radius) {
                    continue;
                }
                // A hit was found of valid ground! woo!
                if (hit.normal.y > 0.7) {
                    resultHit = hit;
                    return true;
                }
            }
            // Reduce our radius and try again...
            halfExtents = new Vector3 (radius-i, 0.1f, radius-i);
        }
        // Last ditch effort to find some ground, essentially a "cylinder" with a radius of 0.
        RaycastHit newHit;
        if (Physics.Raycast (castPos, -transform.up, out newHit, castLength, layerMask, QueryTriggerInteraction.Ignore)) {
            if (newHit.normal.y > 0.7) {
                resultHit = newHit;
                return true;
            }
        }
        resultHit = newHit;
        return false;
    }

    private bool RaycastForHeadroom (out RaycastHit hit, float extraCheckDistance = 0f) {
        return Physics.BoxCast (transform.position, new Vector3 (radius, 0.1f, radius), transform.up, out hit, transform.rotation, originalHeight / 2f + extraCheckDistance, layerMask, QueryTriggerInteraction.Ignore);
    }

    private void RepositionHitboxes () {
        distToGround = controller.height / 2f;
        radius = controller.radius;
        // We define our collision spheres, we have one at our center, one at our head, and one at our feet.
        if (spheres == null) {
            spheres = new CollisionSphere[3] {
                new CollisionSphere (-controller.height / 2 + radius, radius),
                new CollisionSphere (0f, radius),
                new CollisionSphere (controller.height / 2 - radius, radius),
            };
        } else {
            spheres [0].offset = -controller.height / 2 + radius;
            spheres [1].offset = 0f;
            spheres [2].offset = controller.height / 2 - radius;
            spheres [0].radius = radius;
            spheres [1].radius = radius;
            spheres [2].radius = radius;
        }
    }
    // CalculateGround takes a raycast and generates ground information from it.
    // This is necessary to grab material frictions, moving ground velocities, and normals.
    // It also returns if the raycast hit valid ground or not.
    private bool CalculateGround (RaycastHit hit) {
        // Check to see if it's valid solid ground.
        if (hit.normal.y < .7f) {
            return false;
        }
        groundEntity = hit.collider.gameObject;
        groundNormal = hit.normal;
        // If we have a collider, since the raycast hit it-- we probably do, but i check anyway!
        Collider ccheck = groundEntity.GetComponent<Collider> ();
        if (ccheck != null) {
            groundFriction = ccheck.material.dynamicFriction;
        } else {
            groundFriction = 1f;
        }

        // We need to see if we have a velocity now, in order for the player to stay on moving conveyors and stuff.
        groundVelocity = Vector3.zero;
        // A movable just gives us a velocity. (most basic platforms, or conveyors should be movable)
        Movable check = groundEntity.GetComponent<Movable> ();
        if (check != null) {
            groundVelocity = check.velocity;
        }
        // A rigidbody we have to calculate the velocity of the ground immediately below us.
        Rigidbody cccheck = groundEntity.GetComponent<Rigidbody> ();
        if (cccheck != null) {
            groundVelocity = cccheck.GetPointVelocity (hit.point);
        }
        return true;
    }

    private bool ChangeHeight (float newHeight) {
        float diff = newHeight - controller.height;
        RaycastHit hit;
        if (diff > 0f) {
            if (RaycastForHeadroom (out hit, diff)) { // if we hit our head, just quit.
                return false;
            }
        }
        if (diff == 0) {
            return true;
        }
        collider.height = newHeight;
        body.localPosition = originalBodyPosition + new Vector3 (0f, (originalHeight - newHeight) / 2f, 0f);
        controller.height = newHeight;
        if (groundEntity != null) {
            // If we're on the ground, we pull our head down.
            controller.Move (new Vector3 (0, diff / 2f, 0));
        } else {
            // If we're in the air, we pull our legs up
            controller.Move (new Vector3 (0, -diff / 2f, 0));
        }
        RepositionHitboxes ();
        return true;
    }

    // TODO: Gotta make this smoothly transition, shouldn't be hard.
    private void CheckCrouched () {
        RaycastHit hit;
        if (Input.GetButton ("Crouch") && !wantCrouch) {
            wantCrouch = true;
        } else if (!Input.GetButton ("Crouch") && wantCrouch && !RaycastForHeadroom (out hit)) {
            wantCrouch = false;
        }
        if (wantCrouch) {
            if (crouchTimer < crouchTime) {
                crouchTimer += Time.deltaTime;
            } else {
                crouchTimer = crouchTime;
            }
            float progress = crouchTimer / crouchTime;
            float newheight = (originalHeight * (1f - progress)) + crouchHeight * progress;
            ChangeHeight (newheight);
        } else {
            if (crouchTimer > 0f) {
                crouchTimer -= Time.deltaTime;
            } else {
                crouchTimer = 0f;
            }
            float progress = (crouchTime - crouchTimer) / crouchTime;
            float newheight = (crouchHeight * (1f - progress)) + originalHeight * progress;
            if (!ChangeHeight (newheight)) { // If we fail to change heights, we don't progress the timer.
                crouchTimer += Time.deltaTime;
            }
        }
        if (crouchTimer / crouchTime > 0.5f) {
            crouched = true;
        } else {
            crouched = false;
        }
    }

    void Update () {
        // assume we haven't jumped and that we haven't taken damage.
        // assume we also haven't hit the ground.
        justJumped = false;
        justTookFallDamage = false;
        // assume we aren't touching anything
        // TODO: Joey, you'd probably check if we hit a wall last frame here.
        // since contacts currently contains what we hit in the last frame.
        contacts.Clear();
        bool hitGround = false;
        // We only check for ground under us if we're moving downwards.
        if (velocity.y <= 0) {
            RaycastHit hit;
            hitGround = RaycastForGround (out hit);
            if (hitGround) {
                hitGround = CalculateGround (hit);
            }
        }

        // If we failed to find any ground, we set up some variables that let the rest of the code know.
        if (!hitGround) {
            groundEntity = null;
            groundFriction = 1f;
            groundNormal = Vector3.up;
            //groundVelocity = new Vector3 (0f, 0f, 0f); Shouldn't set this, need to remember how fast we were launched off of a moving object.
        }

        //RaycastHit headHit;
        // Make sure the camera doesn't get pushed into a ceiling.
        //if (RaycastForHeadroom (out headHit)) {
        //if (headHit.distance < GetComponent<MouseLook> ().view.localPosition.y) {
        //GetComponent<MouseLook> ().view.localPosition = new Vector3 (0f, headHit.distance, 0f);
        //}
        //}

        // Push ourselves out of nearby objects.
        RecursivePushback (0, MaxPushbackIterations);
        HandleWallRunCollision ();
        PlayerMove ();
    }
    // Slide off of impacting surface
    // This is just projecting a vector onto a plane (our velocity), check wikipedia or purple math if you want to confirm.
    private Vector3 ClipVelocity (Vector3 vel, Vector3 normal) {
        float overbounce = 1.0f; // How much to bounce off the surface, 1.0 means we just slide normally. 2.0 would bounce us off.
        float backoff;
        Vector3 change;
        Vector3 outvel;

        // Determine how far along plane to slide based on incoming direction.
        backoff = Vector3.Dot (vel, normal) * overbounce;

        change = normal * backoff;
        outvel = vel - change;
        // iterate once to make sure we aren't still moving through the plane
        float adjust = Vector3.Dot (outvel, normal);
        if (adjust < 0.0f) {
            outvel -= (normal * adjust);
        }
        return outvel;
    }
    // This command, in a nutshell, scales player input in order to take into account sqrt(2) distortions
    // from walking diagonally.
    public Vector3 GetCommandVelocity () {
        float max;
        float total;
        float scale;
        Vector3 command = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));

        max = Mathf.Max (Mathf.Abs (command.z), Mathf.Abs (command.x));
        if (max <= 0) {
            return new Vector3 (0f, 0f, 0f);
        }

        total = Mathf.Sqrt (command.z * command.z + command.x * command.x);
        scale = max / total;

        return command * scale;
    }
    // Eh
    private void Gravity () {
        velocity += gravity * Time.deltaTime;
    }

    // Checks if we pressed the jump button, oh also checks if you are suddenly launched into the air.
    private void CheckJump () {
        // Check to make sure we have a ground under us, and that it's stable ground.
        if (TryJump() && groundEntity && groundNormal.y > 0.7f) {
            queueHop = false;
            // Right before we jump, lets clip our velocity real quick. That way if we're jumping down a sloped surface, we go faster!
            Vector3 vels = new Vector3 (velocity.x, 0, velocity.z);
            velocity = ClipVelocity (velocity, groundNormal);
            // Only use the new velocity if it made us faster! Don't punish players for trying to go up slopes...
            if (vels.magnitude > velocity.magnitude) {
                velocity = vels;
            }
            // Play a grunt sound, but only so often.
            if (Time.time - lastGrunt > 0.3) {
                jumpGrunt.Play ();
                lastGrunt = Time.time;
            }
            velocity.y = crouched ? crouchJumpSpeed : jumpSpeed;
            groundEntity = null;
            velocity += groundVelocity;
            groundVelocity = Vector3.zero;
            // We give a certain percentage of the current forward movement as a bonus to the jump speed.  That bonus is clipped
            // to not accumulate over time
            Vector3 commandVel = GetCommandVelocity ();
            float flSpeedAddition = Mathf.Abs (commandVel.z * jumpSpeedBonus);
            float flMaxSpeed = maxSpeed + (maxSpeed * jumpSpeedBonus);
            Vector3 flatvel = new Vector3 (velocity.x, 0, velocity.z);
            float flNewSpeed = (flSpeedAddition + flatvel.magnitude);
            // If we're over the maximum, we want to only boost as much as will get us to the goal speed
            if (flNewSpeed > flMaxSpeed) {
                flSpeedAddition -= flNewSpeed - flMaxSpeed;
            }
            if (commandVel.z < 0.0f) {
                flSpeedAddition *= -1.0f;
            }
            velocity += transform.forward * flSpeedAddition;
            justJumped = true;
        }
        // We were standing on the ground, then suddenly are not.
        if (velocity.y >= jumpSpeed / 2f) {
            // We don't inherit the groundVelocity here, because if we just were bumped slightly off of a moving ground
            // that would allow us to accelerate crazily by just being on unstable ground (like a rigidbody).
            groundEntity = null;
        }
    }
    // This is ran ONLY when you hit the ground. Calculates hit sounds and fall damage values.
    private void CheckFalling () {
        //Debug.Log (fallVelocity);
        // this function really deals with landing, not falling, so early out otherwise
        if (groundEntity == null || groundNormal.y < 0.7f || fallVelocity <= 0f) {
            return;
        }

        // We landed on something solidly, if it has some velocity we need to subtract it from our own.
        // This makes our velocities match up again.
        if (groundVelocity.magnitude > 0) {
            velocity -= groundVelocity;
        }
        // If we're falling faster than our fallSoundThreshold, we play a sound.
        if (fallVelocity >= fallSoundThreshold) {
            float fvol = Mathf.Min ((fallVelocity - fallSoundThreshold) / (maxSafeFallSpeed - fallSoundThreshold), 1f);
            RaycastHit hit;
            if (RaycastForGround (out hit)) {
                PlayerRoughLandingEffects (fvol, hit.point, hit.normal);
            }
        }
        if (fallVelocity >= fallPunchThreshold) {

            // Scale it down if we landed on something that's floating...
            //if ( player->GetGroundEntity()->IsFloating() ) {
            //      player->m_Local.m_flFallVelocity -= PLAYER_LAND_ON_FLOATING_OBJECT;
            //}

            //
            // They hit the ground.
            //

            // Player landed on a descending object. Subtract the velocity of the ground entity.
            if (groundVelocity.y < 0f) {
                fallVelocity += groundVelocity.y;
                fallVelocity = Mathf.Max (0.1f, fallVelocity);
            }

            // Calculate camera shake amounts.
            float shakeIntensity = Mathf.Min ((fallVelocity - fallPunchThreshold) / (maxSafeFallSpeed - fallPunchThreshold), 1f);
            gameObject.SendMessage ("ShakeImpact", Vector3.down * shakeIntensity);

            if (fallVelocity > maxSafeFallSpeed) {
                //
                // If they hit the ground going this fast they may take damage (and die).
                //
                justTookFallDamage = true;
                hardLand.Play ();
                //gameObject.SendMessage("Damage", (fallVelocity - maxSafeFallSpeed)*5f );
            }
            // Linearly scale the impact volume with how fast we hit.
        }

        // Clip our velocity, even if we landed on solid ground, we might gain or lose speed depending on the slope...
        // Jumping also clips our velocity, we only want to do it once so we check to make sure we're not jumping.
        if (!TryJump()) {
            velocity = ClipVelocity (velocity, groundNormal);
        }
        //
        // Clear the fall velocity so the impact doesn't happen again.
        //
        fallVelocity = 0;
    }

    // Cast a ray to determine materials, then play the appropriate sounds at the location.
    private void PlayerRoughLandingEffects (float volume, Vector3 hitpos, Vector3 hitnormal) {
        RaycastHit hit;
        if (Physics.Raycast (hitpos + hitnormal * 0.1f, -hitnormal, out hit, 1f)) {
            AudioSource.PlayClipAtPoint (ImpactSounds.GetSound (Helper.getMaterial (hit)), hitpos, volume);
        }
    }

    // Applies friction.
    private void Friction () {
        float speed, newspeed, control;
        float friction;
        float drop;

        // Calculate speed
        speed = velocity.magnitude;

        // If too slow, return
        if (speed < 0.001f) {
            return;
        }

        drop = 0;
        // apply ground friction
        if (groundEntity != null && !TryJump()) { // On an entity that is the ground
            friction = baseFriction * groundFriction;

            // Bleed off some speed, but if we have less than the bleed
            //  threshold, bleed the threshold amount.
            control = (speed < groundDecellerate) ? groundDecellerate : speed;

            // Add the amount to the drop amount.
            drop += control * friction * Time.deltaTime;
        }

        // scale the velocity
        newspeed = speed - drop;
        if (newspeed < 0) {
            newspeed = 0;
        }

        if (newspeed != speed) {
            // Determine proportion of old speed we are using.
            newspeed /= speed;
            // Adjust velocity according to proportion.
            velocity *= newspeed;
        }
    }
    // Make sure an external script didn't NaN our velocity, also ensures we're not going over our speed limit horizontally.
    private void CheckVelocity () {
        int i;
        for (i = 0; i < 3; i++) {
            // See if it's bogus.
            if (float.IsNaN (velocity [i])) {
                Debug.Log ("Got a NaN velocity.");
                velocity [i] = 0;
            }
        }
        /*float savedy = velocity.y;
        velocity.y = 0;
        if (velocity.magnitude > maxSpeed) {
            velocity = Vector3.Normalize (velocity) * maxSpeed;
        }
        velocity.y = savedy;*/
    }
    // Detect which movement code we should run, and set up our parameters for it.
    private void PlayerMove () {
        CheckFalling ();
        // If we are not on ground, store off how fast we are moving down
        if (groundEntity == null) {
            fallVelocity = -velocity.y;
        }
        CheckCrouched ();
        // Was jump button pressed?
        CheckJump ();
        // Make sure we're standing on solid ground

        if (Vector3.Angle (groundNormal, new Vector3 (0f, 1f, 0f)) > controller.slopeLimit) {
            groundEntity = null;
        }
        // Friction is handled before we add in any base velocity. That way, if we are on a conveyor, 
        //  we don't slow when standing still, relative to the conveyor.
        if (groundEntity != null) {
            velocity.y = 0;
            Friction ();
        }

        // Make sure velocity is valid.
        CheckVelocity ();

        // Make sure we are not doing any other type of movement while 
        // wall running 
        if (!wallRunning) {
            if (groundEntity != null) {
                WalkMove ();
            } else {
                AirMove ();  // Take into account movement when in air.
                WallMove (); // Init wall movement
            }
        } else {
            WallMove (); // Continue wall running.
        }
        // Set final flags.
        //CategorizePosition();

        // Make sure velocity is still valid.
        CheckVelocity ();

        Gravity ();

        // If we are on ground, no downward velocity.
        if (groundEntity != null) {
            velocity.y = 0;
        }

        if (wallEntity != null && wallRunning == true) {
            // Wall Running Gravity
            velocity.y = wallGravity * Time.deltaTime; 
        }
    }
    // Smoothly transform our velocity into wishdir*max_velocity at the speed of accel
    private void Accelerate (Vector3 wishdir, float accel, float max_velocity) {
        float addspeed, accelspeed, currentspeed;
        // Determine veer amount
        currentspeed = Vector3.Dot (velocity, wishdir);

        // See how much to add
        addspeed = max_velocity - currentspeed;

        // If not going to add any speed, done.
        if (addspeed <= 0) {
            return;
        }

        // Determine amount of accleration.
        accelspeed = accel * Time.deltaTime * max_velocity;

        // Cap at addspeed
        if (accelspeed > addspeed) {
            accelspeed = addspeed;
        }

        velocity += accelspeed * wishdir;
    }
    // Try to keep ourselves on the ground
    private void StayOnGround () {
        Vector3 savePos = transform.position;
        ignoreCollisions = true;
        controller.Move (new Vector3 (0, -(stepSize+0.1f), 0));
        ignoreCollisions = false;
        RaycastHit outhit;
        if (!RaycastForGround (out outhit)) { // If we slid into the air, discard the move.
            transform.position = savePos;
        }
    }
    // Movement for when on the ground walking/running.
    private void WalkMove () {
        //int i;
        Vector3 wishDir;
        float fmove, smove;

        //Vector3 dest;
        Vector3 forward, right;

        //AngleVectors (mv->m_vecViewAngles, &forward, &right, &up);  // Determine movement angles
        forward = transform.forward;
        right = transform.right;

        // Copy movement amounts
        Vector3 command = GetCommandVelocity ();
        fmove = command.z; // Forward/backward
        smove = command.x; // Left/right

        // Zero out z components of movement vectors
        forward.y = 0;
        right.y = 0;

        forward = Vector3.Normalize (forward);  // Normalize remainder of vectors.
        right = Vector3.Normalize (right);    // 

        // Determine x and y parts of velocity
        wishDir = forward * fmove + right * smove;
        wishDir.y = 0;             // Zero out z part of velocity

        // Set pmove velocity
        Accelerate (wishDir, crouched ? crouchAcceleration : groundAccelerate, crouched ? crouchSpeed : walkSpeed);

        // Add in any base velocity to the current velocity.
        velocity += groundVelocity;

        //controller.Move(velocity*Time.deltaTime
        StepMove ();

        // Now pull the base velocity back out.   Base velocity is set if you are on a moving object, like a conveyor (or maybe another monster?)
        velocity -= groundVelocity;

        StayOnGround ();
    }
    // Movement code for when we're in the air.
    private void AirMove () {
        //int                   i;
        Vector3 wishdir;
        float fmove, smove;
        Vector3 forward, right;

        // Determine movement angles
        forward = transform.forward;
        right = transform.right;

        // Copy movement amounts
        Vector3 command = GetCommandVelocity ();
        fmove = command.z; // Forward/backward
        smove = command.x; // Left/right

        // Zero out up/down components of movement vectors
        forward.y = 0;
        right.y = 0;
        Vector3.Normalize (forward);  // Normalize remainder of vectors
        Vector3.Normalize (right);    // 

        wishdir = forward * fmove + right * smove;
        wishdir.y = 0;             // Zero out up/down part of velocity

        //
        // clamp to server defined max speed
        //
        /*if (wishspeed != 0 && (wishspeed > maxSpeed)) {
            wishvel = wishvel * maxSpeed / wishspeed;
            wishspeed = maxSpeed;
        }*/

        // Check how our wish direction compares to our velocity
        Vector3 flatvel = new Vector3 (velocity.x, 0, velocity.z);
        float check = Vector3.Dot (Vector3.Normalize (flatvel), wishdir);

        if (Mathf.Abs (check) < 0.75f) { // Trying to air-strafe, do air-strafey stuff.
            // Apply air breaks, this keeps our turning really REALLY **REALLY** sharp.
            // It also basically enables or disables surfing. Turning it off makes it feel really bad.
            float airbreak = 1f / Time.deltaTime;
            // Future Dalton, YES this needs to use velocity, not flatvel, or it doesn't bring the character to a complete stop on that axis. (+-.1)
            // You've tested it a thousand times stop changing it back to flatvel. You think "OH IT SHOULDN'T MATTER BECAUSE OF wishdir.y == 0"
            // but Vector3.Dot() uses some approximation mumbo jumbo that makes it much more accurate to use velocity.
            float airBreakMag = -Vector3.Dot (velocity, wishdir);
            Accelerate (wishdir, airbreak, airBreakMag);

            // Then calculate how much we should air-strafe.
            float airStrafe = (1f - Mathf.Abs (check)) * airStrafeAccelerate;
            // We don't want to accelerate just because they pressed A or D, we need them to move their mouse a little also.
            float wishStrafeSpeed = (Mathf.Abs (check) + 0.25f) / 1.25f;
            Accelerate (wishdir, airStrafe, wishStrafeSpeed * flySpeed);

            // The stuff commented out here is used to cheat and give the player speed if they airstrafe
            // The player already recieves speed, but that bonus decreases naturally as you hit a certain threshold.
            // This eliminates that threshold and continues to accelerate the player regardless of what speed they're at.
            // That's not really something we want so, it's gone now.
            //float fcheck = Mathf.Abs (check);
            /*Vector3 pvel = velocity;
            pvel.y = 0f;
            // Give the player a speed bonus based on how they move a mouse.
            if (fcheck > 0.001f && pvel.magnitude > 0.1f) { // Only give the speed bonus if we're moving, otherwise we oscillate like crazy.
                fcheck -= 0.001f;
                fcheck /= 0.999f;
                fcheck *= 5f;
                float speedFunc;
                // Use a disjointed function to check how much speed to gain.
                if (fcheck > 0 && fcheck <= 0.1f) { // Exponentially scales up
                    speedFunc = (fcheck * 10f) * (fcheck * 10f);
                } else { // Logarithmically scales down
                    speedFunc = -Mathf.Log (fcheck);
                }
                float bonusSpeed = airSpeedBonus * speedFunc;
                float yvel = velocity.y;
                velocity = Vector3.Normalize (pvel) * (pvel.magnitude + bonusSpeed);
                velocity.y = yvel;
            }*/
        } else if ( check < -0.75f ) { // Give an acceleration bonus based on if they're trying to stop.
            Accelerate (wishdir, airAccelerate * airBreak, flySpeed);
        } else { // Just trying to move forward, accelerate normally.
            Accelerate (wishdir, airAccelerate, flySpeed);
        }


        // Add in any base velocity to the current velocity.
        velocity += groundVelocity;
        controller.Move (velocity * Time.deltaTime);
        // Now pull the base velocity back out.   Base velocity is set if you are on a moving object, like a conveyor (or maybe another monster?)
        velocity -= groundVelocity;
    }


    private void WallMove () {
        Vector3 tempVel = velocity;
        tempVel.y = 0;
        if (Input.GetKey (KeyCode.LeftShift) && wallEntity != null && (tempVel.magnitude > 0 || tempVel.magnitude < 0)) {
            velocity = ClipVelocity (velocity, wallNormal);
            Accelerate (velocity, wallAccelerate, wallSpeed);
            controller.Move (velocity * Time.deltaTime);
            wallRunning = true;
        } else {
            wallRunning = false;
        }
    }


    // Either the character controller moved into something, or something moved into the supercollider spheres.
    private void HandleCollision (GameObject obj, Vector3 hitNormal, Vector3 hitPos) {
        if (ignoreCollisions) {
            //Debug.Log ("Ignoring collsion because we're ignorin."+Time.time);
            return;
        }
        if ((layerMask & (1 << obj.layer)) == 0) {
            //Debug.Log ("Ignoring collsion of object with " + obj.layer);
            return;
        }
        if (ignoreFootCollisions && hitPos.y-stepSize < transform.position.y - distToGround) {
            //Debug.Log ("Ignoring collsion because its feet."+hitPos.y+0.01f + " " + (transform.position.y - distToGround + stepSize));
            return;
        }
        contacts.Add( new ContactPoint( obj, hitNormal, hitPos ) );
        if (hitNormal.y > 0.7) { // We ignore collisions of valid ground.
            return;
        }
        //Helper.DrawLine(hitPos,hitPos+hitNormal, Color.red, 10f);
        float mag = velocity.magnitude;
        velocity = ClipVelocity (velocity, hitNormal);
        Movable check = obj.GetComponent<Movable> ();
        if (check != null) {
            Vector3 vel = check.velocity;
            float d = Vector3.Dot (vel, hitNormal); // How similar is our velocity to our hitnormal (perp = 0, backwards = -1, same = 1)
            if (d > 0.01f) { // If the velocity should be applied
                velocity += hitNormal * d * overbounce; // We apply it with some overbounce, to keep us from getting stuck.
            }
        }
        float change = Mathf.Abs (mag - velocity.magnitude);
        Rigidbody rigidcheck = obj.GetComponent<Rigidbody> ();
        if (rigidcheck != null) {
            Vector3 vel = rigidcheck.GetPointVelocity (hitPos);
            float d = Vector3.Dot (vel, hitNormal);
            if (d > 0.01f) {
                velocity += hitNormal * d * overbounce;
            }
            rigidcheck.AddForceAtPosition (-hitNormal * change * mass, hitPos);
        }
        if (change > fallSoundThreshold) {
            float fvol = Mathf.Min (change / (maxSafeFallSpeed - fallSoundThreshold), 1f);
            PlayerRoughLandingEffects (fvol, hitPos, hitNormal);
        }
        if (change > fallPunchThreshold) {
            float shakeIntensity = Mathf.Min ((change - fallPunchThreshold) / (maxSafeFallSpeed - fallPunchThreshold), 1f);
            gameObject.SendMessage ("ShakeImpact", -hitNormal * shakeIntensity);
        }
    }

    void OnControllerColliderHit (ControllerColliderHit hit) {
        HandleCollision (hit.gameObject, hit.normal, hit.point);
    }

    void HandleWallRunCollision () {
        //TODO Find better way to do this.
        RaycastHit hitInfo;
        if (Physics.SphereCast (this.transform.position, 0.2f, transform.right, out hitInfo, 0.45f, layerMask)) {
            if (hitInfo.normal.y < 0.7) {
                wallEntity = hitInfo.collider.gameObject;
                wallNormal = hitInfo.normal;
            } else
                wallEntity = null;
        } else if (Physics.SphereCast (this.transform.position, 0.2f, -transform.right, out hitInfo, 0.45f, layerMask)) {
            if (hitInfo.normal.y < 0.7) {
                wallEntity = hitInfo.collider.gameObject;
                wallNormal = hitInfo.normal;
            } else
                wallEntity = null;
        } else {
            wallEntity = null;
        }

    }
    
    // This function makes sure we don't phase through other colliders. (Since character controller doesn't provide this functionality lmao).
    // I copied it from https://github.com/IronWarrior/SuperCharacterController
    // I changed it a bit, but SuperCharacterController is under the MIT license, meaning we can't use it without making our game also under the MIT license.
    // so TODO: Change this enough that we don't have to use the MIT license if we don't want to.
    private void RecursivePushback (int depth, int maxDepth) {
        bool contact = false;
        foreach (var sphere in spheres) {
            foreach (Collider col in Physics.OverlapSphere(SpherePosition(sphere), sphere.radius, layerMask, QueryTriggerInteraction.Ignore)) {
                if (col.isTrigger) {
                    continue;
                }
                Vector3 position = SpherePosition (sphere);
                Vector3 contactPoint;
                bool contactPointSuccess = SuperCollider.ClosestPointOnSurface (col, position, radius, out contactPoint);

                if (!contactPointSuccess) {
                    return;
                }

                Vector3 v = contactPoint - position;
                if (v != Vector3.zero) {
                    // Cache the collider's layer so that we can cast against it
                    int layer = col.gameObject.layer;
                    col.gameObject.layer = TemporaryLayerIndex;
                    // Check which side of the normal we are on
                    bool facingNormal = Physics.SphereCast (new Ray (position, v.normalized), TinyTolerance, v.magnitude + TinyTolerance, 1 << TemporaryLayerIndex, QueryTriggerInteraction.Ignore);
                    col.gameObject.layer = layer;

                    // Orient and scale our vector based on which side of the normal we are situated
                    if (facingNormal) {
                        if (Vector3.Distance (position, contactPoint) < radius) {
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
                    Physics.SphereCast (new Ray (position + v, contactPoint - (position + v)), TinyTolerance, out normalHit, 1 << TemporaryLayerIndex);
                    col.gameObject.layer = layer;
                    HandleCollision (col.gameObject, normalHit.normal, normalHit.point);
                }
            }
        }
        if (depth < maxDepth && contact) {
            RecursivePushback (depth + 1, maxDepth);
        }
    }

    private void StepMove () {
        //Temporarily ignore collisions.
        ignoreCollisions = true;
        ignoreFootCollisions = true;
        // Try sliding forward both on ground and slide forward after being offset by stepSize
        //  take the move that goes farthest
        Vector3 savePos = transform.position;
        // Move normally, then save that position.
        controller.Move (velocity * Time.deltaTime);
        Vector3 groundMove = transform.position;
        // Reset
        transform.position = savePos;
        // Move straight up,
        controller.Move (new Vector3 (0f, stepSize, 0f));
        // Then move normally.
        controller.Move (velocity * Time.deltaTime);
        // Snap back to the ground
        controller.Move (new Vector3 (0f, -stepSize, 0f));
        // Save this position
        Vector3 stepMove = transform.position;
        RaycastHit hit;
        // If we step-moved onto unstable ground, or into the air.. use the original move.
        // Or if we managed to move backwards from the step move. (possible because the top of our capsule head can push us out from under stuff...)
        // Or if we managed to step higher than we want. (possible because the bottom of our capsule can slide up..)
        // Actually now the capsule shouldn't slide up, I cap the stepSize to be .05f less than the radius of the
        // bottom of the capsule, which will keep it from sliding up.
        // I'm going to keep the check for sanity's sake though.
        if (!RaycastForGround (out hit) || Vector3.Dot (stepMove - savePos, velocity) < 0 || Mathf.Abs (savePos.y - stepMove.y) > stepSize) {
            // Move normally
            transform.position = savePos;
            ignoreCollisions = false;
            controller.Move (velocity * Time.deltaTime);
            ignoreFootCollisions = false;
            return;
        }

        // Redo the step move, this time registering collisions.
        transform.position = savePos;
        ignoreCollisions = false;
        controller.Move (new Vector3 (0f, stepSize, 0f));
        controller.Move (velocity * Time.deltaTime);
        controller.Move (new Vector3 (0f, -stepSize, 0f));
        // Enable collisions
        ignoreCollisions = false;
        ignoreFootCollisions = false;
    }
    // Calculates the position of the sphere, probably unnecessary, it's just how they do it from the code i copied.
    public Vector3 SpherePosition (CollisionSphere sphere) {
        return transform.position + sphere.offset * transform.up;
    }
    // When we take damage from anything.
    void Damage (float damage) {
        health -= damage;
        // play a pain grunt.
        painGrunt.Play ();
        if (health <= 0f) {
            // die
            // gameObject.SetActive(false);
            // Instantiate(deathSpawn, transform.position, Quaternion.identity);
            GameManager.instance.Died ();
        }
    }
}

public class ContactPoint {
    public Vector3 point;
    public Vector3 hitNormal;
    public GameObject obj;
    public ContactPoint( GameObject o, Vector3 p, Vector3 hitNormal ) {
        this.obj = o;
        this.point = p;
        this.hitNormal = hitNormal;
    }
}

public class CollisionSphere {
    public float offset;
    public float radius;

    public CollisionSphere (float offset, float radius) {
        this.offset = offset;
        this.radius = radius;
    }
}
