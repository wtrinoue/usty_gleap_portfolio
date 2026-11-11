using System.Collections.Generic;
using UnityEngine;

// 槍本体。プレイヤーをマウス方向にダッシュさせながら、前に突き出す → 消える
// プレイヤーの移動スクリプトより後に動かして、ダッシュの速度が上書きされないようにする
[DefaultExecutionOrder(100)]
public class SpearBehaviour : MonoBehaviour, WeaponInterface
{
    private StatusToken statusToken;
    private bool enableAttack = false;

    private Vector2 direction = Vector2.right;
    private Transform owner;
    private Rigidbody2D ownerRb;

    private float thrustTime = 0.25f;
    private float startOffset = 0.3f;
    private float reach = 1.2f;
    private float dashSpeed = 18f;
    private float dashTime = 0.15f;

    private float timer = 0f;
    private bool dashFinished = false;

    // 1回の突きで同じ敵に何度もダメージが入らないようにする
    private readonly HashSet<StatusContainer> hitTargets = new();

    // 槍が伸びきるまでにかかる割合（突きの時間のうち最初の 1/3 で伸びきる）
    const float ExtendRate = 3f;

    // Core から値を受け取る
    public void Setup(Vector2 dir, Transform owner, float thrustTime, float startOffset, float reach,
                      float dashSpeed, float dashTime)
    {
        direction = dir.normalized;
        this.owner = owner;
        this.startOffset = startOffset;
        this.reach = reach;
        this.dashSpeed = dashSpeed;
        this.dashTime = dashTime;
        this.thrustTime = Mathf.Max(thrustTime, dashTime); // ダッシュ中に槍が消えないように
    }

    public void Init()
    {
        timer = 0f;
        dashFinished = false;
        hitTargets.Clear();
        ownerRb = owner != null ? owner.GetComponent<Rigidbody2D>() : null;
        UpdateSpearPosition();
    }

    public void InsertToken(StatusToken token)
    {
        statusToken = token;
    }

    public void Fire()
    {
        enableAttack = true;
    }

    void Update()
    {
        if (!enableAttack) return;

        timer += Time.deltaTime;

        // プレイヤーに Rigidbody2D が無い場合だけ、ここで直接動かす（壁はすり抜けてしまう）
        if (ownerRb == null && owner != null && timer <= dashTime)
        {
            owner.position += (Vector3)(direction * dashSpeed * Time.deltaTime);
        }

        if (timer >= thrustTime)
        {
            Destroy(gameObject);
        }
    }

    // 物理の更新ごとにプレイヤーの速度をダッシュの速度で上書きする
    void FixedUpdate()
    {
        if (!enableAttack || ownerRb == null || dashFinished) return;

        if (timer < dashTime)
        {
            SetVelocity(ownerRb, direction * dashSpeed);
        }
        else
        {
            // ダッシュ終了。滑り続けないように止める
            SetVelocity(ownerRb, Vector2.zero);
            dashFinished = true;
        }
    }

    // プレイヤーが動いたあとに、槍をプレイヤーの前へ置き直す
    void LateUpdate()
    {
        UpdateSpearPosition();
    }

    void UpdateSpearPosition()
    {
        if (owner == null) return;
        float t = thrustTime > 0f ? Mathf.Clamp01(timer / thrustTime * ExtendRate) : 1f;
        float offset = Mathf.Lerp(startOffset, reach, t);
        transform.position = (Vector2)owner.position + direction * offset;
    }

    static void SetVelocity(Rigidbody2D rb, Vector2 v)
    {
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = v;
#else
        rb.velocity = v;
#endif
    }

    // 当たったオブジェクトを検出
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enableAttack) return;

        // 突いた本人（プレイヤー）にはダメージを与えない
        if (other.CompareTag("Player")) return;
        if (owner != null && other.transform.IsChildOf(owner)) return;

        StatusContainer otherSC = other.gameObject.GetComponent<StatusContainer>();
        if (otherSC == null) return;
        if (!hitTargets.Add(otherSC)) return; // この突きで既に当てた敵

        otherSC.ApplyOneTimeToken(statusToken);
    }
}
