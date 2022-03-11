using System.Collections;
using UnityEngine;

public class NPC : Interactable
{
    public bool isFacingLeft;
    public string message = "";
    protected bool hasBeenSpokenTo = false;
    public Sprite npc;
    public Animator anim;

    /// <summary>
    /// Override this with some basic start up properties
    /// and always include: 
    ///     isFacingLeft = true/false
    ///     message =
    ///     npc = sprite
    /// </summary>
    protected virtual void Start()
    { }

    /// <summary>
    /// Freezes player, faces the player, 
    /// triggers FirstInteraction then OtherInteractions
    /// </summary>
    protected override void OnInteraction()
    {
        // Stop the player's movement
        GameManager.instance.StopPlayerMovement();

        // Move to face the player
        if ((GameManager.instance.player.transform.position - transform.position).x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
        
        StartCoroutine(Interaction());
    }
    

    /// <summary>
    /// Override with coroutine events to occur on interaction,
    /// then call this base event to resume original state
    /// </summary>
    protected virtual IEnumerator Interaction()
    {
        // Resume the player's movement
        GameManager.instance.ResumePlayerMovement();

        // Restore original state
        if (isFacingLeft)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);

        currentlyInteracting = false;

        yield return null;
    }
}
