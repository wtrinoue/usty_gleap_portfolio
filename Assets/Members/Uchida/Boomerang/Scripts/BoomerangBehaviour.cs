using System.Collections.Generic;
using UnityEngine;

// ブーメラン本体。 行く → 戻る → プレイヤーを通り抜ける → 消える
public class BoomerangBehaviour : MonoBehaviour, WeaponInterface
{
    enum Phase { Outgoing, Returning, PassThrough }

    private StatusToken statusToken;
    private bool enableAttack = false;

    private Phase phase = Phase.Outgoing;
    private Vector2 direction = Vector2.right;
    private Transform owner;
    private Vector2 startPos;

    private float forwardDistance = 6f;
    private float passThroughDistance = 3f;
    private float speed = 10f;
    private float spinSpeed = 720f;
    private float maxLifetime = 8f;

    private float traveled = 0f;    // 今のフェーズで進んだ距離
    private float returnTime = 0f;  // 戻り始めてからの時間
    private float life = 0f;

    // 「行き」「帰り」それぞれで、すでに当たった敵（同じ敵に何回も当たらないようにする）
    private readonly HashSet<StatusContainer> hitTargets = new();

    // 折り返し前に減速、戻りで加速する（見た目をブーメランっぽくするため）
    const float MinSpeedRate = 0.3f;
    const float ReturnAccelTime = 0.4f;

    // Core から値を受け取る
    public void Setup(Vector2 dir, Transform owner, float forwardDistance, float passThroughDistance,
                      float speed, float spinSpeed, float maxLifetime)
    {
        direction = dir.normalized;
        this.owner = owner;
        this.forwardDistance = forwardDistance;
        this.passThroughDistance = passThroughDistance;
        this.speed = speed;
        this.spinSpeed = spinSpeed;
        this.maxLifetime = maxLifetime;
    }

    public void Init()
    {
        startPos = transform.position;
        phase = Phase.Outgoing;
        traveled = 0f;
        life = 0f;
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
        float dt = Time.deltaTime;
        life += dt;
        if (life >= maxLifetime)
        {
            Destroy(gameObject);
            return;
        }

        // 見た目をくるくる回す
        transform.Rotate(0f, 0f, spinSpeed * dt);

        Vector2 pos = transform.position;

        switch (phase)
        {
            case Phase.Outgoing:
            {
                // 進むほど遅くなる
                float rate = Mathf.Lerp(1f, MinSpeedRate, traveled / forwardDistance);
                float step = speed * rate * dt;
                pos += direction * step;
                traveled += step;

                if (traveled >= forwardDistance)
                {
                    phase = Phase.Returning;
                    returnTime = 0f;
                    hitTargets.Clear(); // 帰りでもう一度当たれるようにする
                }
                break;
            }

            case Phase.Returning:
            {
                returnTime += dt;
                float rate = Mathf.Lerp(MinSpeedRate, 1f, returnTime / ReturnAccelTime);
                float step = speed * rate * dt;

                Vector2 target = owner != null ? (Vector2)owner.position : startPos;
                Vector2 toTarget = target - pos;

                if (toTarget.magnitude <= step)
                {
                    // プレイヤーに到達 → そのままの向きで通り抜けへ
                    if (toTarget.sqrMagnitude > 0.0001f) direction = toTarget.normalized;
                    pos = target;
                    phase = Phase.PassThrough;
                    traveled = 0f;
                }
                else
                {
                    direction = toTarget.normalized; // プレイヤーを追いかける
                    pos += direction * step;
                }
                break;
            }

            case Phase.PassThrough:
            {
                float step = speed * dt;
                pos += direction * step;
                traveled += step;

                if (traveled >= passThroughDistance)
                {
                    Destroy(gameObject);
                    return;
                }
                break;
            }
        }

        transform.position = pos;
    }

    // 当たったオブジェクトを検出
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enableAttack) return;

        // 敵にだけダメージを与える
        if (!other.CompareTag("Enemy")) return;

        StatusContainer otherSC = other.gameObject.GetComponent<StatusContainer>();
        if (otherSC == null) return;

        // この往路（または復路）ですでに当たっていたら何もしない
        if (!hitTargets.Add(otherSC)) return;

        Debug.Log($"ブーメランヒット: {other.name}");
        otherSC.ApplyOneTimeToken(statusToken);
    }
}
