using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfMutator : MonoBehaviour {

    SourcePlayer player;
    bool hitGround = false;
    bool inAir = false;
    int score = 0;
	bool CanWalk = false;
	bool justHitGround = true;
	bool justJumped = false;
	// Use this for initialization
	void Start () {
		player = GetComponent<SourcePlayer> ();
		CanWalk = false;
	}
	
	// Update is called once per frame
	void Update () {
		player.StopPlayer = !CanWalk;
		if (inAir)
			player.StopPlayer = false;

			
		
		if (player.groundEntity == null) {
			inAir = true;
			if (player.jumpGround != null) 
			{
				if(justHitGround == false)
				{
					score += 1;
					print (score);
				}
				else
				{
					justHitGround = false;
				}
			}

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
				print (score);
                hitGround = false;
				justHitGround = true;
            }
        }

		if (player.jumpGround != null) {
				
		}

	}

	public void ChangeWalk(bool canWalk)
	{
		CanWalk = canWalk;
	}
    
}
