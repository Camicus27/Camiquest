using System.Collections;
using UnityEngine;

// Keeps track of the state to know what actions to perform
public enum BattleState { Start, PlayerAction, PlayerMoves, PlayerItems, PlayerInfo, PlayerRun, EnemyMove, Busy, Finished, GameOver }

/// <summary>
/// A manager to control the flow of an enemy encounter.
/// Updates graphics, prompts changes to game state, and handles
/// some random chance calculations.
/// </summary>
public class BattleManager : MonoBehaviour
{
    BattleState state;

    // References
    public PlayerUnit playerUnit;
    private Player player;
    public EnemyUnit enemyUnit;
    private GameObject enemyObj;
    private Enemy enemy;
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;
    public BattleDialogBox dialogBox;

    // Selection states
    private int currentAction = 0;
    private bool selectingMoves = false;
    private int currentMove = 0;
    private bool choosingItem = false;
    private int currentSlot = 0;
    private bool showingInfo = false;
    private string battleResult = "win";


    // Entry Point ------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Begins the battle with some starting conditions
    /// </summary>
    /// <param name="enemy">The enemy to fight</param>
    public void StartBattle(GameObject enemy)
    {
        state = BattleState.Start;
        enemyObj = enemy;
        currentAction = 0;
        currentMove = 0;
        selectingMoves = false;
        StartCoroutine(SetupBattle());
    }

    /// <summary>
    /// Call at the start of the battle to setup sprites and information
    /// </summary>
    public IEnumerator SetupBattle()
    {
        // Set up the fighters
        playerUnit.Setup();
        enemyUnit.Setup(enemyObj);
        player = GameManager.instance.player;
        enemy = enemyObj.GetComponent<Enemy>();
        // Set up the HUDs
        playerHUD.Setup(null);
        enemyHUD.Setup(enemyObj);
        // Set up the dialog box
        dialogBox.ToggleActionSelector(true, false);
        dialogBox.SetMoveNames(player.moveSet);

        yield return dialogBox.TypeDialog(enemy.enemyName + " is attacking!!");

        EnableActionSelection();
    }

    // Update ---------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Update loop to check states and act accordingly, every frame
    /// </summary>
    private void Update()
    {
        if (state == BattleState.Busy)
        {
            // Just do nothing :)
        }
        else if (state == BattleState.Start)
        {
            state = BattleState.Busy;
        }
        else if (state == BattleState.PlayerAction)
        {
            GetActionSelection();
        }
        else if (state == BattleState.PlayerMoves)
        {
            GetPlayerMove();
        }
        else if (state == BattleState.PlayerItems)
        {
            ShowInventory();
        }
        else if (state == BattleState.PlayerInfo)
        {
            ShowInformation();
        }
        else if (state == BattleState.PlayerRun)
        {
            state = BattleState.Busy;
            StartCoroutine(AttemptToRun());
        }
        else if (state == BattleState.EnemyMove)
        {
            GetEnemyMove();
        }
        else if (state == BattleState.Finished)
        {
            state = BattleState.Busy;
            StartCoroutine(FinishBattle());
        }
    }

    // Main/Action Selection State --------------------------------------------------------------------------------------------------

    private void EnableActionSelection()
    {
        StopAllCoroutines();
        dialogBox.ToggleActionSelector(true, true);
        dialogBox.ToggleDialogText(true);
        StartCoroutine(dialogBox.TypeDialog("What will you do?"));
        state = BattleState.PlayerAction;
    }

    private void GetActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            GameManager.instance.PlaySound("ButtonNavigate");
            if (currentAction < 3)
                currentAction++;
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GameManager.instance.PlaySound("ButtonNavigate");
            if (currentAction > 0)
                currentAction--;
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            GameManager.instance.PlaySound("ButtonNavigate");
            if (currentAction < 2)
                currentAction += 2;
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            GameManager.instance.PlaySound("ButtonNavigate");
            if (currentAction > 1)
                currentAction -= 2;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            state = BattleState.Busy;
            GameManager.instance.PlaySound("ButtonSelect");

