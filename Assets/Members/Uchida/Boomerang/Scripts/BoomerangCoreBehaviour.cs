using UnityEngine;
using UnityEngine.InputSystem;

// ブーメランの「発射台」。クリックで生成され、ブーメラン本体を1つ投げたら自分は消える。
public class BoomerangCoreBehaviour : MonoBehaviour, WeaponCoreInterface
{
    [Header("ブーメランの動き")]
    [Tooltip("まっすぐ進む距離")]
    [SerializeField] float forwardDistance = 6f;

    [Tooltip("プレイヤーを通り抜けてから進む距離")]
    [SerializeField] float passThroughDistance = 3f;

    [Tooltip("飛ぶ速さ")]
    [SerializeField] float speed = 10f;

    [Tooltip("見た目の回転速度（度/秒）")]
    [SerializeField] float spinSpeed = 720f;

    [Tooltip("安全装置：この秒数たったら必ず消える")]
    [SerializeField] float maxLifetime = 8f;

    [Header("発射間隔")]
    [Tooltip("次のブーメランを投げられるまでの秒数")]
    [SerializeField] float fireInterval = 1.0f;

    [Header("生成")]
    [SerializeField] GameObject boomerangPrefab;
    public StatusContainer statusContainer;

    private PackToken packToken;

    // 全ブーメラン共通の「最後に投げた時刻」（Coreは毎回作り直されるので static で覚えておく）
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
        // 発射間隔が空いていなければ何もせず消える
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
        Vector2 origin = transform.position;
        Vector2 dir = GetMouseDirection(origin);

        // ブーメランはプレイヤーに付いて動かないよう、親なし（ワールド）に生成する
        GameObject child = Instantiate(boomerangPrefab, origin, Quaternion.identity);

        BoomerangBehaviour bb = child.GetComponent<BoomerangBehaviour>();
        bb.Setup(dir, FindOwner(), forwardDistance, passThroughDistance, speed, spinSpeed, maxLifetime);

        WeaponInterface wi = child.GetComponent<WeaponInterface>();
        wi.Init();
        wi.InsertToken(packToken);
        wi.Fire();
    }

    // 戻ってくる先（プレイヤー）を探す
    Transform FindOwner()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) return player.transform;
        return transform.parent; // 見つからなければ親。どちらも無ければ発射地点に戻る
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
