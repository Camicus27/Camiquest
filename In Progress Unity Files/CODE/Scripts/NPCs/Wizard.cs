using System.Collections;
using UnityEngine;

public class Wizard : NPC
{
    public string voice = "WizardTyping_";
    private bool hasGivenSecret;
    private bool knowsAboutTheShop;

    protected override void Start()
    {
        isFacingLeft = true;
    }

    protected override IEnumerator Interaction()
    {
        anim.SetBool("isTalking", true);

        // Shop has not been found
        if (GameObject.FindGameObjectWithTag("Shop").GetComponent<TheShop>().dialogNumber == 0)
        {
            // First interaction
            if (!hasBeenSpokenTo)
            {
                hasBeenSpokenTo = true;
                hasGivenSecret = true;
                yield return Say("Why, Hello Traveler...");
                yield return Say("I'm surprised people even make it in here anymore...");
                yield return Say("You ought to be careful around here. Things are not always what they seem.");
                yield return Say("I've seen the creatures around here protecting their treasures with enchantments.");
                yield return Say("It almost becomes some sort of puzzle to recover their protected goods.");
                yield return Say("I'll let you in on a little secret, just between you and me...");
                yield return Say("I watched one of those fire creatures go straight through a wall once.");
                yield return Say("They were looking at some stones on the ground then they ran right at the wall! Just phased right through it!");
                yield return Say("They came back out some time later with a bunch of treasures.");
                yield return Say("It may be worth it to run at some walls that have suspiciously placed stones in front of them...");
            }
            // Other interactions
            else
            {
                yield return Say("Did you run at some walls yet?");
                yield return Say("Maybe try looking near some fire creatures.");
            }
        }
        // Shop has been found
        else
        {
            if (!hasBeenSpokenTo)
            {
                hasBeenSpokenTo = true;
                yield return Say("Why, Hello Traveler...");
                yield return Say("I'm surprised people even make it in here anymore...");
                yield return Say("You ought to be careful around here. Things are not always what they seem.");
                yield return Say("I've seen the creatures around here protecting their treasures with enchantments.");
                yield return Say("It almost becomes some sort of puzzle to recover their protected goods.");
                yield return Say("I'll let you in on a little secret, just between you and me...");
                yield return Say("I watched one of those fire creatures go straight through a wall once.");
                yield return Say("They were looking at some stones on the ground then they ran right at the wall! Just phased right through it!");
                yield return Say("They came back out some time later with a bunch of treasures.");
                message = "I've really been meaning to go investigate whatever is behind that wall.";
                yield return GameManager.instance.ShowTextPrompt(message, voice, 25, "I went through!", "You sound crazy.");
                // I went through
                if (GameManager.instance.selection == 0)
                {
                    yield return Say("Oh, you did? That's actually very impressive you found it! Didn't even need my help!");
                    yield return Say("... what's that? You found a shop?");
                    yield return Say("How peculiar. That would explain where the fire creature got all that treasure from.");
                    yield return Say("They must've gone to the same place as you! Well that's pretty neat!");
                    yield return Say("Here, have this for solving a long standing mystery for me.");
                    yield return Say("Good luck on your journey, and thanks again!");
                    yield return new WaitForSeconds(1f);
                    yield return GameManager.instance.ShowTextInfo("* You received 30 gold!");
                    GameManager.instance.ReceiveGold(30);
                    knowsAboutTheShop = true;
                }
                // Youre crazy
                else if (GameManager.instance.selection == 1)
                {
                    yield return Say("Ah... well what exactly are you still doing talking to a crazy old man?");
                    isInteractable = false;
                }
            }
            else if (hasBeenSpokenTo && hasGivenSecret)
            {
                message = "Have you investigated the walls?";
                yield return GameManager.instance.ShowTextPrompt(message, voice, 25, "I went through!", "Walls are solid.");
                // I went through
                if (GameManager.instance.selection == 0)
                {
                    yield return Say("Oh, you did? Well tell me what you found!");
                    yield return Say("... what's that? You found a shop?");
                    yield return Say("How peculiar. That would explain where the fire creature got all that treasure from.");
                    yield return Say("They must've gone to the same place as you! Well that's pretty neat!");
                    yield return Say("Here, have this for solving a long standing mystery for me.");
                    yield return Say("Good luck on your journey, and thanks again!");
                    yield return new WaitForSeconds(1f);
                    yield return GameManager.instance.ShowTextInfo("* You received 25 gold!");
                    GameManager.instance.ReceiveGold(25);
                    knowsAboutTheShop = true;
                }
                // Walls are solid
                else if (GameManager.instance.selection == 1)
                {
                    yield return Say("Ah... well I know what I saw, guess they sealed it up!");
                    isInteractable = false;
                }
            }
            else if (hasBeenSpokenTo && knowsAboutTheShop)
            {
                yield return Say("How is the journey going? Finding lots a loot I hope!");
                yield return Say("I'll let you know if I see more suspicious activities.");
            }
        }

        // Restore original state
        anim.SetBool("isTalking", false);
        yield return base.Interaction();
    }

    private IEnumerator Say(string msg)
    { yield return GameManager.instance.ShowTextDialog(msg, npc, voice); }
}
