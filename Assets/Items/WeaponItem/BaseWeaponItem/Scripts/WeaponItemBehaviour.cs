using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class WeaponItemBehaviour : MonoBehaviour
{
    [Header("Weapon Core Setting")]
    [SerializeField] private GameObject[] weaponCores;
    private WeaponCoreHolder weaponCoreHolder;

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
        weaponCoreHolder = other.gameObject.GetComponent<WeaponCoreHolder>();
        foreach (GameObject weaponCore in weaponCores)
        {
            weaponCoreHolder.AddWeaponCore(weaponCore);
        }
        Destroy(gameObject);
    }
}
