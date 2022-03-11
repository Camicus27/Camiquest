using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameLaunchMenu : MonoBehaviour
{
    public OpeningCutScene openCutScene;

    public Image playerModel;
    public PlayerUnit animatorScript;

    public GameObject dialogBox;
    public InputField warriorName;
    public GameObject confirmationBox;
    public Text isThisCorrect;
    public Image[] optionHighlights = new Image[2];

    private bool isConfirming = false;
    private bool selectionChosen = false;

    void Start()
    {
        warriorName.ActivateInputField();

        warriorName.contentType = InputField.ContentType.Name;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2) || Input.GetMouseButton(3) || Input.GetMouseButton(4))
            warriorName.Select();

        if (Input.GetKeyDown(KeyCode.Return) && !isConfirming)
        {
            if (warriorName.text.Length == 0)
            {
                warriorName.ActivateInputField();
                return;
            }

            dialogBox.SetActive(false);

            if (warriorName.text == "Camicus")
                isThisCorrect.text = "This name is obviously correct.";
            else
                isThisCorrect.text = "Is this name correct?";

            confirmationBox.SetActive(true);
            GameManager.instance.PlaySound("ButtonNavigate");

            StartCoroutine(WaitForConfirmSelection());
        }
        
        if (isConfirming)
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
                optionHighlights[0].color = new Color(1, 1, 1, 0);
                optionHighlights[1].color = new Color(1, 1, 1, 0);
                selectionChosen = true;
            }
        }
    }

    private IEnumerator WaitForConfirmSelection()
    {
        animatorScript.BattleAction("Confirming");

        // Delay a bit
        yield return null;

        // Show selection option highlight
        GameManager.instance.selection = 0;
        isConfirming = true;
        optionHighlights[0].color = new Color(1, 1, 1, 1);
        selectionChosen = false;

        // Wait for a selection to be chosen
        yield return new WaitUntil(SelectionChosen);
        // GameManger.selection now holds the user's choice

        // YES
        if (GameManager.instance.selection == 0)
        {
            GameManager.instance.PlaySound("ButtonSelect");
            animatorScript.BattleAction("Confirmed");
            GameManager.instance.playerName = warriorName.text;
            // Start the game
            yield return GameManager.instance.FadeOut();
            yield return new WaitForSeconds(1f);
            openCutScene.StartScene();
            gameObject.SetActive(false);
        }
        // NO
        else if (GameManager.instance.selection == 1)
        {
            GameManager.instance.PlaySound("ButtonCancel");
            confirmationBox.SetActive(false);
            dialogBox.SetActive(true);
            warriorName.SetTextWithoutNotify("");
            warriorName.ActivateInputField();
            isConfirming = false;
            animatorScript.BattleAction("Denied");
        }
    }
    private bool SelectionChosen() { return selectionChosen; }

    
}
