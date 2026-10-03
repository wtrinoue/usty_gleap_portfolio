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
}
