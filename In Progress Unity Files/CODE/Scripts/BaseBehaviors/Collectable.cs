using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : Collidable
{
    // Logic
    protected bool collected;
    public Sprite collectedSprite;

    protected override void OnCollide()
    {
        OnCollect();
    }

    // To be overridden by a child class to do whatever it needs when collected
    protected virtual void OnCollect()
    {
        if (!collected)
            collected = true;
    }
}
