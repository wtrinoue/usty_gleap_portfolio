using UnityEngine;

public class BladeBehaviour : MonoBehaviour
{
    [SerializeField] private StatusContainer statusContainer;
    // 当たったオブジェクトを検出
    private void OnTriggerEnter2D(Collider2D other)
    {
        StatusContainer otherSC = other.gameObject.GetComponent<StatusContainer>();
        if (otherSC == null) return;
        DamageToken dt = new();
        dt.ExtractStatus(statusContainer.GetStatus());
        otherSC.ApplyOneTimeToken(dt);
    }

}
