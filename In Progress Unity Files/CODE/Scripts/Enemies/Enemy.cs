using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy class, meant to act as a base class which future various enemy types will inherit from
/// This may or may not require a rework on some classes that deal with enemies such as, movingEntity, Collidable,
/// BattleHUD/BattleUnit, GameManager, and im sure more.
/// </summary>
public class Enemy : MovingEntity
{
    // INHERITANCE
    // Inherited values to set:
    //  Note: These are for patrol, it will double for a chase
    // protected float xSpeed
    // protected float ySpeed
    //
    // Inherited values to be aware of:
    // public bool isMoving = false;
    // public bool canMove = true;
    // protected Animator anim  <- gains ref through script, just be sure enemy has one

    // CHASE LOGIC
    // To be overridden
    protected float triggerLength = 0.5f; // range of attack trigger
    protected float chaseLength = 0.75f;  // range of chase continuation desire
    protected Vector3 startingPosition;   // the position to return to after losing a chase
    // To be handled here
    private bool chasing = false;
    private bool returning = false;
    private Transform playerTransform;

    // probably rework this to be more modular, maybe just make the patrol
    // logic virutal and each enemy overrides how they patrol?
    // Patrol logic
    protected Vector3 leftPatrolEndpoint;
    protected Vector3 rightPatrolEndpoint;
    protected bool movingRight = false;
    protected bool movingLeft = true;

    // STATS LOGIC
    // All of this is to be overridden by the specific enemy
    // this is just some 'defaults'
    public bool isFighting = false;
    public string enemyName = "DefaultEnemy";
    public int health = 100;
    public int maxHealth = 100;
    public float toughness = 1;
    public float strength = 1;
    public int level = 1;
    public int xpGivenOnDeath = 100;
    public float critChance = 0.025f;
    public List<Move> moveSet;
    public string enemyInfo = "This lil guy uses a scary mask to hide his insecurities... \n" +
        "He's kinda cute tho...";
    public string trait = "TIMID";
    public string type = "UNDEAD";


    public RuntimeAnimatorController walkingAnim;
    public RuntimeAnimatorController battlingAnim;


    // Rework this to be more default
    protected override void Start()
    {
        // MovingEntity 'base.Start()' sets up the colliders/animator
        base.Start();

        startingPosition = transform.position;

        //// probably rework this with patrol method
        //leftPatrolEndpoint = new Vector2(startingPosition.x - 0.7f, startingPosition.y);
        //rightPatrolEndpoint = new Vector2(startingPosition.x + 0.7f, startingPosition.y);
        //
        ////
        //// Make all the following code specific to an enemy, not done here
        ////
        //
        //xSpeed = .5f;
        //ySpeed = .5f;
        //
        //if (level == 0)
        //{
        //    if (Random.value < 0.2)
        //        level = Random.Range(2, 3);
        //    else
        //        level = Random.Range(1, 2);
        //}
        //
        //toughness += .9f * level;
        //strength += .9f * level;
        //xpGivenOnDeath = (int)(Random.Range(10f, 15f) * level);
        //
        //moveSet = new List<Move>(2);
        //moveSet.Add(null); moveSet.Add(null);
        //else if (enemyName == "Firite")
        //{
        //    moveSet[0] = new Move("Fire Flick", "Flick a few small shards of flame and ashes.", "FIRE", 20, 0);
        //    moveSet[1] = new Move("Ignite", "Fuels the fire.", "PURE", 20, 0, true);
        //}
        //else if (enemyName == "Destro the Ogre")
        //{
        //    moveSet[0] = new Move("Smash", "Smash enemies using your large fists.", "KINETIC", 30, 0);
        //    moveSet[1] = new Move("Head Bash", "Violently hurl yourself at your enemy.", "KINETIC", 50, 0);
        //}
    }

    /// <summary>
    /// Patrol, check for the player within chase distance, check for collision, and move/animate
    /// </summary>
    private void FixedUpdate()
    {
        // Check if player has loaded, attempt to load if not
        if (playerTransform == null)
        {
            playerTransform = GameManager.instance.playerObj.transform;
            return;
        }

        // If player is within trigger distance and not already chasing, start chasing and stop returning if applicable
        if (!chasing && Vector3.Distance(playerTransform.position, transform.position) < triggerLength)
        {
            chasing = true;
            returning = false;
            GameManager.instance.PlaySound("EnemyAlert");
            FaceThePlayer();
            anim.SetBool("isRunning", true);
            xSpeed *= 2;
            ySpeed *= 2;
        }

        // If we are chasing
        if (chasing)
        {
            // If the player is out of chase range, stop chasing and start returning
            if (Vector3.Distance(playerTransform.position, transform.position) > chaseLength)
            {
                chasing = false;
                returning = true;
                xSpeed /= 2;
                ySpeed /= 2;
                anim.SetBool("isRunning", false);
            }
            else
            {
                // Set movement towards the player
                movement = (playerTransform.position - transform.position).normalized;
                FaceThePlayer();
            }
        }
        // Player is no longer in range
        else
        {
            // Return toward start position if returning
            if (returning)
            {
                if (Vector3.Distance(startingPosition, transform.position) > 0.3f)
                {
                    movement = (startingPosition - transform.position).normalized;
                    FaceCurrentMovement();
                }
                else
                {
                    returning = false;
                }
            }
            // Continue patroling area
            else
                Patrol();
        }

        // Update position and animation
        //MoveAndAnimate();
        Move();

        // Check if touching the player
        if (!isFighting && rigidBody.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            isFighting = true;
            GameManager.instance.StartBattleScene();
        }
    }

