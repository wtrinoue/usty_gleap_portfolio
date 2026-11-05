using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PrismStone：シーン上に3つ揃うと、3つの頂点を結んだ三角形の中にいる敵にダメージを与える。
// ダメージには3つ目に設置されたStoneのStatusTokenを用い、ダメージエリアも3つ目が用意する。
public class PrismStoneBehaviour : MonoBehaviour, WeaponInterface
{
    private const int RequiredStoneCount = 3;

    [Header("Area Settings")]
    [SerializeField] private float areaDisplayTime = 0.3f; // 三角形を表示してから消えるまでの秒数
    [SerializeField] private float lineWidth = 0.05f;
    [SerializeField] private Color lineColor = new Color(0.6f, 0.9f, 1f, 1f);

    // シーン上のStoneと、その位置を保存しておく（3つ揃って攻撃したら全て消去）
    private static readonly List<PrismStoneBehaviour> stones = new();
    private static readonly List<Vector3> stonePositions = new();
    public static int StoneCount => stones.Count;

    private StatusToken statusToken;
    private bool registered = false;

    // Domain Reloadを無効にしている場合でもstaticが残らないようにする
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        stones.Clear();
        stonePositions.Clear();
    }

    public void Init() { }

    public void InsertToken(StatusToken token)
    {
        statusToken = token;
    }

    public void Fire()
    {
        Register();
    }

    private void Register()
    {
        if (registered) return;
        registered = true;

        stones.Add(this);
        stonePositions.Add(transform.position);

        // 3つ目のStoneが攻撃を担当する
        if (stones.Count >= RequiredStoneCount)
        {
            TriggerPrism();
        }
    }

    private void TriggerPrism()
    {
        // 今回の3つを取り出し、staticは全て消去（次の3つは新しく数え直す）
        List<PrismStoneBehaviour> usedStones = new(stones);
        Vector3 a = stonePositions[0];
        Vector3 b = stonePositions[1];
        Vector3 c = stonePositions[2];
        stones.Clear();
        stonePositions.Clear();

        DealDamageInTriangle(a, b, c);

        // 3つ目（自分）以外のStoneはすぐに消す
        foreach (PrismStoneBehaviour stone in usedStones)
        {
            if (stone != null && stone != this)
            {
                stone.registered = false;
                Destroy(stone.gameObject);
            }
        }

        // 自分は三角形を少し表示してから消える
        StartCoroutine(ShowAreaAndDestroy(a, b, c));
    }

    // 3つ目のStoneが三角形のエリアを用意し、中の敵にダメージを与える
    private void DealDamageInTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        GameObject area = new("PrismDamageArea");
        area.transform.SetParent(transform, false);
        area.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        area.transform.localScale = Vector3.one;

        PolygonCollider2D poly = area.AddComponent<PolygonCollider2D>();
        poly.isTrigger = true;
        poly.points = new Vector2[]
        {
            area.transform.InverseTransformPoint(a),
            area.transform.InverseTransformPoint(b),
            area.transform.InverseTransformPoint(c),
        };
        Physics2D.SyncTransforms();

        ContactFilter2D filter = ContactFilter2D.noFilter;
        filter.useTriggers = true;

        List<Collider2D> hits = new();
        poly.Overlap(filter, hits);

        // 1体の敵が複数のColliderを持っていても1回だけダメージを与える
        HashSet<StatusContainer> damaged = new();
        foreach (Collider2D hit in hits)
        {
            if (hit == null) continue;
            if (!hit.CompareTag("Enemy")) continue;
            if (!hit.TryGetComponent<StatusContainer>(out var sc)) continue;
            if (!damaged.Add(sc)) continue;

            sc.ApplyOneTimeToken(statusToken);
        }

        Destroy(area);
    }

    private IEnumerator ShowAreaAndDestroy(Vector3 a, Vector3 b, Vector3 c)
    {
        LineRenderer lr = gameObject.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.loop = true;
        lr.positionCount = 3;
        lr.SetPositions(new[] { a, b, c });
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = lineColor;
        lr.endColor = lineColor;

        yield return new WaitForSeconds(areaDisplayTime);

        registered = false;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // 3つ揃う前に外部から破壊された場合（シーン遷移など）はリストから外す
        if (!registered) return;
        int index = stones.IndexOf(this);
        if (index >= 0)
        {
            stones.RemoveAt(index);
            stonePositions.RemoveAt(index);
        }
    }
}
