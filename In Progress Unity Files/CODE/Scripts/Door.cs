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
    public int ID;
    public bool leverLocked = false;
    public Lever[] levers;
    private bool hasInteracted = false;
    private bool firstOpenAfterUnlock = true;
    public bool isOpen = false;
    private string message;

    public DoorData doorData;

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
            float val = Random.value;
            if (levers.Length > 1)
            {
                if (val < 0.5f) message = "Seems there may be multiple levers used for this door...";
                else message = "It feels like multiple mechanisms are holding this door in place...";
            }
            else
            {
                if (val >= .66f) message = "It feels much too heavy to move...";
                else if (.66f > val && val >= .33f) message = "It won't budge...";
                else message = "Seems some mechanism is holding it in place...";
            }

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

        Save();
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

    public void LoadData(DoorData data)
    {
        doorData = data;

        hasInteracted = doorData.hasInteracted;
        firstOpenAfterUnlock = doorData.firstOpenAfterUnlock;
        isOpen = doorData.isOpen;
        if (isOpen)
            animate.SetTrigger("UseDoor");
    }

    public void Save()
    {
        // Perform save
        doorData.ID = ID;
        doorData.hasInteracted = hasInteracted;
        doorData.firstOpenAfterUnlock = firstOpenAfterUnlock;
        doorData.isOpen = isOpen;
        // Add to save data if not already in it
        if (!SaveData.current.doors.Contains(doorData))
            SaveData.current.doors.Add(doorData);
    }
}

[System.Serializable]
public class DoorData
{
    public int ID;
    public bool hasInteracted;
    public bool firstOpenAfterUnlock;
    public bool isOpen;
}