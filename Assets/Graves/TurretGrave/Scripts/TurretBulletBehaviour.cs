using System.Collections;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class TurretBulletBehaviour : MonoBehaviour, WeaponInterface
{
    public float speed;
    public float lifeTime;
    private float angle = 0f;
    private Vector3 direction;
    private StatusToken statusToken;
    private bool prepared = false;
    public void Init()
    {
        angle = Random.Range(0.0f, 360.0f);
        float rad = angle * Mathf.Deg2Rad;
        direction = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
    }
    public void InsertToken(StatusToken token)
    {
        statusToken = token;
    }
    public void Fire()
    {
        prepared = true;
        StartCoroutine(LifeCountDown());
    }
    void Update()
    {
        if (!prepared) return;
        Move();
    }

    private void Move()
    {
        transform.position += direction * speed * 0.02f;
    }
    private IEnumerator LifeCountDown()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!prepared) return;
        if (collision.CompareTag("Enemy") &&
            collision.TryGetComponent<StatusContainer>(out var hasSC))
        {
            hasSC.ApplyOneTimeToken(statusToken);
        }
    }
}
