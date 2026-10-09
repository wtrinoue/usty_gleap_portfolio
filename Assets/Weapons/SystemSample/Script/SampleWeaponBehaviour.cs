using TMPro;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class SampleWeaponBehaviour : MonoBehaviour, WeaponInterface
{
    private StatusToken statusToken;
    private bool prepared = false;
    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void Init() { }

    public void InsertToken(StatusToken token)
    {
        statusToken = token;
    }

    public void Fire()
    {
        // ぶつかって反応してもいいようにしている。
        prepared = true;
    }

    // 右に移動用
    private void Move()
    {
        transform.position += new Vector3(0.02f, 0f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!prepared) return;
        // 敵に当たったら発動
        if (collision.CompareTag("Enemy") &&
            collision.TryGetComponent<StatusContainer>(out var hasSC))
        {
            // 敵にTokenを埋め込む
            hasSC.ApplyOneTimeToken(statusToken);
        }
    }
}
