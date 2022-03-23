using System.Collections.Generic;
using UnityEngine;

public class Maske : Enemy
{
    // INHERITANCE TO SET:
    //----------------------------------------------
    // SPEED LOGIC - Note: These are for patrol, it will double for a chase
    // protected float xSpeed
    // protected float ySpeed

    // CHASE LOGIC
    // protected float triggerLength = 0.5f;   -- range of attack trigger
    // protected float chaseLength = 0.75f;    -- range of chase continuation desire
    // protected Vector3 startingPosition;     -- the position to return to after losing a chase

    // INFO LOGIC
    // public bool isFighting = false;
    // public string enemyName = "EnemyName";
    // public int health = 100;
    // public int maxHealth = 100;
    // public float toughness = 1;
    // public float strength = 1;
    // public int level = 1;
    // public int xpGivenOnDeath = 100;
    // public float critChance = 0.025f;
    // public List<Move> moveSet;
    // public string enemyInfo = "Info about the enemy, maybe something quirky?";
    // public string trait = "TRAIT";
    // public string type = "TYPE";
    // public RuntimeAnimatorController walkingAnim;      -- Set in unity or through GetComp
    // public RuntimeAnimatorController battlingAnim;     -- Set in unity or through GetComp
    //--------------------------------------------------------------------------------------------


    // INHERITANCE TO BE AWARE OF:
    //----------------------------------------------
    // public bool isMoving = false;
    // public bool canMove = true;
    // protected Animator anim  <- gains ref through script, just be sure enemy has one

    protected override void Start()
    {
        // MovingEntity 'base.Start()' sets up the colliders/animator
        // Enemy 'base.Start()' sets up the starting position and halves speed for patrolling
        base.Start();

        // Patrol setup
        leftPatrolEndpoint = new Vector2(startingPosition.x - 0.65f, startingPosition.y);
        rightPatrolEndpoint = new Vector2(startingPosition.x + 0.65f, startingPosition.y);

        // Speed setup
        xSpeed = .3f;
        ySpeed = .25f;

        // Chase setup
        triggerLength = 0.5f;
        chaseLength = 0.75f;

        // Information setup
        enemyName = "Maske";
        enemyInfo = "This lil guy uses a scary mask to hide his insecurities... He's kinda cute tho...";
        trait = "TIMID";
        type = "UNDEAD";

        // 15% chance for a high level enemy
        if (Random.value < 0.15)
            level = 3;
        else
            level = Random.Range(1, 3);
        xpGivenOnDeath = Random.Range(30, 40) + (5 * level);

        health = 50 + (5 * level);
        maxHealth = 50 + (5 * level);
        toughness += .1f * level;
        strength += .1f * level;

        moveSet = new List<Move>();
        // Run Away!
        moveSet.Add(Moves.GetSpecificMove("RUNAWAY", 0));
        // Bash
        moveSet.Add(Moves.GetSpecificMove("KINETIC", 1));
        // Undying Mesmer
        moveSet.Add(Moves.GetSpecificMove("UNDEAD", 0));
        // Incorporeal Shot
        moveSet.Add(Moves.GetSpecificMove("UNDEAD", 1));

        critChance = 0.03f;
    }

    // Enemy.FixedUpdate() checks for the player within chase distance, checks for collision, and moves/animates
    // It also calls Patrol() and runs that method whether overridden here or the basic left/right in the enemy class

    // Enemy.TakeDamage() method for taking damage from the player

    public override Move GetMoveToUse()
    {
        // Randomly generate a chance number
        float chance = Random.value;

        Move move;
        // 60% chance to BASH
        if (chance > .6f)
            move = moveSet[1];
        // 30% chance to UNDYING MESMER
        else if (.6f > chance && chance > .08f)
            move = moveSet[2];
        // 8% chance to INCORPOREAL SHOT
        else
            move = moveSet[3];

        // If the enemy is below 15% health and chance is above 85% or the enemy is too scared to continue, RUN
        if ((double)health < health * 0.15 && (chance > 0.85f || !TraitDecision.WillDoAction(this)))
            move = moveSet[0];

        return move;
    }
}