    /// <summary>
    /// Patrol the area when not chasing
    /// </summary>
    protected virtual void Patrol()
    {
        // moving left
        if (movingLeft)
        {
            // At least 30px away from patrol point
            if (transform.position.x > leftPatrolEndpoint.x + 0.3f)
            {
                // move left
                movement = leftPatrolEndpoint - transform.position;
                FaceCurrentMovement();
            }
            else
            {
                // swap directions
                movingLeft = false;
                movingRight = true;
            }
        }
        // moving right
        else if (movingRight)
        {
            // At least 30px away from patrol point
            if (transform.position.x < rightPatrolEndpoint.x - 0.3f)
            {
                // move right
                movement = rightPatrolEndpoint - transform.position;
                FaceCurrentMovement();
            }
            else
            {
                // swap directions
                movingLeft = true;
                movingRight = false;
            }
        }
    }

    protected void FaceThePlayer()
    {
        if (playerTransform.position.x > transform.position.x)
            anim.SetTrigger("TurnRight");
        else
            anim.SetTrigger("TurnLeft");
    }

    protected void FaceCurrentMovement()
    {
        if (movement.x > 0)
            anim.SetTrigger("TurnRight");
        else
            anim.SetTrigger("TurnLeft");
    }

    /// <summary>
    /// Assess situation and return an appropriate move
    /// </summary>
    /// <returns></returns>
    public virtual Move GetMoveToUse()
    {
        Move move;
        // Randomly generate a chance number
        float chance = Random.value;
        // If the enemy is below 25% health and chance is above 70%, perform move 2
        if ((double)health < health * 0.25 && chance > 0.7f)
            move = moveSet[1];
        // If the chance is above 90%, perform move 2
        else if (chance > 0.9f)
            move = moveSet[2];
        // If the chance was below 90% or below 70% if health is below 25%, perform move 1
        else
            move = moveSet[1];

        // If the enemy is below 10% health and chance is above 85% or the enemy is too scared to continue, RUN
        if ((double)health < health * 0.1 && (chance > 0.85f || !TraitDecision.WillDoAction(this)))
            move = moveSet[0];

        return move;
    }

    /// <summary>
    /// Take damage from a player
    /// </summary>
    public DamageDetails TakeDamage(Move move, Player attacker)
    {
        // Find type effectiveness
        float effectiveness = TypeChart.GetEffectiveness(move.type, type);

        // Calculate crit chance
        float critDmg = 1f;
        if (Random.value <= move.critChance + attacker.critChance)
            critDmg = 1.75f;

        // Calculate the modifiers
        float modifiers = ((Random.Range(0.93f, 1f) * critDmg) + (attacker.strength / 33) - (toughness / 33)) * effectiveness;

        // Calculate total damage
        int damage = Mathf.FloorToInt(move.damage * modifiers);
        GameManager.instance.totalDmgGiven += damage;

        // Set damage details
        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = effectiveness,
            Critical = critDmg,
            Died = false
        };

        // Apply damage
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            damageDetails.Died = true;
        }

        return damageDetails;
    }

    /// <summary>
    /// Take health from self
    /// </summary>
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

        // Apply damage
        health += healing;
        if (health >= maxHealth)
            health = maxHealth;

        return damageDetails;
    }
}

/// <summary>
/// RNG way to see if an enemy will do something
/// </summary>
public static class TraitDecision
{
    //private enum Willingness { cowardly, wary, timid, conscious, brave, bold, fearless }

    /* eh these over complicate things */
    //private enum competency { moron, stupid, unintelligent, average, bright, intelligent, genius }
    //private enum capability { noob, unskilled, practiced, proficient, experienced, apt, master }

    public static bool WillDoAction(Enemy enemy)
    {
        float willingness = 0;
        // Factor in the enemy trait
        switch (enemy.trait)
        {
            case "COWARDLY": willingness = 1; break;
            case "WARY": willingness = 2; break;
            case "TIMID": willingness = 3; break;
            case "CONSCIOUS": willingness = 4; break;
            case "BRAVE": willingness = 5; break;
            case "BOLD": willingness = 6; break;
            case "FEARLESS": willingness = 7; break;
        }

        Debug.Log("Enemy Trait! : " + enemy.trait);

        // Based on health/willingness with a random factor (.75-1.25) and tack on an extra bit based on level
        float formula = (Random.Range(0.75f, 1.25f) * willingness * ((float)enemy.health / (float)enemy.maxHealth) + 0.33f) + ((float)enemy.level / 10f);

        Debug.Log("Base willingness : " + formula);

        // Roll for adrenaline fueled courage boost!
        if (Random.value > 0.93f)
            formula += 1.5f;

        Debug.Log("After adrenaline chance : " + formula);

        // Also slap in a type fueled courage boost! (or hindrance!)
        if (enemy.type == "UNDEAD" || enemy.type == "FIRE")
            formula *= 1.1f;
        else if (enemy.type == "TOXIC" || enemy.type == "PURE")
            formula *= 0.9f;

        Debug.Log("After type addition : " + formula + " ...is it above 4.0?");

        // Return true if fearless enough to do action, else false
        if (formula >= 4f)
            return true;
        else
            return false;
    }
}