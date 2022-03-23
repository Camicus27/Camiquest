using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUnit : MonoBehaviour
{
    [HideInInspector]
    public GameObject enemy;
    public GameObject sprite;
    public Animator anim;

    public void Setup(GameObject enemyObj)
    {
        enemy = enemyObj;

        // Set appropriate size for the enemy
        sprite.GetComponent<Image>().sprite = enemy.GetComponent<SpriteRenderer>().sprite;
        sprite.GetComponent<Image>().SetNativeSize();

        // Set display to the correct enemy animations
        anim.runtimeAnimatorController = enemy.GetComponent<Enemy>().battlingAnim;
        anim.Play("Default");
        anim.ResetTrigger("EndAnimation");
    }

    public void ResetAnimations()
    {
        anim.SetBool("RunAway", false);
        anim.SetBool("Lose", false);
        anim.SetTrigger("EndAnimation");
    }

    /// <summary>
    /// "Hit", "Attack", or "Heal"
    /// </summary>
    public void BattleAction(string action)
    {
        anim.SetTrigger(action);
    }

    public void RunAway()
    {
        anim.SetBool("RunAway", true);
    }

    public void Death()
    {
        anim.SetBool("Lose", true);
    }

    public bool IsBeingHit()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName("Hit");
    }

    public bool IsDoneWithAnimation()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName("Default");
    }

    public bool IsDead()
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName("Dead");
    }
}
