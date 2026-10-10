using System.Collections;
using UnityEngine;

public class TurretGraveBehaviour : MonoBehaviour
{
    public StatusContainer statusContainer;
    public GameObject turretBulletPrefab;
    [Header("[Seconds]")]
    public float shotInterval = 2f;

    void Start()
    {
        StartCoroutine(ShotCoroutine());
    }

    private void Shot()
    {
        // ダメージトークンの作成
        DamageToken dt = new();
        dt.ExtractStatus(statusContainer.GetStatus());

        // 弾丸の発射
        GameObject turretBullet = Instantiate(
            turretBulletPrefab,
            transform.position,
            transform.rotation
        );

        // ステータストークンを入れ込む
        WeaponInterface wi = turretBullet.GetComponent<WeaponInterface>();
        wi.Init();
        wi.InsertToken(dt);
        wi.Fire();
    }
    private IEnumerator ShotCoroutine()
    {
        while (true)
        {
            Shot();
            yield return new WaitForSeconds(shotInterval);
        }
    }
}
