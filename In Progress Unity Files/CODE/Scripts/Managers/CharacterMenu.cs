using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMenu : MonoBehaviour
{
    // Main menu
    public Text hpText, mpText, levelText, goldText, strText, tufText, xpText;
    public Sprite enabledSprite;
    public Sprite disabledSprite;
    private Player player;
    public GameObject allMovesButton;
    public RectTransform xpBarProgress;
    public List<Text> itemsCounts;
    public List<Text> moveTexts;
    // Logic
    private bool hasMoreThanFourMoves;
    private const float MAX_XP = 695;
    private const float MIN_XP = 40;
    //private const float ANIMATE_SPEED = 0.01f;

    // All moves popup
    public GameObject allMoves;
    public GameObject displaysContainer;
    public Text nameText, descriptionText, mp_Text, damageText;
    // Logic
    private bool isMovesMenuOpen;
    private bool isSelectingPosition;
    private int selectedMove;
    private int movePosition;
    public List<GameObject> movePositionHighlights;

    private void Update()
    {
        // Close the main menu if X is pressed
        if (!isMovesMenuOpen && Input.GetKeyDown(KeyCode.X)) { GameManager.instance.CharacterMenuToggle(); return; }

        // Check if player currently has >4 moves
        if (hasMoreThanFourMoves)
        {
            // Check if the all moves popup menu is not open
            if (!isMovesMenuOpen)
            {
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    GameManager.instance.PlaySound("ButtonSelect");
                    // Open up the move selection screen
                    allMoves.SetActive(true);
                    isMovesMenuOpen = true;
                    selectedMove = 0;
                    UpdateDisplayedMove();
                    return;
                }
            }
            // All moves popup menu is open
            else
            {
                // Check if just scrolling through the menu
                if (!isSelectingPosition)
                {
                    // Close the move selection screen
                    if (Input.GetKeyDown(KeyCode.X))
                    {
                        GameManager.instance.PlaySound("ButtonCancel");
                        allMoves.SetActive(false);
                        isMovesMenuOpen = false;
                        return;
                    }
                    // Close display and start selecting a spot
                    if (Input.GetKeyDown(KeyCode.Z))
                    {
                        GameManager.instance.PlaySound("ButtonSelect");
                        displaysContainer.SetActive(false);
                        movePosition = 0;
                        isSelectingPosition = true;
                        return;
                    }

                    // Scroll through moves
                    if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        GameManager.instance.PlaySound("ButtonNavigate");
                        if (selectedMove == 0)
                            selectedMove = GameManager.instance.player.allMoves.Count - 1;
                        else
                            selectedMove--;
                    }
                    if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        GameManager.instance.PlaySound("ButtonNavigate");
                        if (selectedMove == GameManager.instance.player.allMoves.Count - 1)
                            selectedMove = 0;
                        else
                            selectedMove++;
                    }
                    UpdateDisplayedMove();
                }
                // Menu is open and being viewed
                else
                {
                    // Cancel position selection
                    if (Input.GetKeyDown(KeyCode.X))
                    {
                        GameManager.instance.PlaySound("ButtonCancel");
                        displaysContainer.SetActive(true);
                        isSelectingPosition = false;
                        return;
                    }
                    // Confirm position selection
                    if (Input.GetKeyDown(KeyCode.Z))
                    {
                        GameManager.instance.PlaySound("ButtonSelect");
                        // Put move in active move set
                        GameManager.instance.player.SetMoveInMoveSet(GameManager.instance.player.allMoves[selectedMove], movePosition);
                        // Update main menu display
                        UpdateMenu();
                        // Turn off all highlights
                        movePosition = -1;
                        UpdateDisplayedSelection();
                        displaysContainer.SetActive(true);
                        isSelectingPosition = false;
                        return;
                    }

                    // Scroll through position options
                    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        GameManager.instance.PlaySound("ButtonNavigate");
                        if (movePosition == 0)
                            movePosition = 3;
                        else
                            movePosition--;
                    }
                    if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        GameManager.instance.PlaySound("ButtonNavigate");
                        if (movePosition == 3)
                            movePosition = 0;
                        else
                            movePosition++;
                    }
                    UpdateDisplayedSelection();
                }
            }
        }
    }

    public void UpdateMenu()
    {
        player = GameManager.instance.player;

        // Check if player now has 4 moves, allow viewing 'all moves'
        if (!hasMoreThanFourMoves && player.allMoves.Count > 4)
        { hasMoreThanFourMoves = true; allMovesButton.SetActive(true); }

        // Move set
        switch (player.moveSet.Length)
        {
            case 0:
                break;
            case 1:
                UpdateMoveText(0, true);
                UpdateMoveText(1, false);
                UpdateMoveText(2, false);
                UpdateMoveText(3, false);
                break;
            case 2:
                UpdateMoveText(0, true);
                UpdateMoveText(1, true);
                UpdateMoveText(2, false);
                UpdateMoveText(3, false);
                break;
            case 3:
                UpdateMoveText(0, true);
                UpdateMoveText(1, true);
                UpdateMoveText(2, true);
                UpdateMoveText(3, false);
                break;
            case 4:
                UpdateMoveText(0, true);
                UpdateMoveText(1, true);
                UpdateMoveText(2, true);
                UpdateMoveText(3, true);
                break;
        }

        // Player Info
        hpText.text = GameManager.instance.health + " / " + GameManager.instance.maxHealth;
        mpText.text = GameManager.instance.MP + " / " + GameManager.instance.maxMana;
        levelText.text = GameManager.instance.level.ToString();
        goldText.text = GameManager.instance.GetCurrentGold().ToString();
        strText.text = player.strength.ToString();
        tufText.text = player.toughness.ToString();

        // XP Bar
        xpText.text = "XP: " + GameManager.instance.GetCurrentXP() + " / " + GameManager.instance.levelUpXPNeeded;
        float percentage = (float)GameManager.instance.GetCurrentXP() / GameManager.instance.levelUpXPNeeded;
        UpdateXPProgressSize(new Vector2(percentage * MAX_XP + MIN_XP, 56f));

        // Inventory
        UpdateItemCounts();
    }

    private void UpdateDisplayedMove()
    {
        nameText.text = player.allMoves[selectedMove].moveName;
        descriptionText.text = player.allMoves[selectedMove].moveDescription;
        mp_Text.text = player.allMoves[selectedMove].MPCost.ToString();
        damageText.text = player.allMoves[selectedMove].damage.ToString();
    }

    private void UpdateDisplayedSelection()
    {
        // Highlight the slot
        for (int i = 0; i < 4; i++)
        {
            if (i == movePosition)
                movePositionHighlights[i].SetActive(true);
            else
                movePositionHighlights[i].SetActive(false);
        }
    }

    private void UpdateItemCounts()
    {
        itemsCounts[0].text = "x" + player.GetItemCount(new Item("Health"));
        itemsCounts[1].text = "x" + player.GetItemCount(new Item("Mana"));
        itemsCounts[2].text = "x" + player.GetItemCount(new Item("Strength"));
        itemsCounts[3].text = "x" + player.GetItemCount(new Item("Toughness"));
    }

    private void UpdateXPProgressSize(Vector2 newSize)
    {
        Vector2 oldSize = xpBarProgress.rect.size;
        Vector2 deltaSize = newSize - oldSize;
        xpBarProgress.offsetMin = xpBarProgress.offsetMin - new Vector2(deltaSize.x * xpBarProgress.pivot.x, deltaSize.y * xpBarProgress.pivot.y);
        xpBarProgress.offsetMax = xpBarProgress.offsetMax + new Vector2(deltaSize.x * (1f - xpBarProgress.pivot.x), deltaSize.y * (1f - xpBarProgress.pivot.y));
    }

    private void UpdateMoveText(int moveNumber, bool isSet)
    {
        if (isSet)
        {
            Move move = player.moveSet[moveNumber];
            moveTexts[moveNumber].text = move.moveName + ": " + move.moveDescription + "\n" + move.type +
                " | " + move.damage + " DMG | " + move.MPCost + " MP";
            moveTexts[moveNumber].gameObject.GetComponentInParent<Image>(true).sprite = enabledSprite;
        }
        else
        {
            moveTexts[moveNumber].text = "";
            moveTexts[moveNumber].gameObject.GetComponentInParent<Image>(true).sprite = disabledSprite;
        }
    }

    // Logic to use items outside of a battle
    //public void TryUseItem(int itemIndex)
    //{
    //    StartCoroutine(TryUsingItem(itemIndex));
    //}

    //private IEnumerator TryUsingItem(int itemIndex)
    //{
    //    Item item = new Item();
    //    switch (itemIndex)
    //    {
    //        case 0:
    //            // Get the item
    //            item = new Item("Health");
    //            // Get current HP
    //            float currentHP = (float)GameManager.instance.health;

    //            // Try to consume the item
    //            if (player.TryConsumeItem(item))
    //            {
    //                UpdateItemCounts();
    //                // Play the using potion sound, give text feedback, and update the HP
    //                GameManager.instance.PlaySound("UsePotion");
    //                GameManager.instance.ShowTextInfo("You drank a Health Potion. Your wounds faded and you feel much better!");
    //                yield return UpdateHP(currentHP);
    //            }
    //            break;
    //        case 1:
    //            // Get the item
    //            item = new Item("Mana");
    //            // Get current MP
    //            float currentMP = (float)GameManager.instance.MP;

    //            // Try to consume the item
    //            if (player.TryConsumeItem(item))
    //            {
    //                UpdateItemCounts();
    //                // Play the using potion sound, give text feedback, and update the MP
    //                GameManager.instance.PlaySound("UsePotion");
    //                GameManager.instance.ShowTextInfo("You drank a Mana Potion. Your mana pool feels restored!");
    //                yield return UpdateMP(currentMP);
    //            }
    //            break;
    //        case 2:
    //            // Get the item
    //            item = new Item("Strength");
    //            // Try to consume the item
    //            if (player.TryConsumeItem(item))
    //            {
    //                UpdateItemCounts();
    //                // Play the using potion sound and give text feedback
    //                GameManager.instance.PlaySound("UsePotion");
    //                GameManager.instance.ShowTextInfo("You drank a Strength Potion. You feel extra strength flowing through you!");
    //            }
    //            break;
    //        case 3:
    //            // Get the item
    //            item = new Item("Toughness");
    //            // Try to consume the item
    //            if (player.TryConsumeItem(item))
    //            {
    //                UpdateItemCounts();
    //                // Play the using potion sound and give text feedback
    //                GameManager.instance.PlaySound("UsePotion");
    //                GameManager.instance.ShowTextInfo("You drank a Toughness Potion. You feel MUCH tougher!");
    //            }
    //            break;
    //    }
    //}

    //private IEnumerator UpdateHP(float currentHP)
    //{
    //    float newHP = (float)GameManager.instance.health;
    //    float changeAmount = newHP - currentHP;

    //    while (newHP - currentHP > 0.5f)
    //    {
    //        currentHP += changeAmount * statAnimateSpeed;
    //        // Set text value
    //        hpText.text = Mathf.FloorToInt(currentHP) + " / " + GameManager.instance.maxHealth;
    //        yield return null;
    //    }

    //    // Set final text value
    //    hpText.text = GameManager.instance.health + " / " + GameManager.instance.maxHealth;
    //}

    //private IEnumerator UpdateMP(float currentMP)
    //{
    //    float newMP = (float)GameManager.instance.MP;
    //    float changeAmount = newMP - currentMP;

    //    while (newMP - currentMP > 0.5f)
    //    {
    //        currentMP += changeAmount * statAnimateSpeed;
    //        // Set text value
    //        mpText.text = Mathf.FloorToInt(currentMP) + " / " + GameManager.instance.maxMana;
    //        yield return null;
    //    }

    //    // Set final text value
    //    mpText.text = GameManager.instance.MP + " / " + GameManager.instance.maxMana;
    //}
}
