using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool sceneIsLoaded = false;
    public bool isBackTracking;
    public bool isLoadingFromSave;

    // Play Test Stuff
    private bool GodMode = false;
    private bool SpeedMode = false;
    private float playerTempStrength;
    private float playerTempToughness;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            Destroy(playerObj);
            Destroy(popupUI.gameObject);
            Destroy(eventManager.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += SceneHasLoaded;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        DontDestroyOnLoad(playerObj);

        DontDestroyOnLoad(popupUI);
        textManager = popupUI.GetComponent<TextManager>();

        DontDestroyOnLoad(invObj);
        invManager = invObj.GetComponent<CharacterMenu>();

        DontDestroyOnLoad(audioObj);
        audioManager = audioObj.GetComponent<AudioManager>();

        DontDestroyOnLoad(eventManager);
        DontDestroyOnLoad(battleSystem);
    }
    private void SceneHasLoaded(Scene s, LoadSceneMode mode)
    {
        StartCoroutine(SceneLoadLate(s));
    }
    private IEnumerator SceneLoadLate(Scene s)
    {
        // Delay to be absolutely sure the scene has loaded
        yield return new WaitForSeconds(.005f);

        // Play the appropriate music for the scene
        switch (s.name)
        {
            case "MainMenu":
                StartCoroutine(FadeIn());
                PlaySound("SomberMusic", 2f);
                invCanBeShown = false;
                canQuit = false;
                sceneIsLoaded = true;
                yield break;
            case "GameOver":
                PlaySound("SomberMusic", 2f);
                break;
            case "Area0":
                if (!isBackTracking)
                {
                    invCanBeShown = false;
                    canQuit = false;
                    sceneIsLoaded = true;
                    playerObj.transform.position = GameObject.Find("Spawnpoint").transform.position;
                    FindObjectOfType<OpeningCutScene>().StartScene();
                    PlaySound("SomberMusic", 3f);
                    yield break;
                }
                else
                    PlaySound("SomberMusic", 3f);
                break;
            case "Area1":
                // Check if this is the first scene load and there isn't a savefile yet
                if (string.IsNullOrEmpty(SaveData.current.sceneName))
                {
                    playerObj.transform.position = GameObject.Find("Spawnpoint").transform.position;
                    SaveState();
                    // Write the state to the save file
                    SerializationManager.Save(SaveData.current);
                }
                PlaySound("SlimeMusic", 3f);
                break;
            case "Area2_3":
                PlaySound("SlimeMusic", 3f);
                break;
            case "EndCredits":
                audioManager.StopSlowly(audioManager.GetCurrentlyPlayingSong(), 3f);
                StartCoroutine(GameOverState());
                sceneIsLoaded = true;
                canQuit = true;
                yield break;
        }

        // Move the player into correct position
        if (isBackTracking)
            playerObj.transform.position = GameObject.Find("Backpoint").transform.position;
        else
            playerObj.transform.position = GameObject.Find("Spawnpoint").transform.position;

        // Perform all necessary save state loads
        LoadAllStates(s);

        if (isLoadingFromSave)
        {
            player.Load();
            player.canMove = true;
            invCanBeShown = true;
            popupUI.GetComponentInChildren<Image>(true).enabled = true;
            isLoadingFromSave = false;
        }

        // Tell all classes that the scene is loaded
        sceneIsLoaded = true;
        // Fade in the scene
        yield return FadeIn();
        canQuit = true;
    }

    private void LoadAllStates(Scene s)
    {
        // MAIN LOADS ------------

        // Load all enemies
        foreach (Enemy enemy in FindObjectsOfType<Enemy>(true))
        {
            // Check all saved enemies
            for (int i = 0; i < SaveData.current.enemies.Count; i++)
            {
                // Saved enemy data found
                if (SaveData.current.enemies[i].ID == enemy.ID)
                {
                    // Load the saved data into the enemy
                    enemy.LoadData(SaveData.current.enemies[i]);
                    break;
                }
            }

            // Kill enemy if dead
            if (enemy.health <= 0)
                enemy.gameObject.SetActive(false);
        }

        // Load all NPCs
        foreach (NPC npc in FindObjectsOfType<NPC>(true))
        {
            // Check all saved NPCs
            for (int i = 0; i < SaveData.current.npcs.Count; i++)
            {
                // Saved NPC data found
                if (SaveData.current.npcs[i].ID == npc.ID)
                {
                    // Load the saved data into the NPC
                    npc.LoadData(SaveData.current.npcs[i]);
                    break;
                }
            }
        }

        // Load all inspectables
        foreach (Inspectable inspectable in FindObjectsOfType<Inspectable>(true))
        {
            // Check all saved inspectables
            for (int i = 0; i < SaveData.current.inspections.Count; i++)
            {
                // Saved inspectable data found
                if (SaveData.current.inspections[i].ID == inspectable.ID)
                {
                    // Load the saved data into the inspectable
                    inspectable.LoadData(SaveData.current.inspections[i]);
                    break;
                }
            }
        }

        // Load all chests
        foreach (LootChest chest in FindObjectsOfType<LootChest>(true))
        {
            // Check all saved chests
            for (int i = 0; i < SaveData.current.chests.Count; i++)
            {
                // Saved chest data found
                if (SaveData.current.chests[i].ID == chest.ID)
                {
                    // Open the chest if needed
                    if (SaveData.current.chests[i].isOpened) chest.Open();
                    break;
                }
            }
        }

        // Load all special chests
        foreach (SpecialLootChest chest in FindObjectsOfType<SpecialLootChest>(true))
        {
            // Check all saved chests
            for (int i = 0; i < SaveData.current.specialChests.Count; i++)
            {
                // Saved chest data found
                if (SaveData.current.specialChests[i].ID == chest.ID)
                {
                    // Open the chest if needed
                    if (SaveData.current.specialChests[i].isOpened) chest.Open();
                    break;
                }
            }
        }

        // Load all doors
        foreach (Door door in FindObjectsOfType<Door>(true))
        {
            // Check all saved doors
            for (int i = 0; i < SaveData.current.doors.Count; i++)
            {
                // Saved door data found
                if (SaveData.current.doors[i].ID == door.ID)
                {
                    door.LoadData(SaveData.current.doors[i]);
                    break;
                }
            }
        }

        // Load all levers
        foreach (Lever lever in FindObjectsOfType<Lever>(true))
        {
            // Check all saved levers
            for (int i = 0; i < SaveData.current.levers.Count; i++)
            {
                // Saved lever data found
                if (SaveData.current.levers[i].ID == lever.ID)
                {
                    lever.LoadData(SaveData.current.levers[i]);
                    break;
                }
            }
        }

        // Load all skulls
        foreach (SaveSkull skull in FindObjectsOfType<SaveSkull>(true))
        {
            // Check all saved skulls
            for (int i = 0; i < SaveData.current.skulls.Count; i++)
            {
                // Saved skull data found
                if (SaveData.current.skulls[i].ID == skull.ID)
                {
                    skull.LoadData(SaveData.current.skulls[i]);
                    break;
                }
            }
        }

        // SCENE SPECIFIC LOADS -------------------------------------------------------------------

        switch (s.name)
        {
            case "OpeningLevel":
                FindObjectOfType<Statue>().LoadData();
                break;
            case "Area1":
                TheShop shop = FindObjectOfType<TheShop>();
                // Check all saved shops
                for (int i = 0; i < SaveData.current.shops.Count; i++)
                {
                    // Saved lever data found
                    if (SaveData.current.shops[i].ID == shop.ID)
                    {
                        shop.LoadData(SaveData.current.shops[i]);
                        break;
                    }
                }
                break;
            case "Area2_3":
                
                break;
        }
    }


    public List<int> xpTable;
    public float lastTime = 0;
    public bool canQuit = false;

    // References
    public GameObject playerObj;
    public Player player;
    public Canvas popupUI;
    public TextManager textManager;
    public Animator faderOverlayAnim;
    public Text quitPrompt;
    public GameObject invObj;
    private CharacterMenu invManager;
    public GameObject eventManager;
    public GameObject audioObj;
    private AudioManager audioManager;
    public GameObject battleSystem;
    public GameObject keyPrompt;

    // Player information
    public int totalGold;
    public int totalXP;
    public int levelUpXPNeeded = 100;
    public int totalDmgGiven;
    public int totalDmgReceived;
    public bool invCanBeShown = false;
    private float playerStrength;
    private float playerToughness;

    private float alpha = 1f;

    public int selection;
    public bool settingsMenuActive;

    /// <summary>
    /// Popup dialog box manager
    /// </summary>
    public IEnumerator ShowTextDialog(string message, Sprite npc, string voice) { yield return textManager.ShowDialog(message, npc, voice); }
    public IEnumerator ShowTextInfo(string message) { StopAllText(); yield return textManager.ShowInfo(message); }
    public IEnumerator ShowTextDialog(string message, Sprite npc, string voice, int lettersPerSec) { yield return textManager.ShowDialog(message, npc, voice, lettersPerSec); }
    public IEnumerator ShowTextInfo(string message, int lettersPerSec) { StopAllText(); yield return textManager.ShowInfo(message, lettersPerSec); }
    public IEnumerator ShowTextPrompt(string message, string voice, int lettersPerSec, string op1, string op2) { StopAllText(); yield return textManager.ShowPrompt(message, voice, lettersPerSec, op1, op2); }
    public IEnumerator ShowTextPrompt(string message, string voice, int lettersPerSec, string op1, string op2, string op3, string op4) { StopAllText(); yield return textManager.ShowPrompt(message, voice, lettersPerSec, op1, op2, op3, op4); }
    public int ReturnPromptSelection() { return selection; }
    public IEnumerator TypeText(string text, Text textBox, string voice, int lettersPerSecond, bool isSkippable) 
    { yield return textManager.TypeText(text, textBox, voice, lettersPerSecond, isSkippable); }
    public IEnumerator TypeTextAutoContinue(string text, Text textBox, string voice, int lettersPerSecond, float hangTimeAfter)
    { yield return textManager.TypeTextAuto(text, textBox, voice, lettersPerSecond, hangTimeAfter); }
    public void StopAllText() { textManager.StopAll(); }

    /// <summary>
    /// Audio manager
    /// </summary>
    public void PlaySound(string name) { audioManager.Play(name); }
    public void PlaySound(string name, float fadeInTime) { audioManager.PlaySlowly(name, fadeInTime); }
    public void StopSound(string name) { audioManager.Stop(name); }
    public void StopSound(string name, float fadeOutTime) { audioManager.StopSlowly(name, fadeOutTime); }
    public void StopMusic(float fadeOutTime) { audioManager.StopSlowly(audioManager.currentSong.name, fadeOutTime); }
    public void StopAllSound() { audioManager.StopAll(); }


    /// <summary>
    /// Save the state of the game
    /// 
    /// Update the current player/gamemanager save
    /// Enemies are already updated after battles
    /// </summary>
    public void SaveState()
    {
        // Update the GameManager and Player information
        SaveData.current.gameManager = instance;
        player.Save();
        SaveData.current.sceneName = SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// Load the previously saved state of the game
    /// </summary>
    public void LoadState()
    {
        instance = SaveData.current.gameManager;
        player.Load();
        SceneManager.LoadScene(SaveData.current.sceneName);
    }

    /// <summary>
    /// Checks continually for Tab (inventory) and for Esc (Quit)
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CharacterMenuToggle();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && canQuit)
        {
            Quit();
        }
        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    if (playerObj.GetComponent<CircleCollider2D>().enabled == false)
        //    {
        //        Debug.Log("No Clip Off!");
        //        playerObj.GetComponent<CircleCollider2D>().enabled = true;
        //    }
        //    else
        //    {
        //        Debug.Log("No Clip On!");
        //        playerObj.GetComponent<CircleCollider2D>().enabled = false;
        //    }
        //}
        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    if (GodMode)
        //    {
        //        Debug.Log("God Mode Off!");
        //        GodMode = false;
        //        player.strength = playerTempStrength;
        //        player.toughness = playerTempToughness;
        //    }
        //    else
        //    {
        //        Debug.Log("God Mode On!");
        //        GodMode = true;
        //        playerTempStrength = player.strength;
        //        playerTempToughness = player.toughness;
        //        player.strength = 1000;
        //        player.toughness = 1000;
        //    }
        //}
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    if (SpeedMode)
        //    {
        //        Debug.Log("Speed Mode Off!");
        //        SpeedMode = false;
        //        player.xSpeed = 1f;
        //        player.ySpeed = .8f;
        //    }
        //    else
        //    {
        //        Debug.Log("Speed Mode On!");
        //        SpeedMode = true;
        //        player.xSpeed = 2.25f;
        //        player.ySpeed = 2.25f;
        //    }
        //}
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    Debug.Log("Gave Items!");
        //    player.TryCollectItem(new Item("Health"));
        //    player.TryCollectItem(new Item("Mana"));
        //    player.TryCollectItem(new Item("Strength"));
        //    player.TryCollectItem(new Item("Toughness"));
        //}
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    Debug.Log("You're gonna die!");
        //    player.health = 1;
        //}
    }

    private void Quit()
    { StartCoroutine(Quitting()); }
    private IEnumerator Quitting()
    {
        // Fade in the quit prompt
        alpha = 0f;
        while (alpha < 1f)
        {
            // If still holding escape
            if (Input.GetKey(KeyCode.Escape))
            {
                alpha += 0.01f;
                quitPrompt.color = new Color(1, 1, 1, alpha);
                yield return null;
            }
            // If escape is released, reset the color
            else
            {
                quitPrompt.color = new Color(1, 1, 1, 0);
                yield break;
            }
        }

        // Quit prompt is fully faded in, quit the game
        Application.Quit();
    }

    public void CharacterMenuToggle()
    {
        if (invCanBeShown)
        {
            // If menu is not currently showing
            if (!invObj.activeSelf)
            {
                PlaySound("OpenInventory");
                // update display data
                invManager.Open();
                // show
                invObj.SetActive(true);
                // stop player movement
                player.canMove = false;
            }
            // If inventory is currently showing
            else
            {
                if (!settingsMenuActive)
                {
                    PlaySound("CloseInventory");
                    // hide
                    invObj.SetActive(false);
                    // resume player movement
                    player.canMove = true;
                }
            }
        }
    }

    public void StartBattleScene()
    {
        BattleStartSequence();
    }
    private void BattleStartSequence()
    {
        // Remove any dialog boxes if any
        StopAllText();
        // Stop all audio
        audioManager.StopAll();
        // Stop any coroutines
        StopAllCoroutines();

        // Disable inventory and player movement
        invObj.SetActive(false);
        invCanBeShown = false;
        player.canMove = false;
        popupUI.GetComponentInChildren<Image>(true).enabled = false;

        // Get the attacking enemy and disable all enemies in the process
        GameObject enemyObj = null;
        foreach (Enemy enemy in FindObjectsOfType<Enemy>(true))
        {
            if (enemy.GetComponent<Enemy>().isFighting)
            {
                // Save the enemy's state
                enemy.Save();
                // Set the object
                enemyObj = enemy.gameObject;
            }
            enemy.gameObject.SetActive(false);
        }

        // Store the player's strength/toughness
        playerStrength = player.strength;
        playerToughness = player.toughness;

        // Enable battle scene and start the music
        battleSystem.SetActive(true);
        audioManager.PlaySlowly("BattleMusic", 2f);

        // Begin the battle with the attacking enemy
        battleSystem.GetComponent<BattleManager>().StartBattle(enemyObj);
    }

    public void EndBattleScene(GameObject enemyObj, string result)
    {
        // Restore the player's strength/toughness
        player.strength = playerStrength;
        player.toughness = playerToughness;
        enemyObj.GetComponent<Enemy>().isFighting = false;

        if (result == "win" || result == "enemyRan")
        {
            // Kill enemy (leave them disabled)

            // Re-enable player movement and inventory
            player.canMove = true;
            invCanBeShown = true;
            popupUI.GetComponentInChildren<Image>(true).enabled = true;

            // Unpause the music
            audioManager.PlaySlowly(audioManager.currentSong.name, 3f);
        }
        else if (result == "playerRan")
        {
            // Enemy is on uncollidable cooldown
            enemyObj.SetActive(true);
            enemyObj.GetComponent<Enemy>().canMove = false;
            enemyObj.GetComponent<BoxCollider2D>().enabled = false;

            // Re-enable player movement and inventory
            player.canMove = true;
            invCanBeShown = true;
            popupUI.GetComponentInChildren<Image>(true).enabled = true;

            // Unpause the music
            audioManager.PlaySlowly(audioManager.currentSong.name, 2f);

            // Start cooldown to bring enemy back to life
            StartCoroutine(EnemyInvulnerableCooldown(enemyObj));
        }
        else if (result == "lose")
        {
            // Disable battle scene object
            battleSystem.SetActive(false);
            SceneManager.LoadScene("GameOver");
            return;
        }

        // Update save information
        enemyObj.GetComponent<Enemy>().Save();

        // Disable battle scene object
        battleSystem.SetActive(false);
        // Fade back in
        StartCoroutine(FadeIn());

        foreach (Enemy obj in FindObjectsOfType<Enemy>(true))
        {
            // Only activate alive enemies
            if (obj.health > 0)
                obj.gameObject.SetActive(true);
        }
    }
    private IEnumerator EnemyInvulnerableCooldown(GameObject enemyObj)
    {
        yield return new WaitForSecondsRealtime(2.5f);
        // Re-enable enemy movement and collision
        enemyObj.GetComponent<Enemy>().canMove = true;
        enemyObj.GetComponent<BoxCollider2D>().enabled = true;
    }

    public IEnumerator GameOverState()
    {
        // Disable inventory
        invObj.SetActive(false);
        invCanBeShown = false;

        yield return new WaitUntil(IsLoaded);

        // Update all the information on the scene
        GameOverStatistics statsScene = GameObject.Find("Statistics").GetComponent<GameOverStatistics>();
        statsScene.UpdateAllStats();

        yield return new WaitForSeconds(0.01f);

        yield return FadeIn();
    }
    private bool IsLoaded() { return sceneIsLoaded; }


    // ----------------------------------------------------------------------------------------------------------------------------------------
    // Helpers for all scripts


    /// <summary>
    /// Fade in the scene (must already be blacked out)
    /// </summary>
    public IEnumerator FadeIn()
    {
        faderOverlayAnim.SetTrigger("FadeIn");
        yield return new WaitUntil(IsTransparent);
    }
    private bool IsTransparent()
    { return faderOverlayAnim.GetCurrentAnimatorStateInfo(0).IsName("Transparent"); }
    /// <summary>
    /// Fade out the scene (must have scene visible)
    /// </summary>
    public IEnumerator FadeOut()
    {
        faderOverlayAnim.SetTrigger("FadeOut");
        yield return new WaitUntil(IsOpaque);
    }
    private bool IsOpaque()
    { return faderOverlayAnim.GetCurrentAnimatorStateInfo(0).IsName("Opaque"); }

    /// <summary>
    /// Disable the player's ability to move
    /// </summary>
    public void StopPlayerMovement()
    {
        player.canMove = false;
        invCanBeShown = false;
    }
    /// <summary>
    /// Enable the player's ability to move
    /// </summary>
    public void ResumePlayerMovement()
    {
        player.canMove = true;
        invCanBeShown = true;
    }

    /// <summary>
    /// Return player's current gold amount
    /// </summary>
    public int GetCurrentGold()
    {
        return player.gold;
    }
    /// <summary>
    /// Gives the player gold and adds to the total gold stat
    /// </summary>
    /// <param name="goldAmount">Gold to earn</param>
    public void ReceiveGold(int goldAmount)
    {
        player.gold += goldAmount;
        totalGold += goldAmount;
    }
    /// <summary>
    /// Attempt to remove gold if the player has enough
    /// to be removed without going negative
    /// </summary>
    /// <param name="goldAmount">Gold to pay</param>
    /// <returns>Whether or not the gold was paid</returns>
    public bool TryPayGold(int goldAmount)
    {
        if (player.gold - goldAmount >= 0)
        {
            player.gold -= goldAmount;
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// Return player's current XP amount
    /// </summary>
    /// <returns></returns>
    public int GetCurrentXP()
    {
        return player.xp;
    }
    /// <summary>
    /// Gives the player XP and adds to the total XP stat
    /// </summary>
    /// <param name="xpAmount"></param>
    public void ReceiveXP(int xpAmount)
    {
        player.xp += xpAmount;
        totalXP += xpAmount;
    }
    /// <summary>
    /// Levels up the player and gives the player the level specific stat bonuses
    /// </summary>
    public void LevelUp()
    {
        PlaySound("LeveledUp");

        playerStrength += 1;
        playerToughness += 1;
        player.maxHealth += 50;
        player.maxMana += 75;
        Item.healthEP += 25;
        Item.manaEP += 50;

        // Level up to level 2
        if (player.level == 1)
        {
            ReceiveGold(100);
            player.AddMoveToMoveOptions(Moves.GetSpecificMove(player.type, 1));
            player.xp -= levelUpXPNeeded;
            levelUpXPNeeded = xpTable[player.level - 1];
            player.level++;
        }
        // Level up to level 3, 4, 5
        else if (player.level == 2 || player.level == 3 || player.level == 4)
        {
            ReceiveGold(50);
            player.xp -= levelUpXPNeeded;
            levelUpXPNeeded = xpTable[player.level - 1];
            player.level++;
        }
        // Level up to level 6
        else if (player.level == 5)
        {
            ReceiveGold(100);
            player.AddMoveToMoveOptions(Moves.GetSpecificMove("KINETIC", 4));
            player.xp -= levelUpXPNeeded;
            levelUpXPNeeded = xpTable[player.level - 1];
            player.level++;
        }
        // Level up to level 7, 8, 9
        else if (player.level == 6 || player.level == 7 || player.level == 8)
        {
            player.maxHealth += 50;
            player.maxMana += 50;
            ReceiveGold(50);
            player.xp -= levelUpXPNeeded;
            levelUpXPNeeded = xpTable[player.level - 1];
            player.level++;
        }
        // Level up to level 10
        else if (player.level == 9)
        {
            player.maxHealth = 750;
            ReceiveGold(500);
            levelUpXPNeeded = 0;
            player.xp = totalXP;
            player.level++;
        }
        // Level up past level 10
        else
        {
            player.maxHealth = 750;
            playerStrength += 2;
            playerToughness += 2;
        }

        player.ShowAllDamageImprovements();
        player.health = player.maxHealth;
        player.MP = player.maxMana;
    }
}
