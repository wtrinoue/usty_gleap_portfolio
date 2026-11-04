using System.Collections;
using UnityEngine;

public class CurlingGraveBehaviour : MonoBehaviour
{
    public StatusContainer statusContainer;
    public float targetAngle;
    public float lifeTime = 4.0f;
    private Vector3 direction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float rad = targetAngle * Mathf.Deg2Rad;
        direction = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
        StartCoroutine(LifeCountDown());
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    public void SetAngle(float angle)
    {
        targetAngle = angle;
    }

    private void Move()
    {
        transform.position += direction * 0.02f * statusContainer.GetStatus().Calculate(StatusCategory.Speed);
    }

    private void Attack(in StatusContainer sc)
    {
        DamageToken dt = new();
        dt.ExtractStatus(statusContainer.GetStatus());
        sc.ApplyOneTimeToken(dt);
    }
    // 相手から自分へのベクトルを反射面の法線として扱い、directionを反射させる
    private void Reflect(Vector3 otherPosition)
    {
        Vector3 normal = transform.position - otherPosition;
        normal.z = 0f;
        if (normal.sqrMagnitude < Mathf.Epsilon) return;
        normal.Normalize();

        // 既に相手から離れる向きに進んでいる場合は反射しない(二重反射防止)
        if (Vector3.Dot(direction, normal) >= 0f) return;

        direction = Vector3.Reflect(direction, normal);
    }

    private IEnumerator LifeCountDown()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") &&
            collision.TryGetComponent<StatusContainer>(out var hasSC))
        {
            Attack(hasSC);
            Reflect(collision.transform.position);
        }

    }
}
