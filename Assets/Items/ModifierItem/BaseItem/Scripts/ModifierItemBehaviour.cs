using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class ModifierItemBehaviour : MonoBehaviour
{
    [Header("Effect Setting")]
    [SerializeField] private ModifierDefinition modifierDefinition;
    [SerializeField] private EffectType effectType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Reset()
    {
        // CapsuleCollider2Dを取得してisTriggerをtrueにする
        CapsuleCollider2D capsuleCollider = GetComponent<CapsuleCollider2D>();

        if (capsuleCollider != null)
        {
            capsuleCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Playerタグのときだけ処理
        if (!other.CompareTag("Player")) return;

        Debug.Log("Playerに当たりました: " + other.name);

        // 例：ステータスアクションを適用
        if (modifierDefinition == null) return;
        StatusContainer playerSC = other.gameObject.GetComponent<StatusContainer>();
        playerSC.AddModifier(new Modifier(modifierDefinition, gameObject));
        other.gameObject.GetComponent<EffectGenerator>().AddEffect(gameObject, effectType, modifierDefinition.duration);
        Destroy(gameObject);
    }
}
