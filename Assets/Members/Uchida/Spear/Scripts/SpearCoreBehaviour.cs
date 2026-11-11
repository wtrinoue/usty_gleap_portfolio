using UnityEngine;
using UnityEngine.InputSystem;

// 槍の「発射台」。クリックで生成され、槍本体を1本出したら自分は消える。
public class SpearCoreBehaviour : MonoBehaviour, WeaponCoreInterface
{
    [Header("突き")]
    [Tooltip("槍が出ている時間（秒）。ダッシュ時間より短くはなりません")]
    [SerializeField] float thrustTime = 0.25f;

    [Tooltip("突き始めの位置（プレイヤーの中心から槍の中心までの距離）")]
    [SerializeField] float startOffset = 0.3f;

    [Tooltip("いちばん伸びたときの位置（プレイヤーの中心から槍の中心までの距離）")]
    [SerializeField] float reach = 1.2f;

    [Tooltip("槍の画像の向きの補正（度）。右向きの画像なら0、上向きの画像なら-90")]
    [SerializeField] float spriteAngleOffset = 0f;

    [Header("ダッシュ（プレイヤーをはじく）")]
    [Tooltip("ダッシュの速さ")]
    [SerializeField] float dashSpeed = 18f;

    [Tooltip("ダッシュする時間（秒）。進む距離は およそ 速さ × 時間")]
    [SerializeField] float dashTime = 0.15f;

    [Header("攻撃間隔")]
    [Tooltip("次に突けるまでの秒数")]
    [SerializeField] float fireInterval = 0.6f;

    [Header("生成")]
    [SerializeField] GameObject spearPrefab;
    public StatusContainer statusContainer;

    private PackToken packToken;

    // 全槍共通の「最後に突いた時刻」（Coreは毎回作り直されるので static で覚えておく）
    private static float lastFireTime = -999f;

    // エディタで再生し直したときにリセットする
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatic()
    {
        lastFireTime = -999f;
    }

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
        // 攻撃間隔が空いていなければ何もせず消える
        if (Time.time - lastFireTime < fireInterval)
        {
            Destroy(gameObject);
            return;
        }
        lastFireTime = Time.time;

        Spawn();
        Destroy(gameObject);
    }

    void Spawn()
    {
        Transform owner = FindOwner();
        Vector2 origin = owner != null ? (Vector2)owner.position : (Vector2)transform.position;
        Vector2 dir = GetMouseDirection(origin);

        // 槍の向き（マウス方向）に回転させて生成する
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + spriteAngleOffset;
        GameObject child = Instantiate(spearPrefab, origin, Quaternion.Euler(0f, 0f, angle));

        SpearBehaviour sb = child.GetComponent<SpearBehaviour>();
        sb.Setup(dir, owner, thrustTime, startOffset, reach, dashSpeed, dashTime);

        WeaponInterface wi = child.GetComponent<WeaponInterface>();
        wi.Init();
        wi.InsertToken(packToken);
        wi.Fire();
    }

    // はじく対象（プレイヤー）を探す
    Transform FindOwner()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) return player.transform;
        return transform.parent;
    }

    // マウスカーソルの方向を求める
    Vector2 GetMouseDirection(Vector2 origin)
    {
        Camera cam = Camera.main;
        if (cam == null || Mouse.current == null) return Vector2.right;

        Vector3 screenPos = Mouse.current.position.ReadValue();
        screenPos.z = -cam.transform.position.z;
        Vector2 mouseWorld = cam.ScreenToWorldPoint(screenPos);

        Vector2 dir = mouseWorld - origin;
        if (dir.sqrMagnitude < 0.0001f) return Vector2.right;
        return dir.normalized;
    }
}
