using UnityEngine;

public class BladeBehaviour : MonoBehaviour, WeaponInterface
{
    private StatusToken statusToken;
    private bool enableAttack = false;

    public void Init() { }
    public void InsertToken(StatusToken token)
    {
        statusToken = token;
    }
    public void Fire()
    {
        enableAttack = true;
    }
    // 当たったオブジェクトを検出
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enableAttack) return;
        StatusContainer otherSC = other.gameObject.GetComponent<StatusContainer>();
        if (otherSC == null) return;
        otherSC.ApplyOneTimeToken(statusToken);
    }

}
