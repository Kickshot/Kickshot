using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent( typeof(SourcePlayer) )]
[RequireComponent( typeof(MouseLook) )]
public class SourcePlayerPossession : MonoBehaviour {
    private SourcePlayer player;
    private MouseLook look;
	void Start () {
        player = GetComponent<SourcePlayer> ();
        look = GetComponent<MouseLook> ();
	}
	
	void Update () {
        look.wishXAxis = Input.GetAxisRaw ("Mouse X");
        look.wishYAxis = Input.GetAxisRaw ("Mouse Y");
        player.wishDir = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0f, Input.GetAxisRaw ("Vertical"));
        player.wishJump = Input.GetButton ("Jump");
        player.wishJumpDown = Input.GetButtonDown ("Jump");
        player.wishCrouch = Input.GetButton ("Crouch");
        player.wishSuicideDown = Input.GetButton ("Suicide");
        player.wishDodge = Input.GetButtonDown("Dodge");
		player.wishWallDodge = Input.GetButton("Dodge");
	}
}
