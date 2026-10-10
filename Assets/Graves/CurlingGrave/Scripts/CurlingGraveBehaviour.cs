using System.Collections;
using UnityEngine;

public class CurlingGraveBehaviour : MonoBehaviour
{
    public StatusContainer statusContainer;
    public float targetAngle;
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
    private IEnumerator LifeCountDown()
    {
        yield return new WaitForSeconds(4.0f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") &&
            collision.TryGetComponent<StatusContainer>(out var hasSC))
        {

            Attack(hasSC);
        }

    }
}
