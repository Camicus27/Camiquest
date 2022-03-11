using System.Collections;
using UnityEngine;

public class AttemptToLeave : Collidable
{
    // Have some sort of game state tracker here. Once this is end game, the collision ends the game
    public bool hasTalkedToOldMan;
    public bool hasBeatTheGame;
    private Animator anim;

    protected override void OnCollide()
    {
        // Set the player to cutscene movement
        GameManager.instance.player.GetComponent<Player>().canMove = false;
        GameManager.instance.player.GetComponent<Player>().isMoving = false;
        GameManager.instance.player.enabled = false;
        GameManager.instance.playerObj.GetComponent<CircleCollider2D>().enabled = false;
        anim = GameManager.instance.player.GetAnimator();
        // Update animation state
        anim.SetBool("Moving", false);
        anim.SetFloat("Vertical", -1);
        anim.SetFloat("LastMoveVert", -1);
        anim.SetFloat("Horizontal", 0);
        anim.SetFloat("LastMoveHor", 0);

        // If the player has not beaten the game
        if (!hasTalkedToOldMan && !hasBeatTheGame)
        {
            StartCoroutine(CannotLeaveBeginningOfTheGame());
        }
        // The player hasn't beaten the game but has at least spoken to the old man
        else if (hasTalkedToOldMan && !hasBeatTheGame)
        {
            StartCoroutine(CannotLeaveMiddleOfTheGame());
        }
        // End the game
        else
        {
            StartCoroutine(EndTheGame());
        }
    }

    private IEnumerator CannotLeaveBeginningOfTheGame()
    {
        GameManager.instance.playerObj.GetComponent<Rigidbody2D>().velocity = new Vector2(0, .3f);
        yield return new WaitForSeconds(.1f);
        GameManager.instance.playerObj.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

        string message = "I still want to discover the secrets of this ancient structure.";
        yield return GameManager.instance.ShowTextInfo(message);
        message = "The call for adventure is far too great to pass up.";
        yield return GameManager.instance.ShowTextInfo(message);

        anim.SetFloat("Vertical", 1);
        anim.SetFloat("LastMoveVert", 1);
        // Resume the player's movement
        GameManager.instance.player.GetComponent<Player>().canMove = true;
        GameManager.instance.player.GetComponent<Player>().isMoving = true;
        GameManager.instance.player.enabled = true;
        GameManager.instance.playerObj.GetComponent<CircleCollider2D>().enabled = true;
    }

    private IEnumerator CannotLeaveMiddleOfTheGame()
    {
        GameManager.instance.playerObj.GetComponent<Rigidbody2D>().velocity = new Vector2(0, .3f);
        yield return new WaitForSeconds(.1f);
        GameManager.instance.playerObj.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

        string message = "That man still needs my help. As a hero, I can't deny that request.";
        yield return GameManager.instance.ShowTextInfo(message);
        message = "I should go back and see if I can find his son.";
        yield return GameManager.instance.ShowTextInfo(message);

        anim.SetFloat("Vertical", 1);
        anim.SetFloat("LastMoveVert", 1);
        // Resume the player's movement
        GameManager.instance.player.GetComponent<Player>().canMove = true;
        GameManager.instance.player.GetComponent<Player>().isMoving = true;
        GameManager.instance.player.enabled = true;
        GameManager.instance.playerObj.GetComponent<CircleCollider2D>().enabled = true;
    }

    private IEnumerator EndTheGame()
    {
        // Fade to black
        yield return GameManager.instance.FadeOut();

        // End the game.
    }
}
