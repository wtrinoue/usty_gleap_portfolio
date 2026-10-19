using UnityEngine;

public class SpiritOrbBehaviour : MonoBehaviour, WeaponCoreInterface
{
    private MagicToken mt = new();
    private bool chasable = false;
    private Transform playerTransform;
    private Rigidbody2D rb;

    [Header("移動・追尾設定")]
    [Tooltip("移動のスピード（値が大きいほど速くプレイヤーを追う）")]
    public float moveSpeed = 5.0f;
    [Tooltip("維持したい理想の距離（これより遠いと近づき、近いと離れる）")]
    public float optimalDistance = 1.0f;
    [Tooltip("この距離内に入るとスピードを緩やかにする範囲")]
    public float attenuationRadius = 3.0f;

    [Header("渦巻き設定")]
    [Tooltip("渦巻きの回転の強さ")]
    public float vortexSpeed = 3.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (chasable)
        {
            ChasePlayer();
        }

    }
    public void Init() { }
    public void ExtractPlayerStatus(in StatusVector s)
    {
        mt.ExtractStatus(s);
    }
    public void Fire()
    {
        chasable = true;
    }

    private void ChasePlayer()
    {
        // プレイヤー未取得なら取得を試みる
        if (playerTransform == null)
        {
            playerTransform = PlayerManager.Instance.CurrentPlayer;
        }

        // 1. プレイヤーとの方向と距離を計算 (2D)
        Vector2 toPlayer = (Vector2)playerTransform.position - (Vector2)transform.position;
        float currentDistance = toPlayer.magnitude;

        if (currentDistance <= 0.0001f) return;

        Vector2 radialDir = toPlayer.normalized;
        // 垂直方向（渦巻き用）
        Vector2 tangentialDir = new Vector2(-radialDir.y, radialDir.x);

        // 2. 理想距離との差分をベースにした「目標スピード」の計算
        // ＋なら遠いので近づく方向、－なら近いので離れる方向の速度を作る
        float distanceDifference = currentDistance - optimalDistance;
        float targetSpeed = distanceDifference * moveSpeed;

        // 3. ある範囲（attenuationRadius）内に入ったらスピードを弱める処理
        if (currentDistance < attenuationRadius)
        {
            float attenuation = Mathf.Clamp01(currentDistance / Mathf.Max(attenuationRadius, 0.0001f));
            targetSpeed *= attenuation;
        }

        // 4. 「プレイヤーへ向かう速度」と「渦巻きの速度」を合成
        Vector2 velocity = (radialDir * targetSpeed) + (tangentialDir * vortexSpeed);

        // ★ 5. 速度（velocity）を直接代入する
        rb.linearVelocity = velocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        StatusContainer playerSC = collision.gameObject.GetComponent<StatusContainer>();
        playerSC.ApplyOneTimeToken(mt);
        Destroy(gameObject);
    }
}
