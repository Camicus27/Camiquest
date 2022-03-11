using System.Collections;
using UnityEngine;

public class RedBeard : NPC
{
    public string voice = "RedBeardTyping_";

    protected override void Start()
    {
        isFacingLeft = false;
    }

    protected override IEnumerator Interaction()
    {
        anim.SetBool("isTalking", true);

        if (!hasBeenSpokenTo)
        {
            hasBeenSpokenTo = true;
            // Display first interaction dialog
            message = "Another person! You must help!";
            yield return GameManager.instance.ShowTextDialog(message, npc, voice);
            message = "I came across this structure with my friend and some talking statue let us in.";
            yield return GameManager.instance.ShowTextDialog(message, npc, voice);
            message = "When the older guy outside insisted we help him recover his son, we knew we had to help.";
            yield return GameManager.instance.ShowTextDialog(message, npc, voice);
            message = "We got separated though, after being attacked by some slime-like creature.";
            yield return GameManager.instance.ShowTextDialog(message, npc, voice);
            message = "All I managed to find was this lever... I think it triggers some kind of mechanism.";
            yield return GameManager.instance.ShowTextDialog(message, npc, voice);
            message = "No idea what it actually does, but I've been entertained by the sound while I wait for my friend.";
            yield return GameManager.instance.ShowTextDialog(message, npc, voice);
            message = "Please! If you find him, let him know I'm waiting here for him!";
            yield return GameManager.instance.ShowTextDialog(message, npc, voice);

            // Restore original state
            anim.SetBool("isTalking", false);
            yield return base.Interaction();

            yield break;
        }

        if (Random.value > .5)
        {
            message = "Have you found my friend yet? He's a dwarf. Not too tall, kinda ugly.";
            yield return GameManager.instance.ShowTextDialog(message, npc, voice);
            message = "Please let him know I'm here if you find him!";
            yield return GameManager.instance.ShowTextDialog(message, npc, voice);
        }
        else
        {
            message = "That lever over there seems important for something, not sure what though.";
            yield return GameManager.instance.ShowTextDialog(message, npc, voice);
            message = "See if something happens outside the room, I know nothing changes in here 'cause I've been flipping it a lot.";
            yield return GameManager.instance.ShowTextDialog(message, npc, voice);
        }

        // Restore original state
        anim.SetBool("isTalking", false);
        yield return base.Interaction();
    }
}
