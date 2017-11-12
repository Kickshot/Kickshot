using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfMutator : MonoBehaviour {

    SourcePlayer player;
    bool hitGround = false;
    bool inAir = false;
    int score = -1;
	bool CanWalk = false;
	// Use this for initialization
	void Start () {
		player = GetComponent<SourcePlayer> ();
		CanWalk = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!CanWalk)
			player.wishDir = new Vector3 (0, 0, 0);


		if (player.groundEntity == null) {
			inAir = true;
		}

		if (inAir) {
			player.jumpBufferTime = 0;
			player.wishJump = false;
		}
		
		if (player.groundEntity != null)
        {
            
            if (inAir == true)
            {
                inAir = false;
                hitGround = true;
            }
            if (hitGround)
            {
                score += 1;
                hitGround = false;
            }
        }

	}

	public void ChangeWalk(bool canWalk)
	{
		CanWalk = canWalk;
	}
    
}
