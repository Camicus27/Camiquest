using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    // References for dialog/info/prompt text boxes
    public Image dialogBox;
    public Image dialogImage;
    public Text dialogText;
    public Image infoBox;
    public Text infoText;
    public Image statsBox;
    public Text statsText;
    public Image promptBox;
    public Text promptText;
    public Text opt1Text;
    public Text opt2Text;
    public Text opt3Text;
    public Text opt4Text;
    private Image[] optionHighlights = new Image[4];
    public Text continueKey;

    // Save popup
    public Image savePopupBox;
    public GameObject saveUI;
    public Text fileSavedText;
    public Text nameText;
    public Text hpText;
    public Text lvlText;
    public Text mpText;
    public GameObject saveCancelButtons;
    public Image saveOption;
    public Image cancelOption;

    // Logic
    private int lettersPerSecond = 25;
    private static bool stopTyping;
    private static bool allowStopTyping;
    private static bool cancel;
    // To be used with CancelOnBool. Caller controls this bool to cancel when desired
    public static bool Continue;

    private bool selectingOptions;
    private bool selectionChosen;

    private float hangTime;
    private float timeCheckpoint;

    public int saveOptionPosition;
    private bool isSaveSelecting;
    private bool saveSelectionChosen;

    


    /// <summary>
    /// Stops any and all dialog typing right now
    /// </summary>
    public void StopAll()
    {
        cancel = true;

        // Disable all dialog box objects
        dialogBox.enabled = false; dialogImage.enabled = false; dialogText.enabled = false;

        // Disable all info box objects
        infoBox.enabled = false; infoText.enabled = false;

        // Disable all prompt box objects
        promptBox.enabled = false; promptText.enabled = false; opt1Text.enabled = false; opt2Text.enabled = false;
        promptText.text = ""; opt1Text.text = ""; opt2Text.text = "";
        for (int i = 0; i < 4; i++)
            optionHighlights[i].enabled = false;
    }

    /// <summary>
    /// Setup the four option highlights into an array for easy access
    /// </summary>
    private void Start()
    {
        optionHighlights[0] = opt1Text.GetComponentInParent<Image>();
        optionHighlights[1] = opt2Text.GetComponentInParent<Image>();
        optionHighlights[2] = opt3Text.GetComponentInParent<Image>();
        optionHighlights[3] = opt4Text.GetComponentInParent<Image>();

        for (int i = 0; i < 4; i++)
            optionHighlights[i].color = new Color(1, 1, 1, 0);
    }

    /// <summary>
    /// Called every frame to check for key inputs
    /// Update checks for for X/Z to cancel dialog
    /// It also checks for key inputs when a prompt is shown
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && allowStopTyping)
        {
            stopTyping = true;
        }

        // When selecting in the save menu, this controls the highlight of which
        // option is currently selected.
        if (isSaveSelecting)
        {
            // Select current option
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (saveOptionPosition == 1)
                    GameManager.instance.PlaySound("ButtonSelect");
                saveOption.enabled = false;
                cancelOption.enabled = false;
                saveSelectionChosen = true;
                isSaveSelecting = false;
                return;
            }

            // Scroll through options
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) ||
            Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                GameManager.instance.PlaySound("ButtonNavigate");

                if (saveOptionPosition == 0)
                    saveOptionPosition++;
                else
                    saveOptionPosition--;
            }

            // Highlight the correct option
            if (saveOptionPosition == 0)
            {
                saveOption.enabled = true;
                cancelOption.enabled = false;
            }
            else
            {
                saveOption.enabled = false;
                cancelOption.enabled = true;
            }
        }

        // When selecting from some options, this controls the highlight of which
        // option is currently selected for a prompt box.
        // The first one is for 2 options, the second is for 4 options
        if (selectingOptions && !opt4Text.enabled)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) ||
            Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                GameManager.instance.PlaySound("ButtonNavigate");

                if (GameManager.instance.selection == 0)
                {
                    GameManager.instance.selection++;
                    optionHighlights[1].color = new Color(1, 1, 1, 1);
                    optionHighlights[0].color = new Color(1, 1, 1, 0);
                }
                else
                {
                    GameManager.instance.selection--;
                    optionHighlights[0].color = new Color(1, 1, 1, 1);
                    optionHighlights[1].color = new Color(1, 1, 1, 0);
                }
            }

            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
            {
                GameManager.instance.PlaySound("ButtonSelect");
                optionHighlights[0].color = new Color(1, 1, 1, 0);
                optionHighlights[1].color = new Color(1, 1, 1, 0);
                selectionChosen = true;
                selectingOptions = false;
            }
        }
        else if (selectingOptions && opt4Text.enabled)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                GameManager.instance.PlaySound("ButtonNavigate");
                if (GameManager.instance.selection < 3)
                    GameManager.instance.selection++;
            }
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                GameManager.instance.PlaySound("ButtonNavigate");
                if (GameManager.instance.selection > 0)
                    GameManager.instance.selection--;
            }
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                GameManager.instance.PlaySound("ButtonNavigate");
                if (GameManager.instance.selection < 2)
                    GameManager.instance.selection += 2;
            }
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                GameManager.instance.PlaySound("ButtonNavigate");
                if (GameManager.instance.selection > 1)
                    GameManager.instance.selection -= 2;
            }

            // Highlight the slot
            for (int i = 0; i < 4; i++)
            {
                // Highlight the selection
                if (i == GameManager.instance.selection)
                    optionHighlights[i].color = new Color(1, 1, 1, 1);
                // Un-highlight all
                else
                    optionHighlights[i].color = new Color(1, 1, 1, 0);
            }

            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
            {
                GameManager.instance.PlaySound("ButtonSelect");
                // Un-highlight all
                for (int i = 0; i < 4; i++)
                    optionHighlights[i].color = new Color(1, 1, 1, 0);
                selectionChosen = true;
                selectingOptions = false;
            }
        }
    }

    /// <summary>
    /// Display an NPC dialog with their sprite
    /// </summary>
    public IEnumerator ShowDialog(string msg, Sprite npc, string voice)
    { 
        dialogImage.sprite = npc;
        dialogBox.enabled = true; dialogImage.enabled = true; dialogText.enabled = true;
        yield return TypeText(msg, dialogText, voice, lettersPerSecond, true);
        dialogBox.enabled = false; dialogImage.enabled = false; dialogText.enabled = false;
    }
    public IEnumerator ShowDialog(string msg, Sprite npc, string voice, int lettersPerSec)
    { 
        dialogImage.sprite = npc;
        dialogBox.enabled = true; dialogImage.enabled = true; dialogText.enabled = true;
        yield return TypeText(msg, dialogText, voice, lettersPerSec, true);
        dialogBox.enabled = false; dialogImage.enabled = false; dialogText.enabled = false;
    }
    public IEnumerator ShowDialog(string msg, Sprite npc, string voice, int lettersPerSec, bool isSkippable)
    { 
        dialogImage.sprite = npc;
        dialogBox.enabled = true; dialogImage.enabled = true; dialogText.enabled = true;
        yield return TypeText(msg, dialogText, voice, lettersPerSec, isSkippable);
        dialogBox.enabled = false; dialogImage.enabled = false; dialogText.enabled = false;
    }

    /// <summary>
    /// Display some text information
    /// </summary>
    public IEnumerator ShowInfo(string msg)
    { yield return DisplayText(msg, "Typing_", lettersPerSecond, true); }
    public IEnumerator ShowInfo(string msg, int lettersPerSec)
    { yield return DisplayText(msg, "Typing_", lettersPerSec, true); }
    public IEnumerator ShowInfo(string msg, int lettersPerSec, bool isSkippable)
    { yield return DisplayText(msg, "Typing_", lettersPerSec, isSkippable); }

    /// <summary>
    /// Helper method to display either the dialog text or information text
    /// </summary>
    public IEnumerator DisplayText(string msg, string voice, int lettersPerSec, bool isSkippable)
    {
        cancel = false;

        infoBox.enabled = true; infoText.enabled = true;
        yield return TypeText(msg, infoText, voice, lettersPerSec, isSkippable);
        infoBox.enabled = false; infoText.enabled = false;
    }
    public IEnumerator DisplayTextCancelOnBool(string msg, string voice, int lettersPerSec, bool isSkippable)
    {
        cancel = false;

        infoBox.enabled = true; infoText.enabled = true;
        yield return TypeTextCancelOnBool(msg, infoText, voice, lettersPerSec, isSkippable);
        infoBox.enabled = false; infoText.enabled = false;
    }


    // ----------------------------------------------------------------------------------------------------------------------------------------------


    /// <summary>
    /// Shows a prompt for the player and by the end of the prompt,
    /// GameManage.selection will hold the number of the selection: 0-3
    /// </summary>
    public IEnumerator ShowPrompt(string prompt, string voice, int lettersPerSec, string option1, string option2)
    {
        cancel = false;
        selectionChosen = false;

        // Enable all prompt box objects
        promptText.text = ""; opt1Text.text = ""; opt2Text.text = "";
        promptBox.enabled = true; promptText.enabled = true; opt1Text.enabled = true; opt2Text.enabled = true;
        for (int i = 0; i < 2; i++)
            optionHighlights[i].enabled = true;

        // Display the prompt
        yield return TypeTextAuto(prompt, promptText, voice, lettersPerSec, .25f);
        // Display the two options
        opt1Text.text = option1;
        opt2Text.text = option2;

        GameManager.instance.PlaySound("ButtonNavigate");

        // Show selection option highlight
        GameManager.instance.selection = 0;
        selectingOptions = true;
        optionHighlights[0].color = new Color(1, 1, 1, 1);

        // Wait for a selection to be chosen
        yield return new WaitUntil(SelectionChosen);
        // GameManger.selection now holds the user's choice

        // Disable all prompt box objects
        promptBox.enabled = false; promptText.enabled = false; opt1Text.enabled = false; opt2Text.enabled = false;
        promptText.text = ""; opt1Text.text = ""; opt2Text.text = "";
        for (int i = 0; i < 2; i++)
            optionHighlights[i].enabled = false;
    }
    public IEnumerator ShowPrompt(string prompt, string voice, int lettersPerSec, string option1, string option2, string option3, string option4)
    {
        cancel = false;
        selectionChosen = false;

        // Enable all prompt box objects
        promptText.text = ""; opt1Text.text = ""; opt2Text.text = ""; opt3Text.text = ""; opt4Text.text = "";
        promptBox.enabled = true; promptText.enabled = true; 
        opt1Text.enabled = true; opt2Text.enabled = true; opt3Text.enabled = true; opt4Text.enabled = true;
        for (int i = 0; i < 4; i++)
            optionHighlights[i].enabled = true;

        // Display the prompt
        yield return TypeTextAuto(prompt, promptText, voice, lettersPerSec, .25f);
        // Display the four options
        opt1Text.text = option1;
        opt2Text.text = option2;
        opt3Text.text = option3;
        opt4Text.text = option4;

        GameManager.instance.PlaySound("ButtonNavigate");

        // Show selection option highlight
        GameManager.instance.selection = 0;
        selectingOptions = true;
        optionHighlights[0].color = new Color(1, 1, 1, 1);

        // Wait for a selection to be chosen
        yield return new WaitUntil(SelectionChosen);
        // GameManger.selection now holds the user's choice

        // Disable all prompt box objects
        promptBox.enabled = false; promptText.enabled = false;
        opt1Text.enabled = false; opt2Text.enabled = false; opt3Text.enabled = false; opt4Text.enabled = false;
        promptText.text = ""; opt1Text.text = ""; opt2Text.text = ""; opt3Text.text = ""; opt4Text.text = "";
        for (int i = 0; i < 4; i++)
            optionHighlights[i].enabled = false;
    }
    private bool SelectionChosen() { return selectionChosen; }


    // ----------------------------------------------------------------------------------------------------------------------------------------------

    public void ToggleStatsBox()
    {
        if (statsBox.enabled)
        {
            statsBox.enabled = false;
            statsText.enabled = false;
        }
        else
        {
            statsBox.enabled = true;
            statsText.enabled = true;
        }
    }

    public void UpdateStatsText(string text)
    {
        statsText.text = text;
    }


    // ----------------------------------------------------------------------------------------------------------------------------------------------

    public IEnumerator ActivateSaveMenu()
    {
        UpdatePlayerInfoText();
        savePopupBox.enabled = true;
        saveCancelButtons.SetActive(true);
        saveUI.SetActive(true);
        isSaveSelecting = true;

        // Wait for player to make selection
        yield return new WaitUntil(SaveSelectionChosen);

        // saveOptionPosition now has the chosen selection to be handled by save state
        
    }
    private bool SaveSelectionChosen() { return saveSelectionChosen; }

    private void UpdatePlayerInfoText()
    {
        nameText.text = GameManager.instance.player.playerName;
        hpText.text = "Max HP: " + GameManager.instance.player.maxHealth.ToString();
        mpText.text = "Max MP: " + GameManager.instance.player.maxMana.ToString();
        lvlText.text = "Level: " + GameManager.instance.player.level.ToString();
    }

    public IEnumerator FileSaved()
    {
        saveCancelButtons.SetActive(false);
        fileSavedText.enabled = true;

        yield return new WaitForSeconds(1.25f);

        DeactivateSaveMenu();
    }

    public void DeactivateSaveMenu()
    {
        savePopupBox.enabled = false;
        fileSavedText.enabled = false;
        saveUI.SetActive(false);
        isSaveSelecting = false;
        saveSelectionChosen = false;
    }

    /*
    public Image savePopupBox;
    public Text firstInteractionTextBox;
    public GameObject saveUI;
    public Text fileSavedText;
    public GameObject saveCancelButtons;
    nameText;
    public Text hpText;
    public Text lvlText;
    public Text mpText;
     */

    // ---------------------------------------------------------------------------------------------------------------------------------------------


    /// <summary>
    /// Animates the text to draw one letter at a time.
    /// </summary>
    /// <param name="text">What message to display</param>
    /// <param name="textBox">The text box object to type in</param>
    /// <param name="lettersPerSec">How fast to type</param>
    /// <param name="isSkippable">Can the player cancel the text mid typing?</param>
    /// <returns></returns>
    public IEnumerator TypeText(string text, Text textBox, string voice, int lettersPerSec, bool isSkippable)
    {
        // Setup
        cancel = false;
        allowStopTyping = isSkippable;
        stopTyping = true;
        textBox.text = "";
        yield return new WaitForSeconds(0.001f);
        stopTyping = false;

        // Get all letters and iterate
        foreach (char letter in text.ToCharArray())
        {
            if (cancel)
            {
                textBox.text = "";
                yield break;
            }
            if (stopTyping)
            {
                // Type rest of sentence
                textBox.text = text;
                GameManager.instance.PlaySound(voice);
                break;
            }
            else
            {
                // Type next letter
                textBox.text += letter;
                if (letter != ' ')
                    GameManager.instance.PlaySound(voice);
                yield return new WaitForSeconds(1f / lettersPerSec);
            }
        }

        PressXToContinue_On();
        yield return new WaitForSeconds(.25f);
        yield return new WaitUntil(PressToContinue);
        PressXToContinue_Off();
    }
    /// <summary>
    /// Helper that waits for user to press continue after typing
    /// </summary>
    private bool PressToContinue()
    {
        return (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Z)) ;
    }
    public IEnumerator TypeTextAuto(string text, Text textBox, string voice, int lettersPerSec, float hangTimeAfter)
    {
        // Setup
        cancel = false;
        allowStopTyping = true;
        stopTyping = true;
        textBox.text = "";
        yield return new WaitForSeconds(0.001f);
        stopTyping = false;

        // Get all letters and iterate
        foreach (char letter in text.ToCharArray())
        {
            if (cancel)
            {
                textBox.text = "";
                yield break;
            }
            if (stopTyping)
            {
                // Type rest of sentence
                textBox.text = text;
                GameManager.instance.PlaySound(voice);
                break;
            }
            else
            {
                // Type next letter
                textBox.text += letter;
                if (letter != ' ')
                    GameManager.instance.PlaySound(voice);
                yield return new WaitForSeconds(1f / lettersPerSec);
            }
        }

        hangTime = hangTimeAfter;
        timeCheckpoint = Time.time;
        yield return new WaitUntil(HangAfter);
    }
    private bool HangAfter()
    {
        return (Time.time - timeCheckpoint > hangTime) || PressToContinue();
    }

    public void PressXToContinue_On()
    {
        continueKey.enabled = true;
    }
    public void PressXToContinue_Off()
    {
        continueKey.enabled = false;
    }


    public IEnumerator TypeTextCancelOnBool(string text, Text textBox, string voice, int lettersPerSec, bool isSkippable)
    {
        // Setup
        cancel = false;
        allowStopTyping = isSkippable;
        stopTyping = true;
        textBox.text = "";
        yield return new WaitForSeconds(0.001f);
        stopTyping = false;

        // Get all letters and iterate
        foreach (char letter in text.ToCharArray())
        {
            if (cancel)
            {
                textBox.text = "";
                yield break;
            }
            if (stopTyping)
            {
                // Type rest of sentence
                textBox.text = text;
                GameManager.instance.PlaySound(voice);
                break;
            }
            else
            {
                // Type next letter
                textBox.text += letter;
                if (letter != ' ')
                    GameManager.instance.PlaySound(voice);
                yield return new WaitForSeconds(1f / lettersPerSec);
            }
        }

        yield return new WaitUntil(OnCancel);
    }
    private bool OnCancel() { return Continue; }
}
