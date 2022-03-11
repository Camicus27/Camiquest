using System.Collections;
using UnityEngine;

public class Statue : Interactable
{
    private string message;
    // FOR DEBUG PURPOSES: [4]
    private int dialogNumber = 0;

    public GameObject cutscenePt1;
    public GameObject cutscenePt2;
    public GameObject cutscenePt3;
    public string voice = "StatueTyping_";
    private int textDisplaySpeed = 20;
    private float lastShown;

    /// <summary>
    /// Freezes player, triggers FirstInteraction, then OtherInteractions
    /// </summary>
    protected override void OnInteraction()
    {
        // Stop the player's movement
        GameManager.instance.StopPlayerMovement();

        StartCoroutine(Interaction());
    }

    private IEnumerator Interaction()
    {
        switch (dialogNumber)
        {
            case 0:
                message = "The statue looks surprisingly new for how long this structure has stood.";
                yield return GameManager.instance.ShowTextInfo(message);
                message = "The base has ancient engravings on it...";
                yield return GameManager.instance.ShowTextInfo(message);

                message = "\"The mark of a true Hero is the ability to instill hope in those they protect.\"";
                yield return GameManager.instance.ShowTextInfo(message);
                message = "\"Heroes seek adventure and triumph. Above all else, they protect those they love.\"";
                yield return GameManager.instance.ShowTextInfo(message);
                GameManager.instance.PlaySound("StoneHeadTilt");
                yield return new WaitForSeconds(1.2f);

                message = "Strange, the statue's head seems to have tilted down to look at you...";
                yield return GameManager.instance.ShowTextInfo(message);

                message = "Do you bear the mark of a true Hero?";
                yield return GameManager.instance.ShowTextPrompt(message, voice, textDisplaySpeed, "Yes.", "No.");
                yield return new WaitForSeconds(.5f);
                // Yes
                if (GameManager.instance.selection == 0)
                {
                    yield return Say("I see...");
                    yield return Say("Then that means you must have a class you belong to, no?");
                    message = "Would you mind telling me your class?";
                    yield return GameManager.instance.ShowTextPrompt(message, voice, textDisplaySpeed, "Fire", "Water", "Earth", "Pure");
                    // FIRE
                    if (GameManager.instance.selection == 0)
                    {
                        GameManager.instance.player.type = "FIRE";
                        GameManager.instance.player.AddMoveToMoveOptions(Moves.GetSpecificMove("FIRE", 0));
                        yield return Say("Ah, Fire. A bold and dangerous class. I lost an old friend to a worthy Fire Hero.");
                    }
                    // WATER
                    else if (GameManager.instance.selection == 1)
                    {
                        GameManager.instance.player.type = "WATER";
                        GameManager.instance.player.AddMoveToMoveOptions(Moves.GetSpecificMove("WATER", 0));
                        yield return Say("Mm, Water. A mysterious and ancient class. The tides... as old as time itself.");
                    }
                    // EARTH
                    else if(GameManager.instance.selection == 2)
                    {
                        GameManager.instance.player.type = "EARTH";
                        GameManager.instance.player.AddMoveToMoveOptions(Moves.GetSpecificMove("EARTH", 0));
                        yield return Say("Hmmm, Earth. A strong and solid class. Strong enough to control the ground we stand on.");
                    }
                    // PURE
                    else
                    {
                        GameManager.instance.player.type = "PURE";
                        GameManager.instance.player.AddMoveToMoveOptions(Moves.GetSpecificMove("PURE", 0));
                        yield return Say("Pure... How fitting to be called a Pure Hero. I do hope you don't tarnish that title.");
                    }
                }
                // No
                else
                {
                    yield return Say("I see...");
                    yield return Say("It was a pleasure meeting you...");
                    yield return Say("You won't remember you ever stumbled upon this ancient structure. I bid you a nice life.");

                    // Game over!
                    // Fade to black
                    yield return GameManager.instance.FadeOut();
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GameOver");
                    yield break;
                }

                yield return Say("I have spoken to many Heroes long before you arrived.");
                yield return Say("Many I have never seen again. I can only assume the worst.");
                message = "Do you really wish to endure the trials many have been bested by before?";
                yield return GameManager.instance.ShowTextPrompt(message, voice, 20, "Yes.", "No.");
                // No
                if (GameManager.instance.selection != 0)
                {
                    yield return Say("I do understand...");
                    yield return Say("It was a pleasure meeting you...");
                    yield return Say("You won't remember you ever stumbled upon this ancient structure. I bid you a nice life.");

                    // Game over!
                    // Fade to black
                    yield return GameManager.instance.FadeOut();
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GameOver");
                    yield break;
                }

                yield return Say("Very well...");
                yield return Say("But be warned. No one, save for a True Hero, may conquer this place and discover it's secrets.");

                lastShown = Time.time;
                GameManager.instance.PlaySound("StoneGrinding");
                yield return new WaitUntil(OpenStairs);

                yield return Say("I do hope to see you again. As I have said many times before...");
                yield return Say("Good luck.");
                yield return new WaitForSeconds(1f);
                dialogNumber++;
                break;
            // After the stair case has opened.
            case 1:
                message = "The statue is back to looking stoically into the distance.";
                yield return GameManager.instance.ShowTextInfo(message);
                break;




            case 4:
                DEBUGGING_CUTSCENE_SKIP();
                break;
        }

        // Resume the player's movement
        GameManager.instance.ResumePlayerMovement();
        currentlyInteracting = false;
    }
    private bool OpenStairs()
    {
        cutscenePt1.SetActive(false);

        if (Time.time - lastShown < 1.5f)
            return false;
        else
            cutscenePt2.SetActive(false);
        if (Time.time - lastShown < 3f)
            return false;
        else
            cutscenePt3.SetActive(false);

        return true;
    }
    private IEnumerator Say(string message)
    { yield return GameManager.instance.textManager.DisplayText(message, voice, textDisplaySpeed, true); }

    private void DEBUGGING_CUTSCENE_SKIP()
    {
        cutscenePt1.SetActive(false);
        cutscenePt2.SetActive(false);
        cutscenePt3.SetActive(false);
    }
}
