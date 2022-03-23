using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to open doors. Can be physically placed or used
/// internally through reference for other gameObjects to
/// trigger doors as long as behindTheScenesEvent is true.
/// </summary>
public class Lever : Interactable
{
    public bool ON = false;
    public bool behindTheScenesEvent = false;
    private bool hasBeenTurnedOnOnce = false;
    public bool dontDisplayMessage;
    public string message;

    protected override void OnInteraction()
    {
        // Stop the player's movement
        GameManager.instance.StopPlayerMovement();

        // Move to opposite position
        if (!ON)
        {
            ON = true;
            transform.rotation = Quaternion.Inverse(transform.rotation);
            transform.position = new Vector3(transform.position.x + 0.104f, transform.position.y, transform.position.z);
            GameManager.instance.PlaySound("Switch");
        }
        else
        {
            ON = false;
            transform.rotation = Quaternion.Inverse(transform.rotation);
            transform.position = new Vector3(transform.position.x - 0.104f, transform.position.y, transform.position.z);
            GameManager.instance.PlaySound("Switch");
        }

        StartCoroutine(Interaction());
    }

    private IEnumerator Interaction()
    {
        if (!hasBeenTurnedOnOnce && !dontDisplayMessage)
        {
            hasBeenTurnedOnOnce = true;
            // Display interaction dialog
            yield return GameManager.instance.ShowTextInfo(message);
        }

        yield return new WaitForSeconds(0.2f);

        // Resume the player's movement
        GameManager.instance.ResumePlayerMovement();

        // Restore original state
        currentlyInteracting = false;
    }

    protected override void Update()
    {
        if (behindTheScenesEvent)
        {
        }
        else
        {
            base.Update();
        }
    }
}
