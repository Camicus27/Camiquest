using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool sceneIsLoaded = false;

    // Play Test Stuff
    private bool GodMode = false;
    private float playerStrength;
    private float playerToughness;

    private void Awake()
    {
        if (GameManager.instance != null)
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
        player = playerObj.GetComponent<Player>();

        DontDestroyOnLoad(popupUI);
        textManager = popupUI.GetComponent<TextManager>();

        DontDestroyOnLoad(invObj);
        invManager = invObj.GetComponent<CharacterMenu>();

        DontDestroyOnLoad(audioObj);
        audioManager = audioObj.GetComponent<AudioManager>();

        DontDestroyOnLoad(eventManager);
        DontDestroyOnLoad(battleSystem);

        // Play Test Stuff
        Debug.Log("No Clip: F9");
        Debug.Log("God Mode: F10");
        Debug.Log("Cursor Toggle: F11");
    }
    private void SceneHasLoaded(Scene s, LoadSceneMode mode)
    {
        StartCoroutine(SceneLoadLate(s));
    }
    private IEnumerator SceneLoadLate(Scene s)
    {
        // Delay to be absolutely sure the scene has loaded
        yield return new WaitForSeconds(.25f);

        playerObj.transform.position = GameObject.Find("Spawnpoint").transform.position;

        switch (s.name)
        {
            case "OpeningLevel":
                yield return new WaitForSecondsRealtime(.25f);
                PlaySound("SomberMusic", 3f);
                break;
            case "Area1":
                PlaySound("SlimeMusic", 3f);
                break;
            case "GameOver":
                audioManager.StopSlowly(audioManager.GetCurrentlyPlayingSong(), 3f);
                StartCoroutine(GameOverState());
                sceneIsLoaded = true;
                canQuit = true;
                yield break;
        }

        sceneExclusiveContainer = GameObject.FindGameObjectWithTag("SceneContainer");

        sceneIsLoaded = true;

        // Fade in the scene
        yield return FadeIn();
        canQuit = true;
    }


    public List<int> xpTable;
    public int fps = 59;
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
    private GameObject sceneExclusiveContainer;
    public GameObject keyPrompt;

    public string[] sceneNames;

    // Logic
    public string playerName = "Default Name";
    private int gold;
    public int totalGold;
    private int xp;
    public int totalXP;
    public int levelUpXPNeeded = 100;
    public int level = 1;
    public int health = 100;
    public int maxHealth = 100;
    public int MP = 100;
    public int maxMana = 100;
    public int totalDmgGiven;
    public int totalDmgReceived;
    public bool invCanBeShown = false;
    
    private float alpha = 1f;
    public int selection;

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
    /// Save the state.
    /// 
    /// STRING name
    /// INT health
    /// INT gold
    /// INT xp
    /// INT level
    /// 
    /// </summary>
    public void SaveState()
    {
        string state = "";

        state += playerName + "_";
        state += health + "_";
        state += gold + "_";
        state += xp + "_";
        state += level;

        PlayerPrefs.SetString("SaveState", state);
        Debug.Log("Saved!");
    }

    public void LoadState(Scene s, LoadSceneMode mode)
    {
        if (!PlayerPrefs.HasKey("SaveState"))
            return;

        string[] data = PlayerPrefs.GetString("SaveState").Split('_');

        // Change name
        playerName = data[0];
        // Change health amount
        health = int.Parse(data[1]);
        // Change gold amount
        gold = int.Parse(data[2]);
        // Change xp amount
        xp = int.Parse(data[3]);
        // Change player level
        level = int.Parse(data[4]);

        Debug.Log("Data Loaded!");
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
        if (Input.GetKeyDown(KeyCode.F9))
        {
            if (playerObj.GetComponent<CircleCollider2D>().enabled == false)
            {
                Debug.Log("No Clip Off!");
                playerObj.GetComponent<CircleCollider2D>().enabled = true;
            }
            else
            {
                Debug.Log("No Clip On!");
                playerObj.GetComponent<CircleCollider2D>().enabled = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.F10))
        {
            if (GodMode)
            {
                Debug.Log("God Mode Off!");
                GodMode = false;
                player.strength = playerStrength;
                player.toughness = playerToughness;
            }
            else
            {
                Debug.Log("God Mode On!");
                GodMode = true;
                playerStrength = player.strength;
                playerToughness = player.toughness;
                player.strength = 500;
                player.toughness = 500;
            }
        }
        if (Input.GetKeyDown(KeyCode.F11))
        {
            if (Cursor.visible)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
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
                alpha += 0.005f;
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
                invManager.UpdateMenu();
                // show
                invObj.SetActive(true);
                // stop player movement
                player.canMove = false;
            }
            // If inventory is currently showing
            else
            {
                PlaySound("CloseInventory");
                // hide
                invObj.SetActive(false);
                // resume player movement
                player.canMove = true;
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
        GameObject enemy = null;
        foreach (Enemy obj in FindObjectsOfType<Enemy>(true))
        {
            if (obj.GetComponent<Enemy>().isFighting)
                enemy = obj.gameObject;
            obj.gameObject.SetActive(false);
        }

        // Enable battle scene and start the music
        battleSystem.SetActive(true);
        audioManager.PlaySlowly("BattleMusic", 2f);

        // Begin the battle with the attacking enemy
        battleSystem.GetComponent<BattleManager>().StartBattle(enemy);
    }

    public void EndBattleScene(GameObject enemyObj, string result)
    {
        // Reset the player's strength/toughness
        player.strength = 0;
        player.toughness = 0;

        if (result == "win" || result == "enemyRan")
        {
            // Kill enemy
            Destroy(enemyObj);

            // Re-enable player movement and inventory
            player.canMove = true;
            invCanBeShown = true;
            popupUI.GetComponentInChildren<Image>(true).enabled = true;

            // Unpause the music
            audioManager.PlaySlowly(audioManager.currentSong.name, 2f);
        }
        else if (result == "run")
        {
            // Enemy is on uncollidable cooldown
            enemyObj.SetActive(true);
            enemyObj.GetComponent<Enemy>().canMove = false;
            enemyObj.GetComponent<Enemy>().isFighting = false;
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

        // Disable battle scene object
        battleSystem.SetActive(false);
        // Fade back in
        StartCoroutine(FadeIn());

        foreach (Enemy obj in FindObjectsOfType<Enemy>(true))
            obj.gameObject.SetActive(true);
    }
    private IEnumerator EnemyInvulnerableCooldown(GameObject enemyObj)
    {
        yield return new WaitForSeconds(2.5f);
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
        return gold;
    }
    /// <summary>
    /// Gives the player gold and adds to the total gold stat
    /// </summary>
    /// <param name="goldAmount">Gold to earn</param>
    public void ReceiveGold(int goldAmount)
    {
        gold += goldAmount;
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
        if (gold - goldAmount >= 0)
        {
            gold -= goldAmount;
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
        return xp;
    }
    /// <summary>
    /// Gives the player XP and adds to the total XP stat
    /// </summary>
    /// <param name="xpAmount"></param>
    public void ReceiveXP(int xpAmount)
    {
        xp += xpAmount;
        totalXP += xpAmount;
    }
    /// <summary>
    /// Levels up the player and gives the player the level specific stat bonuses
    /// </summary>
    public void LevelUp()
    {
        PlaySound("LeveledUp");

        player.strength += 1;
        player.toughness += 1;
        maxHealth += 50;
        maxMana += 50;

        // Level up to level 2
        if (level == 1)
        {
            ReceiveGold(100);
            player.AddMoveToMoveOptions(Moves.GetSpecificMove(player.type, 1));
            xp -= levelUpXPNeeded;
            levelUpXPNeeded = xpTable[level - 1];
            level++;
        }
        // Level up to level 3, 4, 5
        else if (level == 2 || level == 3 || level == 4)
        {
            ReceiveGold(50);
            xp -= levelUpXPNeeded;
            levelUpXPNeeded = xpTable[level - 1];
            level++;
        }
        // Level up to level 6
        else if (level == 5)
        {
            ReceiveGold(100);
            player.AddMoveToMoveOptions(Moves.GetSpecificMove("KINETIC", 4));
            xp -= levelUpXPNeeded;
            levelUpXPNeeded = xpTable[level - 1];
            level++;
        }
        // Level up to level 7, 8, 9
        else if (level == 6 || level == 7 || level == 8)
        {
            maxHealth += 50;
            maxMana += 50;
            ReceiveGold(50);
            xp -= levelUpXPNeeded;
            levelUpXPNeeded = xpTable[level - 1];
            level++;
        }
        // Level up to level 10
        else if (level == 9)
        {
            maxHealth = 750;
            ReceiveGold(500);
            levelUpXPNeeded = 0;
            xp = totalXP;
            level++;
        }
        // Level up past level 10
        else
        {
            maxHealth = 750;
            player.strength += 2;
            player.toughness += 2;
        }

        health = maxHealth;
        MP = maxMana;
    }
}
