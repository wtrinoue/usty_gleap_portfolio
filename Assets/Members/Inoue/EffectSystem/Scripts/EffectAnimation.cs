using UnityEngine;

public class EffectAnimation : MonoBehaviour
{
    public Animator animator;


    public void Idle()
    {
        animator.SetTrigger("IdleTrigger");
    }

    public void Poison()
    {
        animator.SetTrigger("PoisonTrigger");
    }

    public void Stun()
    {
        animator.SetTrigger("StunTrigger");
    }

    public void Speed()
    {
        animator.SetTrigger("SpeedTrigger");
    }

    public void Heal()
    {
        animator.SetTrigger("HealTrigger");
    }

    public void Power()
    {
        animator.SetTrigger("PowerTrigger");
    }
}
