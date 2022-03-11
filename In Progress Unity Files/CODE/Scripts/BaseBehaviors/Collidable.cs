using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collidable : MonoBehaviour
{
    protected Rigidbody2D rigidBody;
    protected bool isCollidable = true;

    protected virtual void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        // Collision work
        if (isCollidable && rigidBody.IsTouchingLayers(LayerMask.GetMask("Player")))
            OnCollide();
    }

    protected virtual void OnCollide()
    {
        // Meant to be overridden
    }
}
