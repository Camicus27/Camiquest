using System.Collections;
using UnityEngine;

public class LootChest : Interactable
{
    public Sprite collectedSprite;

    protected override void OnInteraction()
    {
        // Stop the player's movement
        GameManager.instance.StopPlayerMovement();

        // Collect the item
        isInteractable = false;
        isInRange = false;
        currentlyInteracting = false;
        GameManager.instance.PlaySound("ChestCollected");
        gameObject.GetComponent<SpriteRenderer>().sprite = collectedSprite;

        // Disable the collider
        GetComponent<BoxCollider2D>().enabled = false;

        StartCoroutine(Interaction());
    }

    private IEnumerator Interaction()
    {
        // 60% chance to collect gold
        if (Random.value < 0.60f)
        {
            int gold = Random.Range(15, 25) + 5 * GameManager.instance.level;
            GameManager.instance.ReceiveGold(gold);
            yield return GameManager.instance.ShowTextInfo("Collected " + gold + " Gold!");
        }
        // 40% chance to collect a random potion
        else
        {
            switch (Random.Range(0, 4))
            {
                case 0:
                    if (GameManager.instance.player.TryCollectItem(new Item("Health")))
                        yield return GameManager.instance.ShowTextInfo("Found a Health Potion!");
                    else
                        yield return GameManager.instance.ShowTextInfo("Found a Health Potion... but you are carrying too many!");
                    break;
                case 1:
                    if (GameManager.instance.player.TryCollectItem(new Item("Mana")))
                        yield return GameManager.instance.ShowTextInfo("Found a Mana Potion!");
                    else
                        yield return GameManager.instance.ShowTextInfo("Found a Mana Potion... but you are carrying too many!");
                    break;
                case 2:
                    if (GameManager.instance.player.TryCollectItem(new Item("Strength")))
                        yield return GameManager.instance.ShowTextInfo("Found a Strength Potion!");
                    else
                        yield return GameManager.instance.ShowTextInfo("Found a Strength Potion... but you are carrying too many!");
                    break;
                case 3:
                    if (GameManager.instance.player.TryCollectItem(new Item("Toughness")))
                        yield return GameManager.instance.ShowTextInfo("Found a Toughness Potion!");
                    else
                        yield return GameManager.instance.ShowTextInfo("Found a Toughness Potion... but you are carrying too many!");
                    break;
            }
        }

        // Resume the player's movement
        GameManager.instance.ResumePlayerMovement();
    }
}
