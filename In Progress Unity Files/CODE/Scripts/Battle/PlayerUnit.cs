using UnityEngine;

public class PlayerUnit : MonoBehaviour
{
    public Animator anim;
    public Animator slashAnim;

    public void Setup()
    {
        anim.Play("Default");
        anim.ResetTrigger("EndAnimation");
    }

    public void ResetAnimations()
    {
        anim.SetBool("RunAway", false);
        anim.SetBool("Win", false);
        anim.SetBool("Lose", false);
        anim.SetTrigger("EndAnimation");
    }

    /// <summary>
    /// "Hit", Move Type, "Consume", or "Healing"
    /// </summary>
    public void BattleAction(string action)
    {
        anim.SetTrigger(action);
        if (slashAnim != null && action.Contains("_"))
            slashAnim.SetTrigger(action.Substring(7));
    }

    public void RunAway()
    {
        anim.SetBool("RunAway", true);
    }

    public void Win()
    {
        anim.SetBool("Win", true);
    }

    public void Death()
    {
        anim.SetBool("Lose", true);
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
