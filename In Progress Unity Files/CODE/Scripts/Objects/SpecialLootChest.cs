using System.Collections;
using UnityEngine;

public class SpecialLootChest : Interactable
{
    public Animator anim;
    private string message;

    protected override void OnInteraction()
    {
        // Stop the player's movement
        GameManager.instance.StopPlayerMovement();

        // Collect the item
        anim.SetBool("isCollected", true);
        isInteractable = false;
        isInRange = false;
        currentlyInteracting = false;
        GetComponent<BoxCollider2D>().enabled = false;
        GameManager.instance.PlaySound("SpecialChestCollected");

        StartCoroutine(Interaction());
    }

    private IEnumerator Interaction()
    {
        // Pick a random opening dialog
        float randDialog = Random.value;
        if (randDialog > .8f)
            message = "The glowing fades and some sort of entity phases into you. You feel a new sense of power...";
        else if (.8f > randDialog && randDialog > .5f)
            message = "As the glow fades away, a sudden rush of power surges through you...";
        else
            message = "The bright glow fades and a strange new thought pops into your head...";
        yield return GameManager.instance.textManager.ShowInfo(message);

        // Pick a random new move
        yield return new WaitUntil(PickANewMove);
        yield return GameManager.instance.textManager.ShowInfo(message);

        // Resume the player's movement
        GameManager.instance.ResumePlayerMovement();
    }

    private bool PickANewMove()
    {
        switch (Random.Range(0, 4))
        {
            case 0:
                // Holy Hand Grenade
                GameManager.instance.player.AddMoveToMoveOptions(Moves.GetSpecificMove("PURE", 3));
                message = "* You learned Holy Hand Grenade!";
                break;
            case 1:
                // Bash
                GameManager.instance.player.AddMoveToMoveOptions(Moves.GetSpecificMove("KINETIC", 1));
                message = "* You learned Bash!";
                break;
            case 2:
                // Sludge Bomb
                GameManager.instance.player.AddMoveToMoveOptions(Moves.GetSpecificMove("TOXIC", 0));
                message = "* You learned Sludge Bomb!";
                break;
            case 3:
                // Incorporeal Shot
                GameManager.instance.player.AddMoveToMoveOptions(Moves.GetSpecificMove("UNDEAD", 1));
                message = "* You learned Incorporeal Shot!";
                break;
        }

        return true;
    }
}