            if (currentAction == 0)
            {
                state = BattleState.PlayerMoves;
            }
            else if (currentAction == 1)
            {
                state = BattleState.PlayerItems;
            }
            else if (currentAction == 2)
            {
                state = BattleState.PlayerInfo;
            }
            else if (currentAction == 3)
            {
                state = BattleState.PlayerRun;
            }
        }
    }

    // Information State ---------------------------------------------------------------------------------------------------------

    private void ShowInformation()
    {
        // Basically if this is the first frame, do this, on all other frames skip this
        if (!showingInfo)
        {
            dialogBox.ToggleActionSelector(false, false);
            dialogBox.ToggleDialogText(false);

            dialogBox.ToggleInformation(true);
            dialogBox.UpdateEnemyInformation(enemy);

            showingInfo = true;
        }

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X))
        {
            state = BattleState.Busy;
            GameManager.instance.PlaySound("ButtonCancel");
            showingInfo = false;
            dialogBox.ToggleInformation(false);
            dialogBox.ToggleDialogText(true);
            EnableActionSelection();
        }
    }

    // Run Away State ---------------------------------------------------------------------------------------------------------

    private IEnumerator AttemptToRun()
    {
        state = BattleState.Busy;

        float runChance = 1f;

        // Calculate run chance
        if (enemy.level - GameManager.instance.level > 0)
        {
            runChance -= 0.1f * (enemy.level - GameManager.instance.level);
        }
        if ((double)GameManager.instance.health < GameManager.instance.health * 0.25)
        {
            runChance -= 0.2f;
        }
        if ((double)GameManager.instance.MP < GameManager.instance.MP * 0.1)
        {
            runChance += 0.15f;
        }

        // End battle with successful escape
        if (runChance > Random.value) 
        {
            battleResult = "playerRan";
            state = BattleState.Finished;
        }
        // End turn
        else
        {
            yield return dialogBox.TypeDialog("You were unable to escape!");
            state = BattleState.EnemyMove;
        }
    }

    // Inventory State ---------------------------------------------------------------------------------------------------------

    private void ShowInventory()
    {
        // Basically if this is the first frame, do this, on all other frames skip this
        if (!choosingItem)
        {
            dialogBox.UpdateItemSelection(currentSlot, player.items);
            dialogBox.ToggleActionSelector(false, false);
            dialogBox.ToggleDialogText(false);

            dialogBox.ToggleInventorySelector(true);
            dialogBox.SetItems(player.items);

            choosingItem = true;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            state = BattleState.Busy;
            GameManager.instance.PlaySound("ButtonCancel");
            choosingItem = false;
            dialogBox.ToggleInventorySelector(false);
            dialogBox.ToggleDialogText(true);
            EnableActionSelection();
            return;
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            GameManager.instance.PlaySound("ButtonNavigate");
            if (currentSlot < 3)
                currentSlot++;
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GameManager.instance.PlaySound("ButtonNavigate");
            if (currentSlot > 0)
                currentSlot--;
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            GameManager.instance.PlaySound("ButtonNavigate");
            if (currentSlot < 2)
                currentSlot += 2;
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            GameManager.instance.PlaySound("ButtonNavigate");
            if (currentSlot > 1)
                currentSlot -= 2;
        }

        dialogBox.UpdateItemSelection(currentSlot, player.items);

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
        {
            state = BattleState.Busy;
            GameManager.instance.PlaySound("ButtonSelect");

            // Get the item
            Item item = null;
            switch (currentSlot)
            {
                case 0:
                    item = new Item("Health"); break;
                case 1:
                    item = new Item("Mana"); break;
                case 2:
                    item = new Item("Strength"); break;
                case 3:
                    item = new Item("Toughness"); break;
            }

            // Continue the state
            float oldHP = GameManager.instance.health;
            float oldMP = GameManager.instance.MP;
            bool didConsume = player.TryConsumeItem(item);
            choosingItem = false;
            dialogBox.ToggleInventorySelector(false);
            dialogBox.ToggleDialogText(true);
            StopAllCoroutines();
            StartCoroutine(UsePlayerItem(item, didConsume, oldHP, oldMP));
        }

        /* This is an implemenation if pages of items were used */
        //else if (Input.GetKeyDown(KeyCode.Z) && currentSlot >= 4)
        //{
        //    if (currentSlot == 4)
        //    {
        //        if (dialogBox.pageNumber == 0)
        //            dialogBox.pageNumber = 2;
        //        else
        //            dialogBox.pageNumber--;
        //    }
        //    else
        //    {
        //        if (dialogBox.pageNumber == 2)
        //            dialogBox.pageNumber = 0;
        //        else
        //            dialogBox.pageNumber++;
        //    }
        //    dialogBox.SetItems(player.items);
        //    dialogBox.pageNumberText.text = (dialogBox.pageNumber + 1).ToString();
        //}
    }

    private IEnumerator UsePlayerItem(Item item, bool didConsume, float oldHP, float oldMP)
    {
        if (!didConsume)
        {
            yield return dialogBox.TypeDialog("You do not have any more of these!");
            state = BattleState.PlayerItems;
        }
        else
        {
            // Type the item used
            yield return dialogBox.TypeDialog(GameManager.instance.playerName + " used " + item.itemName + ".");
            GameManager.instance.PlaySound("UsePotion");
            playerUnit.BattleAction("Consume");
            yield return dialogBox.TypeDialog("...", 2f);

            // Type the item effect
            if (item.type == "HEALTH")
            {
                StartCoroutine(playerHUD.UpdateHP(oldHP));
                yield return dialogBox.TypeDialog("Restored " + item.GetEP() + " health!");
            }
            else if (item.type == "MANA")
            {
                StartCoroutine(playerHUD.UpdateMP(oldMP));
                yield return dialogBox.TypeDialog("Restored " + item.GetEP() + " mana!");
            }
            else if (item.type == "STRENGTH")
                yield return dialogBox.TypeDialog("You feel strengthened!");
            else if (item.type == "TOUGHNESS")
                yield return dialogBox.TypeDialog("You feel much tougher!");
            else
                yield return dialogBox.TypeDialog("It didn't really do anything... was it supposed to?");

            yield return new WaitUntil(playerUnit.IsDoneWithAnimation);

            // End turn
            state = BattleState.EnemyMove;
        }
    }

    // Player and Enemy Move States ----------------------------------------------------------------------------------------------------

    private void GetPlayerMove()
    {
        // Basically if this is the first frame, do this, on all other frames skip this
        if (!selectingMoves)
        {
            dialogBox.ToggleActionSelector(false, false);
            dialogBox.ToggleDialogText(false);

            dialogBox.ToggleMoveSelector(true);

            selectingMoves = true;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            state = BattleState.Busy;
            GameManager.instance.PlaySound("ButtonCancel");
            selectingMoves = false;
            dialogBox.ToggleMoveSelector(false);
            dialogBox.ToggleDialogText(true);
            EnableActionSelection();
            return;
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            GameManager.instance.PlaySound("ButtonNavigate");
            if (currentMove < 3 && player.moveSet[currentMove + 1] != null)
                currentMove++;
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GameManager.instance.PlaySound("ButtonNavigate");
            if (currentMove > 0 && player.moveSet[currentMove - 1] != null)
                currentMove--;
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            GameManager.instance.PlaySound("ButtonNavigate");
            if (currentMove < 2 && player.moveSet[currentMove + 2] != null)
                currentMove += 2;
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            GameManager.instance.PlaySound("ButtonNavigate");
            if (currentMove > 1 && player.moveSet[currentMove - 2] != null)
                currentMove -= 2;
        }

        dialogBox.UpdateMoveSelection(currentMove, player.moveSet[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
        {
            state = BattleState.Busy;
            GameManager.instance.PlaySound("ButtonSelect");

            // Continue the state
            bool canPerform = GameManager.instance.MP >= player.moveSet[currentMove].MPCost;
            selectingMoves = false;
            dialogBox.ToggleMoveSelector(false);
            dialogBox.ToggleDialogText(true);
            StopAllCoroutines();
            StartCoroutine(PerformPlayerMove(canPerform));
        }
    }

    private IEnumerator PerformPlayerMove(bool canPerform)
    {
        if (!canPerform)
        {
            yield return dialogBox.TypeDialog("You do not have enough mana!");
            state = BattleState.PlayerMoves;
        }
        else
        {
            // Get the move
            Move move = player.moveSet[currentMove];

            // Update the mana amount
            int currentMP = GameManager.instance.MP;
            GameManager.instance.MP -= player.moveSet[currentMove].MPCost;
            StartCoroutine(playerHUD.UpdateMP(currentMP));

            // Calculate the damage taken and to what fighter
            float oldHP;
            DamageDetails damageDetails = null;
            if (!move.isHealing)
            {
                // Type the move used and wait
                playerUnit.BattleAction("Attack_" + move.type);
                GameManager.instance.PlaySound("PlayerAttack");
                yield return dialogBox.TypeDialog(GameManager.instance.playerName + " used " + move.moveName + "!");
                yield return new WaitUntil(playerUnit.IsDoneWithAnimation);
                enemyUnit.BattleAction("Hit");
                GameManager.instance.PlaySound("EnemyHit");

                // Pass in the current HP to the HUD and take the damage
                oldHP = enemy.health;
                // Also get the details of the damage
                damageDetails = enemy.TakeDamage(move, player);
                yield return enemyHUD.UpdateHP(oldHP);
            }
            else
            {
                // Type the move used and wait
                playerUnit.BattleAction("Healing");
                GameManager.instance.PlaySound("PlayerHealing");
                yield return dialogBox.TypeDialog(GameManager.instance.playerName + " used " + move.moveName + "!");
                yield return new WaitUntil(playerUnit.IsDoneWithAnimation);
                // Pass in the current HP to the HUD and take the damage
                oldHP = GameManager.instance.health;
                // Also get the details of the damage
                damageDetails = player.TakeHealth(move);
                yield return playerHUD.UpdateHP(oldHP);
            }

            // Display the attack details
            yield return ShowDamageDetails(damageDetails);

            // Improve the skill of the move
            var moveDetails = move.ImproveSkill();
            // Check for move level up
            if (moveDetails.LeveledUp)
            {
                //GameManager.instance.PlaySound("MoveImproved");
                yield return dialogBox.TypeDialog(GameManager.instance.playerName + "\'s move " + move.moveName + " has improved!");
                yield return dialogBox.TypeDialog(move.moveName + " is now a " + moveDetails.Mastery + " Move!");
            }

            // Wait for enemy to finish being hit
            yield return new WaitWhile(enemyUnit.IsBeingHit);

            // Check if enemy has died
            if (damageDetails.Died)
            {
                battleResult = "win";
                state = BattleState.Finished;
            }
            // End turn
            else
                state = BattleState.EnemyMove;
        }
    }

    // Enemy Move State --------------------------------------

    private void GetEnemyMove()
    {
        // Get the enemy move
        Move move = enemy.GetMoveToUse();

        // Continue the state
        StopAllCoroutines();
        if (move.type == "RUNAWAY")
            StartCoroutine(PerformEnemyMove(move, true));
        else
            StartCoroutine(PerformEnemyMove(move, false));
    }

    private IEnumerator PerformEnemyMove(Move move, bool runAway)
    {
        state = BattleState.Busy;

        // Check if the enemy ran away
        if (runAway)
        {
            battleResult = "enemyRan";
            state = BattleState.Finished;
        }
        else
        {
            // Calculate the damage taken and to what fighter
            float oldHP;
            DamageDetails damageDetails = null;
            if (!move.isHealing)
            {
                // Type the move used and wait
                enemyUnit.BattleAction("Attack");
                GameManager.instance.PlaySound("EnemyAttack");
                yield return dialogBox.TypeDialog(enemy.enemyName + " used " + move.moveName + "!");
                yield return new WaitUntil(enemyUnit.IsDoneWithAnimation);
                playerUnit.BattleAction("Hit");
                GameManager.instance.PlaySound("PlayerHit");

                // Pass in the current HP to the HUD and take the damage
                oldHP = GameManager.instance.health;
                // Also get the details of the damage
                damageDetails = player.TakeDamage(move, enemy);
                yield return playerHUD.UpdateHP(oldHP);
            }
            else
            {
                // Type the move used and wait
                enemyUnit.BattleAction("Heal");
                GameManager.instance.PlaySound("EnemyHealing");
                yield return dialogBox.TypeDialog(enemy.enemyName + " used " + move.moveName + "!");
                yield return new WaitUntil(enemyUnit.IsDoneWithAnimation);

                // Pass in the current HP to the HUD and take the damage
                oldHP = enemy.health;
                // Also get the details of the damage
                damageDetails = enemy.TakeHealth(move);
                yield return enemyHUD.UpdateHP(oldHP);
            }

            // Display the attack details
            yield return ShowDamageDetails(damageDetails);

            // Check if player has died
            if (damageDetails.Died)
            {
                battleResult = "lose";
                state = BattleState.Finished;
            }
            // End turn
            else
                EnableActionSelection();
        }
    }

    // Helpers and End State ---------------------------------------------------------------------------------------------------------

    private IEnumerator ShowDamageDetails(DamageDetails dmgDets)
    {
        if (dmgDets.Critical > 1f)
            yield return dialogBox.TypeDialog("It was a critical hit!");

        if (dmgDets.TypeEffectiveness > 1.5f)
            yield return dialogBox.TypeDialog("Woah, that's a lot of damage!");
        else if (dmgDets.TypeEffectiveness < .5f)
            yield return dialogBox.TypeDialog("Eh, didn't seem to have as much effect.");
    }

    private IEnumerator FinishBattle()
    {
        GameManager.instance.StopSound("BattleMusic", 5f);

        // Player died
        if (battleResult == "lose")
        {
            playerUnit.Death();
            yield return dialogBox.TypeDialog(GameManager.instance.playerName + " was killed!");
            yield return dialogBox.TypeDialog("A hero has fallen.", 2f);
        }
        // Player ran away
        else if (battleResult == "playerRan")
        {
            playerUnit.RunAway();
            yield return new WaitUntil(playerUnit.IsDead);
            yield return dialogBox.TypeDialog("You successfully escaped..");
        }
        // Player won or the enemy ran away
        else
        {
            int XP;
            if (battleResult == "enemyRan")
            {
                XP = (int)(enemy.xpGivenOnDeath * 0.5);
                enemyUnit.RunAway();
                yield return dialogBox.TypeDialog(enemy.enemyName + " ran away in fear!");
            }
            else
            {
                XP = enemy.xpGivenOnDeath;
                enemyUnit.Death();
                yield return dialogBox.TypeDialog(enemy.enemyName + " was killed!");
            }

            yield return new WaitUntil(enemyUnit.IsDead);
            playerUnit.Win();
            yield return dialogBox.TypeDialog(GameManager.instance.playerName + " gained " + XP + "xp!", 1.5f);
            GameManager.instance.ReceiveXP(XP);
            if (GameManager.instance.GetCurrentXP() > GameManager.instance.levelUpXPNeeded)
            {
                GameManager.instance.LevelUp();
                yield return dialogBox.TypeDialog(GameManager.instance.playerName + " leveled up to level " + GameManager.instance.level + "!!!");
                yield return new WaitForSeconds(1.75f);
                if (GameManager.instance.level == 2)
                {
                    yield return dialogBox.TypeDialog(GameManager.instance.playerName + " learned " + Moves.GetSpecificMove(player.type, 1).moveName + "!");
                    yield return new WaitForSeconds(.75f);
                }
            }
        }

        GameManager.instance.StopAllSound();
        yield return GameManager.instance.FadeOut();
        yield return new WaitForSeconds(.1f);
        playerUnit.ResetAnimations();
        enemyUnit.ResetAnimations();
        yield return new WaitForSeconds(.1f);
        GameManager.instance.EndBattleScene(enemyObj, battleResult);
    }
}