using UnityEngine;

public class BombCoreBehaviour : MonoBehaviour, WeaponCoreInterface
{
    [Header("Bomb Settings")]
    [SerializeField] private GameObject bombCenterPrefab;
    [SerializeField] private float launchForce = 10f;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnDistance = 1.0f;
    public StatusContainer statusContainer;
    private PackToken packToken;


    public void Init()
    {
        packToken = new();
        packToken.AddStatusToken(new DamageToken());
        packToken.ExtractStatus(statusContainer.GetStatus());
    }
    public void ExtractPlayerStatus(in StatusVector s)
    {
        packToken.ExtractStatus(s);
    }
    public void Fire()
    {
        SpawnAndLaunch();
    }

    private void SpawnAndLaunch()
    {
        if (bombCenterPrefab == null)
            return;

        // 向いている方向の逆（後ろ方向）
        Vector2 direction = -transform.up;

        // 指定距離だけ後方に生成
        Vector3 spawnPosition =
            transform.position + (Vector3)(direction * spawnDistance);

        GameObject bomb = Instantiate(
            bombCenterPrefab,
            spawnPosition,
            transform.rotation
        );

        bomb.GetComponent<BombCenterBehaviour>().SetToken(packToken);

        Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.AddForce(direction * launchForce, ForceMode2D.Impulse);
        }
        else
        {
            Debug.LogWarning("BombCenterPrefab に Rigidbody2D がありません");
        }
    }
}
