using UnityEngine;

public class BladeBehaviour : MonoBehaviour
{
    private StatusToken statusToken;
    public void SetToken(StatusToken st)
    {
        statusToken = st;
    }
    // 当たったオブジェクトを検出
    private void OnTriggerEnter2D(Collider2D other)
    {
        StatusContainer otherSC = other.gameObject.GetComponent<StatusContainer>();
        if (otherSC == null) return;
        otherSC.ApplyOneTimeToken(statusToken);
    }

}
