using UnityEngine;

[RequireComponent(typeof(RandomItemGenerator))]
public class ItemBoxBehaviour : MonoBehaviour
{
    public StatusContainer sc;
    private RandomItemGenerator rig;
    private ItemBoxAnimation anim;   // ★追加：アニメーション担当
    private bool isDead = false;

    void Start()
    {
        rig = GetComponent<RandomItemGenerator>();
        anim = GetComponent<ItemBoxAnimation>();   // ★追加
        if (sc == null) sc = GetComponent<StatusContainer>();

        DeadToken dt = new();
        dt.SetAction(() => { OnDead(); }); // HPが0になったときに発動する
        sc.ApplyEternalToken(dt);
    }

    // HP0のとき：アイテム生成と自分自身の破壊
    private void OnDead()
    {
        // ステータスが変化するたびに呼ばれるので、二重に実行しないようにする
        if (isDead) return;
        isDead = true;

        // ★追加：壊れている途中でぶつからないように当たり判定を消す
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        GenerateItem();

        // ★変更：アニメーションを再生し、終わってから消す
        float delay = (anim != null) ? anim.PlayBreak() : 0f;
        Destroy(gameObject, delay);
    }

    public void GenerateItem()
    {
        GameObject item = rig.GetRandom();
        if (item == null) return;
        Instantiate(item, transform.position, Quaternion.identity);
    }
}
