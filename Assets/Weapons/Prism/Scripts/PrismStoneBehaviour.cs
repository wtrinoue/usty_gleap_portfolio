using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PrismStone：射出されて停止した後、シーン上に3つ揃うと、
// 3つの頂点を結んだ三角形の中にいる敵にダメージを与える。
// ダメージには3つ目に揃ったStoneのStatusTokenを用い、ダメージエリアも3つ目が用意する。
public class PrismStoneBehaviour : MonoBehaviour, WeaponInterface
{
    private const int RequiredStoneCount = 3;

    [Header("Move Settings")]
    [SerializeField] private float speed = 6f;      // 射出直後の速さ
    [SerializeField] private float moveTime = 0.4f; // 停止するまでの秒数（徐々に減速）

    [Header("Life Settings")]
    [SerializeField] private float idleLifeTime = 8f; // 新しいStoneが来ないまま、この秒数が経つと消滅

    [Header("Area Settings")]
    [SerializeField] private float areaDisplayTime = 0.5f; // 三角形を表示してから消えるまでの秒数
    [SerializeField] private float lineWidth = 0.05f;
    [SerializeField] private Color lineColor = Color.white;
    [SerializeField] private Color fillColor = new(1f, 1f, 1f, 0.35f); // 三角形の中（透明度のある白）
    [SerializeField] private Color activeColor = Color.green;          // 揃ったときのStoneの色
    [SerializeField] private int areaSortingOrder = -1;                 // 三角形の描画順（Stoneより奥に）

    // シーン上のStoneと、その位置を保存しておく（3つ揃って攻撃したら全て消去）
    private static readonly List<PrismStoneBehaviour> stones = new();
    private static readonly List<Vector3> stonePositions = new();
    public static int StoneCount => stones.Count;

    private StatusToken statusToken;
    private bool registered = false;
    private bool moving = false;
    private float moveTimer = 0f;
    private float idleTimer = 0f;
    private Vector3 moveDirection;

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
        // BombFragmentと同様に transform.right の方向へ射出する
        moveDirection = transform.right.normalized;
        moveTimer = 0f;
        moving = true;
    }

    void Update()
    {
        // 設置済みで3つ揃うのを待っている間、一定時間何もなければ消滅
        // （OnDestroyでstatic変数からも削除される）
        if (registered)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleLifeTime)
            {
                Destroy(gameObject);
            }
            return;
        }

        if (!moving) return;

        moveTimer += Time.deltaTime;
        float rate = Mathf.Clamp01(1f - moveTimer / moveTime); // 徐々に減速
        transform.position += moveDirection * (speed * rate * Time.deltaTime);

        if (moveTimer >= moveTime)
        {
            // 停止したら設置完了として登録
            moving = false;
            Register();
        }
    }

    private void Register()
    {
        if (registered) return;
        registered = true;

        stones.Add(this);
        stonePositions.Add(transform.position);

        // 新しいStoneが来たので、待機中のStone全員の消滅カウントをリセット
        foreach (PrismStoneBehaviour stone in stones)
        {
            if (stone != null) stone.idleTimer = 0f;
        }

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

        // 3つとも緑色に変化させる
        foreach (PrismStoneBehaviour stone in usedStones)
        {
            if (stone == null) continue;
            stone.registered = false;
            stone.SetColor(activeColor);
        }

        DealDamageInTriangle(a, b, c);

        // 三角形を少し表示してから、3つまとめて消える
        StartCoroutine(ShowAreaAndDestroy(a, b, c, usedStones));
    }

    private void SetColor(Color color)
    {
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = color;
        }
    }

    // 3つ目のStoneが三角形のエリアを用意し、中の敵にダメージを与える
    private void DealDamageInTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        GameObject area = new("PrismDamageArea");
        area.transform.SetParent(transform, false);
        area.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

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

    private IEnumerator ShowAreaAndDestroy(Vector3 a, Vector3 b, Vector3 c, List<PrismStoneBehaviour> usedStones)
    {
        GameObject visual = CreateTriangleVisual(a, b, c);

        yield return new WaitForSeconds(areaDisplayTime);

        Destroy(visual);
        foreach (PrismStoneBehaviour stone in usedStones)
        {
            if (stone != null && stone != this) Destroy(stone.gameObject);
        }
        Destroy(gameObject);
    }

    // 透明度のある白で中を満たした三角形と、その縁取りを生成する
    private GameObject CreateTriangleVisual(Vector3 a, Vector3 b, Vector3 c)
    {
        GameObject visual = new("PrismTriangle");
        Material mat = new(Shader.Find("Sprites/Default"));

        // 中の塗りつぶし
        Mesh mesh = new();
        mesh.vertices = new[] { a, b, c };
        // 頂点の並びが時計回り・反時計回りのどちらでも表示されるよう両面分の三角形を用意
        mesh.triangles = new[] { 0, 1, 2, 0, 2, 1 };
        mesh.colors = new[] { fillColor, fillColor, fillColor };
        mesh.RecalculateBounds();

        visual.AddComponent<MeshFilter>().mesh = mesh;
        MeshRenderer mr = visual.AddComponent<MeshRenderer>();
        mr.material = mat;
        mr.sortingOrder = areaSortingOrder;

        // 縁取り
        LineRenderer lr = visual.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.loop = true;
        lr.positionCount = 3;
        lr.SetPositions(new[] { a, b, c });
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.material = mat;
        lr.startColor = lineColor;
        lr.endColor = lineColor;
        lr.sortingOrder = areaSortingOrder;

        return visual;
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
