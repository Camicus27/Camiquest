using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingEntity
{
    public List<Move> allMoves;
    public Move[] moveSet;
    // NOTE: Forceful slice bought from the shop is the swordDmg * 1.25, remember to increase that if sword upgrading becomes a thing.
    public int swordDamage = 15;
    public Dictionary<Item, int> items;
    private bool takingLiveDamage;
    private bool onCooldown;
    public float strength;
    public float toughness;
    // "specialty"
    public string type = "KINETIC";
    public float critChance = 0.025f;

    private void Awake()
    {
        xSpeed = 1f;
        ySpeed = .8f;

        strength = 0f;
        toughness = 0f;

        allMoves = new List<Move>();
        moveSet = new Move[] { };
        // Sword
        AddMoveToMoveOptions(Moves.GetSpecificMove("KINETIC", 0));

        //AddMoveToMoveOptions(Moves.GetRandomMoveOfType("FIRE"));
        //AddMoveToMoveOptions(Moves.GetRandomMoveOfType("WATER"));
        //AddMoveToMoveOptions(Moves.GetRandomMoveOfType("EARTH"));
        //AddMoveToMoveOptions(Moves.GetRandomMoveOfType("TOXIC"));
        //AddMoveToMoveOptions(Moves.GetRandomMoveOfType("UNDEAD"));

        items = new Dictionary<Item, int>();
        items.Add(new Item("Health"), 2);
        items.Add(new Item("Mana"), 2);
        items.Add(new Item("Strength"), 1);
        items.Add(new Item("Toughness"), 1);
    }

    protected override void Start()
    {
        base.Start();
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (canMove)
        {
            // left == -1 ; no move == 0 ; right == 1
            movement.x = Input.GetAxisRaw("Horizontal");
            // down == -1 ; no move == 0 ; up == 1
            movement.y = Input.GetAxisRaw("Vertical");

            if (takingLiveDamage)
                anim.SetBool("TakingDamage", true);
            else
                anim.SetBool("TakingDamage", false);

            Move();
        }
        else
        {
            if (isMoving)
            {
                isMoving = false;
                rigidBody.velocity = Vector2.zero;
            }

            if (takingLiveDamage)
                anim.SetBool("TakingDamage", true);
            else
                anim.SetBool("TakingDamage", false);
        }

        Animate();
    }

    public Animator GetAnimator()
    {
        return anim;
    }

    public bool TryCollectItem(Item item)
    {
        if (items[item] == 99)
            return false;

        items[item] += 1;
        return true;
    }
    public bool TryConsumeItem(Item item)
    {
        if (items[item] == 0)
            return false;

        switch (item.type)
        {
            case "HEALTH":
                GameManager.instance.health += item.effectivenessPoints;
                if (GameManager.instance.health > GameManager.instance.maxHealth)
                    GameManager.instance.health = GameManager.instance.maxHealth;
                break;
            case "MANA":
                GameManager.instance.MP += item.effectivenessPoints;
                if (GameManager.instance.MP > GameManager.instance.maxMana)
                    GameManager.instance.MP = GameManager.instance.maxMana;
                break;
            case "STRENGTH":
                strength += item.effectivenessPoints;
                break;
            case "TOUGHNESS":
                toughness += item.effectivenessPoints;
                break;

        }
        items[item] -= 1;
        return true;
    }
    public bool TryRemoveItem(Item item)
    {
        if (items[item] == 0)
            return false;
        else
        {
            items[item] -= 1;
            return true;
        }
    }
    public int GetItemCount(Item item) { return items[item]; }

    public void AddMoveToMoveOptions(Move move)
    {
        // Cancel if already have move
        if (HasMove(move))
            return;

        allMoves.Add(move);
        if (moveSet.Length < 4)
        {
            switch (moveSet.Length)
            {
                case 0:
                    moveSet = new Move[] { move };
                    break;
                case 1:
                    moveSet = new Move[] { moveSet[0], move };
                    break;
                case 2:
                    moveSet = new Move[] { moveSet[0], moveSet[1], move };
                    break;
                case 3:
                    moveSet = new Move[] { moveSet[0], moveSet[1], moveSet[2], move };
                    break;
            }
        }
    }

    public void SetMoveInMoveSet(Move move, int position)
    {
        moveSet[position] = move;
    }

    public bool HasMove(Move move)
    {
        return allMoves.Contains(move);
    }

    public DamageDetails TakeDamage(Move move, Enemy attacker)
    {
        // Find type effectiveness
        float effectiveness = TypeChart.GetEffectiveness(move.type, type);

        // Calculate crit chance
        float critDmg = 1f;
        if (Random.value <= move.critChance + attacker.critChance)
            critDmg = 1.5f;

        // Calculate the modifiers
        float modifiers = ((Random.Range(0.93f, 1f) * critDmg) + (attacker.strength / 30) - (toughness / 30)) * effectiveness;

        // Calculate total damage
        int damage = Mathf.FloorToInt(move.damage * modifiers);    if (damage <= 0) damage = 0;
        GameManager.instance.totalDmgReceived += damage;

        // Set damage details
        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = effectiveness,
            Critical = critDmg,
            Died = false
        };

        // Apply damage
        GameManager.instance.health -= damage;
        if (GameManager.instance.health <= 0)
        {
            GameManager.instance.health = 0;
            damageDetails.Died = true;
        }

        return damageDetails;
    }

    public DamageDetails TakeHealth(Move move)
    {
        // Calculate the modifiers
        float modifiers = Random.Range(0.93f, 1f) + (strength / 30);

        // Calculate total healing
        int healing = Mathf.FloorToInt(move.damage * modifiers);

        // Set damage details
        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = 1f,
            Critical = 1f,
            Died = false
        };

        // Apply health
        GameManager.instance.health += healing;
        if (GameManager.instance.health >= GameManager.instance.maxHealth)
            GameManager.instance.health = GameManager.instance.maxHealth;

        return damageDetails;
    }

    public bool TakeLiveDamage(int damage)
    {
        if (!onCooldown)
        {
            onCooldown = true;
            if (GameManager.instance.health >= GameManager.instance.health * 0.25f)
                GameManager.instance.health -= damage;
            StartCoroutine(AnimateLiveDamage());
            return true;
        }
        else
            return false;
    }
    // Slow down the player and animate the red "hit" sprite
    private IEnumerator AnimateLiveDamage()
    {
        takingLiveDamage = true;
        xSpeed /= 2f;
        ySpeed /= 2f;
        yield return new WaitForSeconds(.2f);
        takingLiveDamage = false;
        yield return new WaitForSeconds(.2f);
        takingLiveDamage = true;
        yield return new WaitForSeconds(.2f);
        takingLiveDamage = false;
        xSpeed *= 2f;
        ySpeed *= 2f;
        onCooldown = false;
    }
}

public class DamageDetails
{
    public bool Died { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}
