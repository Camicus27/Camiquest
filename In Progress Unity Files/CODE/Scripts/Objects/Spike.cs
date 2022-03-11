using System.Collections;
using UnityEngine;

public class Spike : Collidable
{
    protected override void OnCollide()
    {
        // Disable the player movement
        GameManager.instance.StopPlayerMovement();
        // Play a feedback sound
        GameManager.instance.PlaySound("HitBySpike");
        StartCoroutine(PushPlayer());
        // Animate the damage
        GameManager.instance.player.TakeLiveDamage(5);
    }

    private IEnumerator PushPlayer()
    {
        // Push the player back
        GameManager.instance.playerObj.GetComponent<Rigidbody2D>().velocity = (GameManager.instance.playerObj.transform.position - transform.position).normalized * 1.4f;
        yield return new WaitForSeconds(.1f);
        GameManager.instance.playerObj.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(.3f);

        // Re-enable the player
        GameManager.instance.ResumePlayerMovement();
    }
}