using System.Collections.Generic;
using UnityEngine;

public class Shlimey : Enemy
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

    // PATROL LOGIC
    // protected Vector3 leftPatrolEndpoint;
    // protected Vector3 rightPatrolEndpoint;
    // protected bool movingRight = false;
    // protected bool movingLeft = true;

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
    private bool facingRight;
    [SerializeField]
    private bool movingUp;
    [SerializeField]
    private bool circularPatrol;
    private Vector3 downPatrolEndpoint;
    private Vector3 upPatrolEndpoint;

    protected override void Start()
    {
        // MovingEntity 'base.Start()' sets up the colliders/animator
        // Enemy 'base.Start()' sets up the starting position and halves speed for patrolling
        base.Start();

        // Patrol setup
        leftPatrolEndpoint = new Vector2(startingPosition.x - 0.6f, startingPosition.y);
        rightPatrolEndpoint = new Vector2(startingPosition.x + 0.6f, startingPosition.y);
        upPatrolEndpoint = new Vector2(startingPosition.x, startingPosition.y + 0.6f);
        downPatrolEndpoint = new Vector2(startingPosition.x, startingPosition.y - 0.6f);
        if (facingRight)
        {
            movingLeft = false;
            movingRight = true;
            anim.SetTrigger("TurnRight");
        }

        // Speed setup
        xSpeed = .275f;
        ySpeed = .275f;

        // Chase setup
        triggerLength = 0.5f;
        chaseLength = 0.75f;

        // Information setup
        enemyName = "Shlimey";
        enemyInfo = "A gross ball of slime and sludge... with a face. It smells like battery acid burned" +
            "through a pile of hair stuck together with sewage. Ew.";
        trait = "WARY";
        type = "TOXIC";

        // 35% chance for a high level enemy
        if (Random.value < 0.35)
            level = 5;
        else
            level = Random.Range(3, 5);
        xpGivenOnDeath = Random.Range(40, 50) + (15 * level);

        health = 170 + (5 * level);
        maxHealth = 170 + (5 * level);
        toughness += .25f * level;
        strength += .25f * level;

        moveSet = new List<Move>();
        // Run Away!
        moveSet.Add(Moves.GetSpecificMove("RUNAWAY", 0));
        // Sludge Bomb
        moveSet.Add(Moves.GetSpecificMove("TOXIC", 0));
        // Slime Spout
        moveSet.Add(Moves.GetSpecificMove("TOXIC", 2));
        // Noxious Blast
        moveSet.Add(Moves.GetSpecificMove("TOXIC", 3));
        // Toxic Cleanse
        moveSet.Add(Moves.GetSpecificMove("TOXIC", 1));

        critChance = 0.04f;
    }

    // Enemy.FixedUpdate() checks for the player within chase distance, checks for collision, and moves/animates
    protected override void Patrol()
    {
        if (circularPatrol)
        {
            // Moving in a circle!
            if (movingLeft)
            {
                // If left endpoint is hit, switch
                if (transform.position.x < leftPatrolEndpoint.x + .3f)
                {
                    movingLeft = false; return;
                }

                if (movingUp)
                {
                    // If top endpoint is hit, switch
                    if (transform.position.y > upPatrolEndpoint.y - .3f)
                    {
                        movingUp = false; return;
                    }
                    // Move up, left
                    movement = upPatrolEndpoint - transform.position;
                }
                else
                    // Move down, left
                    movement = leftPatrolEndpoint - transform.position;
            }
            else
            {
                // If right endpoint is hit, switch
                if (transform.position.x > rightPatrolEndpoint.x - .3f)
                {
                    movingLeft = true; return;
                }

                if (!movingUp)
                {
                    // If bottom endpoint is hit, switch
                    if (transform.position.y < downPatrolEndpoint.y + .3f)
                    {
                        movingUp = true; return;
                    }
                    // Move down, right
                    movement = downPatrolEndpoint - transform.position;
                }
                else
                    // Move up, right
                    movement = rightPatrolEndpoint - transform.position;
            }

            FaceCurrentMovement();
        }
        else
            base.Patrol();
    }

    // Enemy.TakeDamage() method for taking damage from the player

    public override Move GetMoveToUse()
    {
        // If health < 25%
        if (health < health * 0.25)
        {
            float specialChance = Random.value;
            // 50% chance to Heal
            if (specialChance < .5f)
                return moveSet[4];
            // 20% chance for Noxious Blast
            else if (specialChance > .85f)
                return moveSet[3];
        }

        // Randomly generate a chance number
        float chance = Random.value;

        Move move;
        // 60% chance to Sludge Bomb
        if (chance > .6f)
            move = moveSet[1];
        // 30% chance to Slime Spout
        else if (.6f > chance && chance > .1f)
            move = moveSet[2];
        // 10% chance to Noxious Blast
        else
            move = moveSet[3];

        // If the enemy is below 15% health and chance is above 85% or the enemy is too scared to continue, RUN
        if ((double)health < health * 0.15 && (chance > 0.85f || !TraitDecision.WillDoAction(this)))
            move = moveSet[0];

        return move;
    }
}