using UnityEngine;

public abstract class MovingEntity : MonoBehaviour
{
    protected BoxCollider2D boxCollider;
    protected CircleCollider2D circleCollider;
    protected Rigidbody2D rigidBody;
    protected Animator anim;
    protected Vector2 movement;

    public float ySpeed;
    public float xSpeed;

    public bool isMoving = false;
    public bool canMove = true;

    protected virtual void Start()
    {
        // Get necessary components from parent object
        try { boxCollider = GetComponent<BoxCollider2D>(); } catch { }
        try { circleCollider = GetComponent<CircleCollider2D>(); } catch { }
        rigidBody = GetComponent<Rigidbody2D>();
        try { anim = GetComponent<Animator>();

        // Set start animation state
        anim.SetBool("Moving", false);
        anim.SetBool("TakingDamage", false);
        anim.SetFloat("Vertical", 0);
        anim.SetFloat("LastMoveVert", 0);
        anim.SetFloat("Horizontal", 1);
        anim.SetFloat("LastMoveHor", 1);
        }
        catch { }
    }

    protected void MoveAndAnimate()
    {
        Move();
        Animate();
    }

    protected void Move()
    {
        if (!canMove)
            return;

        // Set velocity with the speed multipliers
        rigidBody.velocity = new Vector2(movement.x * xSpeed, movement.y * ySpeed);

        // Verify isMoving state
        isMoving = movement != Vector2.zero;
    }

    protected void Animate()
    {
        // Maybe use this at some point to detect collision with certain layers?
        // rigidBody.IsTouchingLayers(LayerMask.GetMask("Actor", "Blocking"))

        if (isMoving)
        {
            // Moving horizontally
            if (movement.x > 0 || 0 > movement.x)
            {
                // Update animation state
                anim.SetFloat("Horizontal", movement.x);
                anim.SetFloat("LastMoveHor", movement.x);

                anim.SetFloat("Vertical", 0);
                anim.SetFloat("LastMoveVert", 0);
            }
            // Moving Vertically, not moving horizontally
            else if (movement.y > 0 || 0 > movement.y)
            {
                // Update animation state
                anim.SetFloat("Vertical", movement.y);
                anim.SetFloat("LastMoveVert", movement.y);

                anim.SetFloat("Horizontal", 0);
                anim.SetFloat("LastMoveHor", 0);
            }
        }

        // Update moving state
        anim.SetBool("Moving", isMoving);
    }
}
