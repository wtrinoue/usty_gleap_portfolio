using UnityEngine;

// PrismCore：PrismStoneを1つ設置するだけのWeaponCore。
// Player -> PrismCore -> PrismStone（3つ揃うと三角形内の敵にダメージ）
public class PrismCoreBehaviour : MonoBehaviour, WeaponCoreInterface
{
    [Header("Spawn Settings")]
    public GameObject prismStonePrefab;
    [SerializeField] private float spawnDistance = 0.5f;

    public StatusContainer statusContainer;
    private PackToken packToken;

    public void Init()
    {
        // 通常の攻撃武器なのでDamageTokenを用いる
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
        SpawnStone();
        // 生成したらもういらないので自己破壊
        Destroy(gameObject);
    }

    private void SpawnStone()
    {
        if (prismStonePrefab == null)
        {
            Debug.LogWarning("PrismStone Prefab is not assigned!");
            return;
        }

        // 他の武器と同様に、向いている方向の逆（後ろ）に設置する
        Vector3 spawnPos = transform.position + (-transform.up) * spawnDistance;

        GameObject stone = Instantiate(prismStonePrefab, spawnPos, Quaternion.identity);

        WeaponInterface wi = stone.GetComponent<WeaponInterface>();
        wi.Init();
        wi.InsertToken(packToken);
        wi.Fire();
    }
}
