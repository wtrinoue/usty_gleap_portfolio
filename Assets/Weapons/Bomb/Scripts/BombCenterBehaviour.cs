using System;
using UnityEngine;

public class BombCenterBehaviour : MonoBehaviour, WeaponInterface
{
    [Header("Explosion Settings")]
    [SerializeField] private GameObject projectilePrefab; // 発射するPrefab
    [SerializeField] private int spawnCount = 8;           // 個数
    [SerializeField] private float delay = 2.0f;           // 爆発までの秒数
    [SerializeField] private float spawnRadius = 0.5f;     // 生成半径（中心から少し離す用）
    private StatusToken statusToken;

    public void Init() { }
    public void InsertToken(StatusToken token)
    {
        statusToken = token;
        Invoke(nameof(Explode), delay);

        // 自分の方向を整える（丸い爆弾のアニメーションのため）
        transform.eulerAngles = new Vector3(0, 0, 0);
    }
    public void Fire() { }

    private void Explode()
    {
        if (projectilePrefab == null || spawnCount <= 0)
        {
            Destroy(gameObject);
            return;
        }

        float angleStep = 360f / spawnCount;

        for (int i = 0; i < spawnCount; i++)
        {
            float angle = angleStep * i;
            float rad = angle * Mathf.Deg2Rad;

            // 円周上の位置（2D想定 XY平面）
            Vector3 dir = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);

            Vector3 spawnPos = transform.position + dir * spawnRadius;

            // 外向きに回転
            Quaternion rotation = Quaternion.LookRotation(Vector3.forward, dir);

            GameObject bombFragment = Instantiate(projectilePrefab, spawnPos, rotation);
            WeaponInterface wi = bombFragment.GetComponent<WeaponInterface>();
            wi.Init();
            wi.InsertToken(statusToken);
            wi.Fire();
        }

        // 爆弾のアニメーションを再生
        BombBallAnimation animation = GetComponent<BombBallAnimation>();
        animation.Explosion();

        // 自分を破壊
        Invoke(nameof(DestroyMe), 0.5f);
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }
}
