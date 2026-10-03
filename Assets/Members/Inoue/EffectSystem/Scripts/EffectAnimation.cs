using UnityEngine;

public class EffectAnimation : MonoBehaviour
{
    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Stun();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResetAnimation()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isPoison", false);
        animator.SetBool("isStun", false);
    }

    public void Idle()
    {
        ResetAnimation();
        animator.SetBool("isIdle", true);
    }

    public void Poison()
    {
        ResetAnimation();
        animator.SetBool("isPoison", true);
    }

    public void Stun()
    {
        ResetAnimation();
        animator.SetBool("isStun", true);
    }
}
