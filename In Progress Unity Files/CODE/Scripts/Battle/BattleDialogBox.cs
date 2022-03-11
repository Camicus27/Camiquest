using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    // References to TextBoxes and Images
    public Text dialogText;
    public GameObject moveSelector;
    public Text movesInfoText;
    public Text damageInfoText;
    public GameObject divider;
    public GameObject information;
    public Text description;
    public Text stats;
    public GameObject inventory;
    public Text itemInfo;
    public Text itemStats;
    public Text itemCountText;
    public List<GameObject> invSlots;
    public List<Image> actionOptions;
    public List<Image> moveOptions;

    // Text logic
    private int defaultLettersPerSecond = 25;
    //[HideInInspector]
    //public int pageNumber;


    /// <summary>
    /// Type the dialog one letter at a time
    /// </summary>
    public IEnumerator TypeDialog(string message)
    { yield return GameManager.instance.TypeTextAutoContinue(message, dialogText, "Typing_", defaultLettersPerSecond, 1f); }
    public IEnumerator TypeDialog(string message, float hangTimeAfter)
    {
        yield return GameManager.instance.TypeTextAutoContinue(message, dialogText, "Typing_", defaultLettersPerSecond, hangTimeAfter);
    }


    /// <summary>
    /// Toggle the different text boxes and images
    /// </summary>
    public void ToggleDialogText(bool enabled) { dialogText.enabled = enabled; }
    public void ToggleDivider(bool enabled) { divider.SetActive(enabled); }
    public void ToggleMoveSelector(bool enabled) { moveSelector.SetActive(enabled); }
    public void ToggleInventorySelector(bool enabled) { inventory.SetActive(enabled); }
    public void ToggleInformation(bool enabled) { information.SetActive(enabled); }
    public void ToggleActionSelector(bool visible, bool enabled)
    {
        if (visible)
        {
            if (enabled)
            {
                foreach (Image action in actionOptions)
                {
                    action.gameObject.SetActive(true);
                    action.color = new Color(1, 1, 1, 1);
                }
            }
            else
            {
                foreach (Image action in actionOptions)
                {
                    action.gameObject.SetActive(true);
                    action.color = new Color(1, 1, 1, .6f);
                }
            }
        }
        else
        {
            foreach (Image butt in actionOptions)
                butt.gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// Update selection
    /// </summary>
    /// <param name="selectedAction"></param>
    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i == selectedAction)
                actionOptions[i].gameObject.GetComponent<Animator>().SetBool("isHighlighted", true);
            else
                actionOptions[i].gameObject.GetComponent<Animator>().SetBool("isHighlighted", false);
        }
    }
    public void UpdateMoveSelection(int selectedAction, Move move)
    {
        if (move == null)
            return;

        for (int i = 0; i < 4; i++)
        {
            if (i == selectedAction)
                moveOptions[i].gameObject.GetComponent<Animator>().SetBool("isHighlighted", true);
                //moveButtons[i].targetGraphic.CrossFadeColor(moveButtons[i].colors.highlightedColor, false ? 0f : moveButtons[i].colors.fadeDuration, true, true);
            else
                moveOptions[i].gameObject.GetComponent<Animator>().SetBool("isHighlighted", false);
        }

        movesInfoText.text = move.moveDescription;
        if (move.type == "KINETIC")
            damageInfoText.text = move.damage + " " + move.type;
        else
            damageInfoText.text = move.damage + " " + move.type + " | COST: " + move.MPCost + "MP";
    }
    public void UpdateEnemyInformation(Enemy enemy)
    {
        int levelDiff = GameManager.instance.level - enemy.level;

        if (levelDiff > 1)
        {
            stats.text = enemy.trait + "\n" + enemy.type + "\nSTRENGTH " + enemy.strength + "\nTOUGHNESS " + enemy.toughness;
        }
        else if (levelDiff > 0)
        {
            stats.text = enemy.trait + "\n" + enemy.type + "\nSTRENGTH ??\nTOUGHNESS ??";
        }
        else
        {
            stats.text = "???\n????\nSTRENGTH ??\nTOUGHNESS ??";
        }

        description.text = enemy.enemyInfo;
    }
    public void UpdateItemSelection(int selectedItem, Dictionary<Item, int> playerItems)
    {
        Item item = new Item();
        switch (selectedItem)
        {
            case 0:
                item = new Item("Health");
                itemStats.text = "RESTORES " + item.effectivenessPoints + " HEALTH"; break;
            case 1:
                item = new Item("Mana");
                itemStats.text = "RESTORES " + item.effectivenessPoints + " MANA"; break;
            case 2:
                item = new Item("Strength");
                itemStats.text = "+" + item.effectivenessPoints + " STRENGTH"; break;
            case 3:
                item = new Item("Toughness");
                itemStats.text = "+" + item.effectivenessPoints + " TOUGHNESS"; break;
        }
        // Update the info about the item
        itemInfo.text = item.itemInfo;
        itemCountText.text = "x" + playerItems[item];

        UpdateAllButtonGraphics(selectedItem, playerItems[item] == 0);
    }
    private void UpdateAllButtonGraphics(int selectedItem, bool isOut)
    {
        // Highlight currently selected item
        for (int i = 0; i < 4; i++)
        {
            if (i == selectedItem)
            {
                if (isOut)
                    invSlots[i].GetComponent<Animator>().SetBool("isOut", true);
                invSlots[i].GetComponent<Animator>().SetBool("isHighlighted", true);
            }
            else
                invSlots[i].GetComponent<Animator>().SetBool("isHighlighted", false);
        }
    }


    /// <summary>
    /// Set the dialog box with the player's moves/inventory
    /// </summary>
    public void SetMoveNames(Move[] moves)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i < moves.Length)
            {
                moveOptions[i].gameObject.GetComponentInChildren<Text>().text = moves[i].moveName;
                moveOptions[i].enabled = true;
            }
            else
            {
                moveOptions[i].gameObject.GetComponentInChildren<Text>().text = "-";
                moveOptions[i].enabled = false;
            }
        }
    }
    public void SetItems(Dictionary<Item, int> playerItems)
    {
        invSlots[0].GetComponentInChildren<Text>().text = "Health Potion";
        invSlots[1].GetComponentInChildren<Text>().text = "Mana Potion";
        invSlots[2].GetComponentInChildren<Text>().text = "Strength Potion";
        invSlots[3].GetComponentInChildren<Text>().text = "Toughness Potion";
    }


    /* This is an implemenation if pages of items were used */
    //public void SetItems(List<Item> playerItems, int page)
    //{
    //    pageNumber = page;
    //    // page 0, 1, or 2
    //    if (pageNumber == 0)
    //    {
    //        for (int i = 0; i < 4; i++)
    //        {
    //            // Update each slot
    //            invSlots[i].GetComponentInChildren<Text>().text = playerItems[i].itemName;
    //        }
    //    }
    //    else if (pageNumber == 1)
    //    {
    //        for (int i = 4; i < 8; i++)
    //        {
    //            // Update each slot
    //            invSlots[i%4].GetComponentInChildren<Text>().text = playerItems[i].itemName;
    //        }
    //    }
    //    else
    //    {
    //        for (int i = 8; i < 12; i++)
    //        {
    //            // Update each slot
    //            invSlots[i%4].GetComponentInChildren<Text>().text = playerItems[i].itemName;
    //        }
    //    }
    //}

}
