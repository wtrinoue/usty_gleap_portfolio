using System.Security.Cryptography;
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

    private void Start()
    {
        if (_pausableCache == null) return;

        // PauseManagerに登録
        foreach (var pausable in _pausableCache)
        {
            PauseManager.Instance.Register(pausable);
            Debug.Log("登録しました！");
        }
    }

    private void OnDisable()
    {
        if (_pausableCache == null) return;

        // PauseManagerから解除
        foreach (var pausable in _pausableCache)
        {
            PauseManager.Instance.Unregister(pausable);
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