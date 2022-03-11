using System.Collections;
using UnityEngine;

public class TheShop : Interactable
{
    private string message;
    public int dialogNumber = 0;

    // Speaking vars
    public string voice = "TheShop_";
    private int lettersPerSec = 20;

    private bool hasAskedWho = false;
    private int movesGold = 100;
    private bool move1Bought = false;
    private bool move2Bought = false;
    private bool move3Bought = false;
    private bool move4Bought = false;
    private bool allMovesBought = false;

    /// <summary>
    /// Freezes player, triggers FirstInteraction, then OtherInteractions
    /// </summary>
    protected override void OnInteraction()
    {
        // Stop the player's movement
        GameManager.instance.StopPlayerMovement();

        StartCoroutine(Interaction());
    }

    /// <summary>
    /// At the start, dialogNum is 0
    /// After the first interaction, dialogNum is 1
    /// 
    /// If dialogNum = 5, it opens the interaction, lets the player pick an option (chat[6], buy[7], sell[8], leave[9])
    /// 
    /// If dialogNum = 6, opens chat menu, pick an option (who are you?, what is this place?)
    /// If dialogNum = 7, opens buy menu, pick an option (health, mana, newMove, goBack)
    /// If dialogNum = 8, opens sell menu, pick an option (health, mana, soul, goBack)
    /// If dialogNum = 3, says closing dialog, quits
    /// 
    /// </summary>
    private IEnumerator Interaction()
    {
        // The opening dialog line with first/other interactions
        if (dialogNumber < 2)
        {
            if (dialogNumber == 0)
            {
                message = "Hello there traveler. I see ye discovered ma shop...";
                yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                message = "I don' see many folks around 'ere anymore, care ta buy somethin'? Or maybe jus' chat?";
            }
            else if (dialogNumber == 1)
            {
                float randDialog = Random.value;
                if (randDialog > .8f)
                    message = "Ya got gold? I don' do trades, sorry.";
                else if (.8f > randDialog && randDialog > .6f)
                    message = "What is up wit that old guy?";
                else if (.6f > randDialog && randDialog > .4f)
                    message = "I only buy GOODS, so yer junk better be good...";
                else if (.4f > randDialog && randDialog > .2f)
                    message = "I got a lotta moves I can teach ya!\nAssuming ye can afford my services...";
                else
                    message = "Don' ask where I get ma potions OR why I got so many...";
            }

            yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
            dialogNumber = 5;
            GameManager.instance.textManager.ToggleStatsBox();
            GameManager.instance.textManager.UpdateStatsText("Gold: " + GameManager.instance.GetCurrentGold());
        }

        while (dialogNumber > 4)
        {
            // Player has opened the intro selection
            if (dialogNumber == 5)
            {
                float randDialog = Random.value;
                if (randDialog > .7f)
                    message = "Make it quick, I got places ta be ya know?";
                else if (.7f > randDialog && randDialog > .35f)
                    message = "Alrigh', wha' can I do ya for?";
                else
                    message = "These potions don' jus' make 'emselves ya know...";
                yield return GameManager.instance.ShowTextPrompt(message, voice, lettersPerSec, "Chat", "Buy", "Sell", "Leave");
                // CHAT
                if (GameManager.instance.selection == 0)
                    dialogNumber = 6;
                // BUY
                else if (GameManager.instance.selection == 1)
                    dialogNumber = 7;
                // SELL
                else if (GameManager.instance.selection == 2)
                    dialogNumber = 8;
                // LEAVE
                else
                    dialogNumber = 3;
            }

            // Player has opened the chat
            if (dialogNumber == 6)
            {
                float randDialog = Random.value;
                if (randDialog > .5f)
                    message = "Yeah what? Try not ta ask too many questions.";
                else
                    message = "What's on yer mind?";
                yield return GameManager.instance.ShowTextPrompt(message, voice, lettersPerSec, "Who are you?", "What is this place?");
                // Who
                if (GameManager.instance.selection == 0)
                {
                    if (!hasAskedWho)
                    {
                        hasAskedWho = true;
                        message = "Well... that answer is a tad complicated.";
                        yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                        message = "I know who I am, and nobody else knows who I am.";
                        yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                        message = "I walked by this buildin' when I noticed it appeared one day. Jus' popped up outta nowhere.";
                        yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                        message = "I came to check it out when some old guy asked me abou' my intentions.";
                        yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                        message = "I told 'im \"I wanna setup a shop. Ye expect many visitors?\"";
                        yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                        message = "When he told me he did, that was good 'nough for me! So I been sellin' 'ere since.";
                        yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                        message = "Not much else t' it really... no need ta ask me that again, ya hear?";
                        yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                    }
                    else
                    {
                        message = "I told ya not to ask me that again...";
                        yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                    }
                }
                // What
                else
                {
                    message = "This here place is a buildin' built by who KNOWS what.";
                    yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                    message = "I don' know much abou' it, other than I get business when I need it.";
                    yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                    message = "It's got some strange happenin's goin' on, I mean ya walked through a wall jus' now!";
                    yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                    message = "The creatures that wander the halls are blunt, not up fer much chattin' really.";
                    yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                    message = "Also, while I was away, someone went and put that there flag up. I don' know what that means neither.";
                    yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                }

                dialogNumber = 5;
            }

            // Player has opened the buy menu
            if (dialogNumber == 7)
            {
                while (dialogNumber != 5)
                {
                    float randDialog = Random.value;
                    if (randDialog > .7f)
                        message = "Ya lookin' to learn somethin' new?";
                    else if (.7f > randDialog && randDialog > .3f)
                        message = "Wha'll it be?";
                    else
                        message = "Best potions aroun'... tell yer friends!";
                    yield return GameManager.instance.ShowTextPrompt(message, voice, lettersPerSec, "Health Potion (30g)", "Mana Potion (25g)", "Learn Move (" + movesGold + "g)" , "Go Back");
                    // HEALTH
                    if (GameManager.instance.selection == 0)
                    {
                        if (GameManager.instance.TryPayGold(30))
                        {
                            if (GameManager.instance.player.TryCollectItem(new Item("Health")))
                            {
                                // Give audio queue for purchase
                                GameManager.instance.PlaySound("Purchase");
                                // Update the gold display
                                GameManager.instance.textManager.UpdateStatsText("Gold: " + GameManager.instance.GetCurrentGold());
                            }
                            else
                            {
                                message = "Ehh, ya don' have enough room ta carry this... don' wanna have ya drop it after ya leave.";
                                yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                                GameManager.instance.ReceiveGold(30);
                                GameManager.instance.totalGold -= 30;
                                GameManager.instance.textManager.UpdateStatsText("Gold: " + GameManager.instance.GetCurrentGold());
                            }
                        }
                        else
                        {
                            message = "Sorry, not enough gold... This ain' a charity.";
                            yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                        }
                    }
                    // MANA
                    else if (GameManager.instance.selection == 1)
                    {
                        if (GameManager.instance.TryPayGold(25))
                        {
                            if (GameManager.instance.player.TryCollectItem(new Item("Mana")))
                            {
                                GameManager.instance.PlaySound("Purchase");
                                GameManager.instance.textManager.UpdateStatsText("Gold: " + GameManager.instance.GetCurrentGold());
                            }
                            else
                            {
                                message = "Ehh, ya don' have enough room ta carry this... don' wanna have ya drop it after ya leave.";
                                yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                                GameManager.instance.ReceiveGold(30);
                                GameManager.instance.totalGold -= 30;
                                GameManager.instance.textManager.UpdateStatsText("Gold: " + GameManager.instance.GetCurrentGold());
                            }
                        }
                        else
                        {
                            message = "Sorry, not enough gold... This ain' a charity.";
                            yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                        }
                    }
                    // LEARN
                    else if (GameManager.instance.selection == 2)
                    {
                        if (allMovesBought)
                        {
                            message = "Yeah... I ran outta moves ta teach ya. I really gotta pick up some new skills.";
                            yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                            continue;
                        }

                        message = "That'll be " + movesGold + " gold.";
                        yield return GameManager.instance.ShowTextPrompt(message, voice, lettersPerSec, "Deal.", "Never mind.");
                        // Deal
                        if (GameManager.instance.selection == 0)
                        {
                            if (GameManager.instance.TryPayGold(movesGold))
                            {
                                // Update gold display
                                GameManager.instance.textManager.UpdateStatsText("Gold: " + GameManager.instance.GetCurrentGold());

                                message = "Thank ya very much!";
                                yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                                yield return GivePlayerAMove(Random.Range(1, 5));
                                movesGold += 100;
                            }
                            else
                            {
                                message = "Sorry, not enough gold... This ain' a charity.";
                                yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                            }
                        }
                        // No
                        else
                        {
                            message = "No worries, but try learning this somewhere else! I dare ya!";
                            yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                        }
                    }
                    // GO BACK
                    else
                    {
                        dialogNumber = 5;
                    }
                }
            }

            // Player has opened the sell menu
            if (dialogNumber == 8)
            {
                while (dialogNumber != 5)
                {
                    float randDialog = Random.value;
                    if (randDialog > .7f)
                        message = "Ya better have good loot.";
                    else if (.7f > randDialog && randDialog > .3f)
                        message = "Wha've ya got?";
                    else
                        message = "Yer potions won' be as good as mine... but I may take them.";
                    yield return GameManager.instance.ShowTextPrompt(message, voice, lettersPerSec, "Health Potion (15)", "Mana Potion (10)", "Soul (?)", "Go Back");
                    // HEALTH
                    if (GameManager.instance.selection == 0)
                    {
                        if (GameManager.instance.player.TryRemoveItem(new Item("Health")))
                        {
                            GameManager.instance.PlaySound("Swish");
                            GameManager.instance.ReceiveGold(15);
                            GameManager.instance.textManager.UpdateStatsText("Gold: " + GameManager.instance.GetCurrentGold());
                        }
                        else
                        {
                            message = "Ya don' even have enough of those ta sell... What're ya tryin' ta pull?";
                            yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                        }
                    }
                    // MANA
                    else if (GameManager.instance.selection == 1)
                    {
                        if (GameManager.instance.player.TryRemoveItem(new Item("Mana")))
                        {
                            GameManager.instance.PlaySound("Swish");
                            GameManager.instance.ReceiveGold(10);
                            GameManager.instance.textManager.UpdateStatsText("Gold: " + GameManager.instance.GetCurrentGold());
                        }
                        else
                        {
                            message = "Ya don' even have enough of those ta sell... What're ya tryin' ta pull?";
                            yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                        }
                    }
                    // SOUL
                    else if (GameManager.instance.selection == 2)
                    {
                        message = "Woah...";
                        yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                        message = "Hey man, I don' have the gold ta buy somethin' like that.";
                        yield return GameManager.instance.ShowTextPrompt(message, voice, lettersPerSec, "Please.", "...");
                        // Please
                        if (GameManager.instance.selection == 0)
                        {
                            message = "Dude...";
                            yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                            message = "...";
                            yield return GameManager.instance.textManager.DisplayText(message, voice, 4, true);
                            message = "No.";
                            yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                        }
                        // ...
                        else
                        {
                            message = "........";
                            yield return GameManager.instance.textManager.DisplayText(message, voice, 5, true);
                            yield return new WaitForSeconds(1.75f);
                            message = "Well this is awkward..";
                            yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                            yield return new WaitForSeconds(.3f);
                        }
                    }
                    // GO BACK
                    else
                    {
                        dialogNumber = 5;
                    }
                }
            }
        }

        GameManager.instance.textManager.ToggleStatsBox();

        float rando = Random.value;
        if (rando > .7f)
            message = "Seeya 'round.";
        else if (.7f > rando && rando > .3f)
            message = "Thanks fer stoppin' by!";
        else
            message = "Pleasure doin' business.";
        yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);

