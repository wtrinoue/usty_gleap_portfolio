using UnityEngine;

public class PauseRegister : MonoBehaviour
{
    [Header("ポーズ制御したいコンポーネント")]
    [Tooltip("IPausableを実装したMonoBehaviourをアタッチしてください")]
    [SerializeField] private MonoBehaviour[] targetComponents;

    // 内部でキャッシュしておくIPausableのリスト
    private IPausable[] _pausableCache;

    private void Awake()
    {
        // キャッシュの初期化と有効性チェック
        InitializePausables();
    }

    // 有効化されるたびに登録する（SetActive(true)での再有効化にも対応）
    private void OnEnable()
    {
        RegisterAll();
    }

    // シーン開始時にOnEnableの時点でPauseManagerが未生成だった場合の保険
    // （Registerは二重登録されないので、重複して呼んでも問題ない）
    private void Start()
    {
        RegisterAll();
    }

    private void OnDisable()
    {
        if (_pausableCache == null) return;

        // シーン終了時などでPauseManagerが先に破棄されている場合は何もしない
        var manager = PauseManager.Instance;
        if (manager == null) return;

        // PauseManagerから解除
        foreach (var pausable in _pausableCache)
        {
            manager.Unregister(pausable);
        }
    }

    private void RegisterAll()
    {
        if (_pausableCache == null) return;

        var manager = PauseManager.Instance;
        if (manager == null) return;

        // PauseManagerに登録
        foreach (var pausable in _pausableCache)
        {
            manager.Register(pausable);
        }
    }

    private void InitializePausables()
    {
        if (targetComponents == null || targetComponents.Length == 0) return;

        var validList = new System.Collections.Generic.List<IPausable>();

        foreach (var comp in targetComponents)
        {
            if (comp == null) continue;

            // コンポーネントが IPausable を実装しているか判定
            if (comp is IPausable pausable)
            {
                validList.Add(pausable);
            }
            else
            {
                // 実装していない場合は明確なエラーをコンソールに出力
                Debug.LogError($"[PauseRegister] 割り当てられたコンポーネント '{comp.GetType().Name}' ({comp.gameObject.name}) は IPausable インターフェースを実装していません！", comp);
            }
        }

        _pausableCache = validList.ToArray();
    }

    // --- エディター上での即時チェック (OnValidate) ---
    private void OnValidate()
    {
        if (targetComponents == null) return;

        foreach (var comp in targetComponents)
        {
            if (comp == null) continue;

            // インスペクターで設定された瞬間にIPausableかチェックする
            if (!(comp is IPausable))
            {
                Debug.LogError($"[PauseRegister] 警告: '{comp.GetType().Name}' ({comp.gameObject.name}) は IPausable を実装していません。ポーズ機能が正しく働きません。", comp);
            }
        }
    }
}
