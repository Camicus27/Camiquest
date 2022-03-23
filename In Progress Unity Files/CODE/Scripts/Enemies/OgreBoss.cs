using System.Collections.Generic;
using UnityEngine;

public class OgreBoss : Enemy
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

    [SerializeField]
    private Lever lever;

    protected override void Start()
    {
        // MovingEntity 'base.Start()' sets up the colliders/animator
        // Enemy 'base.Start()' sets up the starting position and halves speed for patrolling
        base.Start();

        // Patrol setup
        leftPatrolEndpoint = new Vector2(startingPosition.x - 0.65f, startingPosition.y);
        rightPatrolEndpoint = new Vector2(startingPosition.x + 0.65f, startingPosition.y);

        // Speed setup
        xSpeed = .15f;
        ySpeed = .2f;

        // Chase setup
        triggerLength = 0.6f;
        chaseLength = 0.9f;

        // Information setup
        enemyName = "Destro the Ogre";
        enemyInfo = "A large and very deadly Ogre. He does NOT look very happy to see you.";
        trait = "BOLD";
        type = "TOXIC";

        health = 250;
        maxHealth = 250;
        toughness = 2;
        strength = 8;

        level = 5;
        xpGivenOnDeath = Random.Range(100, 125);

        moveSet = new List<Move>();
        // Bash
        moveSet.Add(Moves.GetSpecificMove("KINETIC", 1));
        // Smash
        moveSet.Add(Moves.GetSpecificMove("KINETIC", 2));
        // Healing Pool
        moveSet.Add(Moves.GetSpecificMove("PURE", 1));
        // Head Mash
        moveSet.Add(Moves.GetSpecificMove("KINETIC", 3));
        // Run Away!
        moveSet.Add(Moves.GetSpecificMove("RUNAWAY", 0));

        critChance = 0.0f;
    }

    // Enemy.FixedUpdate() checks for the player within chase distance, checks for collision, and moves/animates
    // It also calls Patrol() and runs that method whether overridden here or the basic left/right in the enemy class

    // Enemy.TakeDamage() method for taking damage from the player

    public override Move GetMoveToUse()
    {
        // Randomly generate a chance number
        float chance = Random.value;

        // If health <= 20%
        if (health <= health * 0.2)
        {
            // 40% chance for Smash
            if (chance >= .6f)
                return moveSet[1];
            // 30% chance for Head Mash
            else if (.6f > chance && chance >= .3f)
                return moveSet[3];
            // 29% chance to heal
            else if (.3f > chance && chance > .01f)
                return moveSet[2];
            // 1% chance to RUN
            else
                return moveSet[4];
        }

        Move move;
        // 45% chance to BASH
        if (chance >= .55f)
            move = moveSet[0];
        // 33% chance to SMASH
        else if (.55f > chance && chance >= .22f)
            move = moveSet[1];
        // 12% chance to HEAL
        else if (.22f > chance && chance >= .1f)
            move = moveSet[2];
        // 10% chance to HEAD MASH
        else
            move = moveSet[3];

        return move;
    }

    public void OnDestroy()
    {
        lever.ON = true;
    }
}
