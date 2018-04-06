using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAnimator : MonoBehaviour {

    public int emotes;
    public float emoteMinDelay;
    public float emoteMaxDelay;

    private Animator anim;

	public void Start()
	{
        anim = GetComponent<Animator>();
        Debug.Assert(anim != null);
        Invoke("Emote", 3f);
    }

    public void Emote() {
        anim.SetInteger("emote", Random.Range(1,emotes));
        anim.SetTrigger("startEmote");
        Invoke("Emote", Random.Range(emoteMinDelay, emoteMaxDelay));
    }
}
