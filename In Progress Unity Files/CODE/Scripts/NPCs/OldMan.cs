using System.Collections;
using UnityEngine;

public class OldMan : NPC
{
    public string voice = "OldManTyping_";
    public GameObject portal;
    public AttemptToLeave exitArea;

    protected override void Start()
    {
        isFacingLeft = true;
        exitArea = GameObject.Find("AttemptToLeave").GetComponent<AttemptToLeave>();
    }

    protected override IEnumerator Interaction()
    {
        anim.SetBool("isTalking", true);

        if (!hasBeenSpokenTo)
        {
            hasBeenSpokenTo = true;
            exitArea.hasTalkedToOldMan = true;
            portal.SetActive(true);
            // Display first interaction dialog
            message = "Oh thank goodness! A warrior!! You MUST help me!";
            yield return GameManager.instance.ShowTextDialog(message, npc, voice);
            message = "I've lost my son! He wandered near this strange structure and got snatched up by " +
            "some large, horrifying beast!";
            yield return GameManager.instance.ShowTextDialog(message, npc, voice);
        }

        // Display dialog
        message = "Please go after the beast and save my son!";
        yield return GameManager.instance.ShowTextDialog(message, npc, voice);

        // Restore original state
        anim.SetBool("isTalking", false);
        yield return base.Interaction();
    }
}
