using System.Collections;
using UnityEngine;

/// <summary>
/// Door that can either be freely opened/closed,
/// controlled by a physical lever or levers,
/// or controlled internally by giving some object a lever reference
/// and controlling the lever through script.
/// 
/// If for example, a door opens after a certain enemy is killed, that enemy
/// can hold a lever reference and set behindTheScenesEvent to true, then whenever
/// that enemy is killed, set the lever.ON to true and the door will open!
/// (be sure to add the internal lever object to the door's levers list)
/// </summary>
public class Door : Interactable
{
    private Animator animate;
    public bool leverLocked = false;
    public Lever[] levers;
    private bool hasInteracted = false;
    private bool firstOpenAfterUnlock = true;
    public bool isOpen = false;
    private string message;

    void Awake()
    {
        animate = GetComponent<Animator>();
    }

    protected override void OnInteraction()
    {
        // Stop the player's movement
        GameManager.instance.StopPlayerMovement();

        StartCoroutine(Interaction());
    }

    /// <summary>
    /// Override with coroutine events to occur on interaction
    /// </summary>
    protected virtual IEnumerator Interaction()
    {
        if (leverLocked && !AllLeversOn())
        {
            hasInteracted = true;
            GameManager.instance.PlaySound("DoorCreak_");

            // Display locked interaction dialog
            if (Random.value < 0.5f) message = "It feels much too heavy to move...";
            else message = "It won't budge...";

            yield return new WaitForSeconds(.75f);
            yield return GameManager.instance.ShowTextInfo(message);
        }
        else if (leverLocked && AllLeversOn() && firstOpenAfterUnlock)
        {
            // Display unlocked interaction dialog
            if (hasInteracted)
            {
                message = "The door feels much lighter now!";
                yield return GameManager.instance.ShowTextInfo(message);
            }

            firstOpenAfterUnlock = false;
            isOpen = true;
            GameManager.instance.PlaySound("DoorOpen");

            animate.SetTrigger("UseDoor");
        }
        else
        {
            if (isOpen)
            {
                isOpen = false;
                GameManager.instance.PlaySound("DoorClose");
            }
            else
            {
                isOpen = true;
                GameManager.instance.PlaySound("DoorOpen");
            }

            animate.SetTrigger("UseDoor");
        }

        yield return new WaitForSeconds(0.1f);

        // Resume the player's movement
        GameManager.instance.ResumePlayerMovement();

        // Restore original state
        currentlyInteracting = false;
    }

    private bool AllLeversOn()
    {
        int numberOfLeversOn = 0;
        foreach (Lever lever in levers)
        {
            if (lever.ON)
                numberOfLeversOn++;
        }
        return numberOfLeversOn == levers.Length;
    }
}