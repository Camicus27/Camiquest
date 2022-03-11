using System.Collections;
using UnityEngine;

public class Inspectable : Interactable
{
    public string[] firstInteractMessages;
    public string[] secondInteractMessages;
    public string[] otherInteractMessages;
    public bool onlyOneInteractionMessage;
    public bool canOnlyHaveOneInteraction;
    protected int interactNumber = 0;

    private static bool hasInteractedWithSomethingTooMuch = false;

    /// <summary>
    /// Freezes player, triggers an inspection
    /// </summary>
    protected override void OnInteraction()
    {
        // Stop the player's movement
        GameManager.instance.StopPlayerMovement();

        StartCoroutine(Interaction());
    }

    private IEnumerator Interaction()
    {
        if (canOnlyHaveOneInteraction)
        {
            // Display the message
            foreach (string message in firstInteractMessages)
                yield return GameManager.instance.ShowTextInfo(message);
            // Resume the player's movement
            GameManager.instance.player.GetComponent<Player>().canMove = true;
            GameManager.instance.invCanBeShown = true;
            // Remove interactability
            isInteractable = false;
            isInRange = false;
            yield break;
        }
        else
        {
            if (onlyOneInteractionMessage)
            {
                foreach (string message in firstInteractMessages)
                    yield return GameManager.instance.ShowTextInfo(message);
            }
            else
            {
                if (interactNumber == 0)
                {
                    foreach (string message in firstInteractMessages)
                        yield return GameManager.instance.ShowTextInfo(message);
                    interactNumber++;
                }
                else if (secondInteractMessages.Length > 0 && interactNumber == 1)
                {
                    foreach (string message in secondInteractMessages)
                        yield return GameManager.instance.ShowTextInfo(message);
                    interactNumber++;
                }
                else if (otherInteractMessages.Length > 0 && interactNumber > 1)
                {
                    foreach (string message in otherInteractMessages)
                        yield return GameManager.instance.ShowTextInfo(message);
                    interactNumber++;
                }
                if (!hasInteractedWithSomethingTooMuch && interactNumber == 99)
                {
                    string message = "Yeesh. Can you PLEASE stop interacting with this? It's exhausting to display.";
                    yield return GameManager.instance.ShowTextInfo(message);
                    message = "You really interacted with this thing 100 times? Get a life, get a hobby, get SOMETHING.";
                    yield return GameManager.instance.ShowTextInfo(message);
                    message = "You know what? You can't interact with this thing anymore, I don't even care. You're done.";
                    yield return GameManager.instance.ShowTextInfo(message);

                    // Remove interactability
                    isInteractable = false;
                    isInRange = false;

                    Animator anim = GameManager.instance.player.GetAnimator();
                    // Update animation state
                    anim.SetBool("Moving", false);
                    anim.SetFloat("Vertical", 0);
                    anim.SetFloat("LastMoveVert", 0);
                    anim.SetFloat("Horizontal", -1);
                    anim.SetFloat("LastMoveHor", -1);

                    // Look left
                    yield return new WaitForSeconds(1f);
                    // Look right
                    anim.SetFloat("LastMoveHor", 1);
                    yield return new WaitForSeconds(1f);
                    // Look at camera
                    anim.SetFloat("LastMoveVert", -1);
                    anim.SetFloat("LastMoveHor", 0);

                    yield return new WaitForSeconds(1.5f);

                    hasInteractedWithSomethingTooMuch = true;
                }
            }
        }

        // Resume the player's movement
        GameManager.instance.ResumePlayerMovement();
        currentlyInteracting = false;
    }
}
