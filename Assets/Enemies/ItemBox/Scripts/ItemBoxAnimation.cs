using UnityEngine;

// 木箱の見た目（アニメーション）を担当するスクリプト
[RequireComponent(typeof(Animator))]
public class ItemBoxAnimation : MonoBehaviour
{
    [Tooltip("壊れるアニメーションの長さ（秒）。4コマ÷12fps = 約0.33秒")]
    public float breakDuration = 0.4f;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // 壊れるアニメーションを再生して、その長さ（秒）を返す
    public float PlayBreak()
    {
        animator.SetTrigger("Break"); // Animatorの「Break」トリガーを引く
        return breakDuration;
    }
}
