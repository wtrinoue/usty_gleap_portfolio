using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class GraveItemBehaviour : MonoBehaviour
{
    [Header("Grave Setting")]
    [SerializeField] private GameObject[] graves;
    private GraveHolder graveHolder;

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

        // PlayerのGraveHolderにgraveを入れる
        graveHolder = other.gameObject.GetComponent<GraveHolder>();
        foreach (GameObject grave in graves)
        {
            graveHolder.AddGrave(grave);
        }
        Destroy(gameObject);
    }
}
