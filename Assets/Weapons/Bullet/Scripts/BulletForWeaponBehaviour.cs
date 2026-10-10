using UnityEngine;

public class BulletForWeaponBehaviour : MonoBehaviour, WeaponInterface
{
    [SerializeField] private float lifeTimeSeconds = 5f; // 生存時間（秒）

    [SerializeField] private float speed = 5f;
    private Rigidbody2D rb;
    private float lifeTimer;
    private bool isDestroyed;
    private Vector2 moveDirection;
    private StatusToken statusToken;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        moveDirection = transform.right.normalized;

        // 寿命カウントをコルーチンで開始
        StartCoroutine(LifeTimeRoutine());
    }

    void Update()
    {
        if (isDestroyed) return;

        // ①毎フレームごとにTransform.rotationの方向に進む
        rb.linearVelocity = moveDirection * speed;

    }

    public void Init() { }
    public void InsertToken(StatusToken token)
    {
        statusToken = token;
    }
    public void Fire() { }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroyed) return;

        // ②Enemyのオブジェクトにぶつかったときに、ダメージを与えてから消滅
        if (collision.CompareTag("Enemy") &&
            collision.TryGetComponent<StatusContainer>(out var hasSC))
        {
            hasSC.ApplyOneTimeToken(statusToken);
        }
    }

    public void SetToken(StatusToken st)
    {
        statusToken = st;
    }

    private void DestroyBullet()
    {
        if (isDestroyed) return;
        isDestroyed = true;
        Destroy(this.gameObject);
    }

    private System.Collections.IEnumerator LifeTimeRoutine()
    {
        yield return new WaitForSeconds(lifeTimeSeconds);
        DestroyBullet();
    }
}
