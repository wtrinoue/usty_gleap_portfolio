using UnityEngine;

public class DummyEnemyBehaviour : MonoBehaviour
{
    private StatusContainer statusContainer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        statusContainer = GetComponent<StatusContainer>();
        // HPが0になるかの監視となったときの実行関数の登録
        DeadToken dt = new();
        dt.SetAction(() => { Destroy(gameObject); });// 今回は消滅する。
        statusContainer.ApplyEternalToken(dt);
    }
}