        dialogNumber = 1;

        // Resume the player's movement
        GameManager.instance.ResumePlayerMovement();
        currentlyInteracting = false;
    }

    private IEnumerator GivePlayerAMove(int moveNum)
    {
        if (!move1Bought || !move2Bought || !move3Bought || !move4Bought)
        {
            float randDialog = Random.value;
            if (randDialog > .8f)
                message = "Ye're in more than capable hands, I promise. Jus' do EXACTLY what I do...";
            else if (.8f > randDialog && randDialog > .5f)
                message = "Alrigh' this may be a bit complex. Jus' do as I do...";
            else
                message = "This 'ere move is a good one. Jus' follow my lead...";
            yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
            yield return GameManager.instance.FadeOut();
            yield return new WaitForSeconds(.1f);
            yield return GameManager.instance.FadeIn();
            yield return new WaitForSeconds(.1f);

            bool somethingPurchased = false;
            while (!somethingPurchased)
            {
                if (moveNum == 1)
                {
                    move1Bought = true;
                    if (!GameManager.instance.player.HasMove(Moves.GetSpecificMove("PURE", 0)))
                    {
                        message = "See! Easy. I told ya ye could do it.";
                        yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                        message = "Now go an' test it out. An' hey... Don' abuse it's power.";
                        yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);

                        // Holy Light
                        GameManager.instance.player.AddMoveToMoveOptions(Moves.GetSpecificMove("PURE", 0));

                        yield return new WaitForSeconds(.3f);
                        message = "* You learned Holy Light.";
                        yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                        yield return new WaitForSeconds(.3f);

                        somethingPurchased = true;
                        yield break;
                    }
                    else
                        moveNum++;
                }

                if (moveNum == 2)
                {
                    move2Bought = true;
                    if (!GameManager.instance.player.HasMove(Moves.GetSpecificMove("WATER", 2)))
                    {
                        message = "There ya go, that weren' so bad.";
                        yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);

                        // Power Wave
                        GameManager.instance.player.AddMoveToMoveOptions(Moves.GetSpecificMove("WATER", 2));

                        yield return new WaitForSeconds(.3f);
                        message = "* You learned Power Wave.";
                        yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                        yield return new WaitForSeconds(.3f);

                        somethingPurchased = true;
                        yield break;
                    }
                    else
                        moveNum++;
                }

                if (moveNum == 3)
                {
                    move3Bought = true;
                    if (!GameManager.instance.player.HasMove(Moves.GetSpecificMove("UNDEAD", 0)))
                    {
                        message = "Nice job! Ye're a fast learner.";
                        yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                        message = "Wasn' really expectin' that...";
                        yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);

                        // Undying Mesmer
                        GameManager.instance.player.AddMoveToMoveOptions(Moves.GetSpecificMove("UNDEAD", 0));

                        yield return new WaitForSeconds(.3f);
                        message = "* You learned Undying Mesmer.";
                        yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                        yield return new WaitForSeconds(.3f);

                        somethingPurchased = true;
                        yield break;
                    }
                    else
                        moveNum++;
                }

                if (moveNum == 4)
                {
                    move4Bought = true;
                    if (!GameManager.instance.player.HasMove(Moves.GetSpecificMove("EARTH", 2)))
                    {
                        message = "An' finished. Good learnin' session.";
                        yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                        message = "Go enjoy yer new found skill.";
                        yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);

                        // Earth Tremble
                        GameManager.instance.player.AddMoveToMoveOptions(Moves.GetSpecificMove("EARTH", 2));

                        yield return new WaitForSeconds(.3f);
                        message = "* You learned Earth Tremble!";
                        yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                        yield return new WaitForSeconds(.3f);

                        somethingPurchased = true;
                        yield break;
                    }
                    else
                        moveNum = 1;
                }
            }
        }
        else
        {
            allMovesBought = true;
            GameManager.instance.ReceiveGold(movesGold);
            GameManager.instance.totalGold -= movesGold;
            message = "I never thought this day would come...";
            yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
            message = "I ran outta moves ta teach. I guess ya can have this...";
            yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);

            if (GameManager.instance.player.TryCollectItem(new Item("Strength")))
            {
                message = "It ain' much but I hope it helps.";
                yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                yield return new WaitForSeconds(.3f);
                message = "* You received a Strength Potion.";
                yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                yield return new WaitForSeconds(.3f);
            }
            else
            {
                message = "* You don't have enough room for a Strength Potion.";
                yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                message = "Oh boy... I guess there really is nothin' I can do fer ya.";
                yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                message = "I'll just give ya this here rock I guess...";
                yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                yield return new WaitForSeconds(.3f);
                message = "* You took the rock.";
                yield return GameManager.instance.textManager.DisplayText(message, voice, lettersPerSec, true);
                yield return new WaitForSeconds(.3f);
            }
        }
    }
}