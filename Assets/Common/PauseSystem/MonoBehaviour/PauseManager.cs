using System.Collections.Generic;
using UnityEngine;

// 他のスクリプトのAwake/OnEnableより先にInstanceを用意するため、実行順を早める
[DefaultExecutionOrder(-1000)]
public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    private readonly HashSet<IPausable> _pausableObjects = new HashSet<IPausable>();

    // 更新ループ中に発生した登録・解除を一時的に溜めておく
    private readonly HashSet<IPausable> _pendingAdd = new HashSet<IPausable>();
    private readonly HashSet<IPausable> _pendingRemove = new HashSet<IPausable>();

    private bool _isUpdating = false;
    private bool _isPaused = false;

    public bool IsPaused => _isPaused;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        // Pauseされている時には実行しない
        if (_isPaused) return;

        // ここですべてのオブジェクトのOnCustomUpdateを回す
        _isUpdating = true;
        try
        {
            foreach (var obj in _pausableObjects)
            {
                // このフレーム中に解除されたものは呼ばない
                if (_pendingRemove.Contains(obj)) continue;

                obj.OnCustomUpdate();
            }
        }
        finally
        {
            _isUpdating = false;
            ApplyPendingChanges();
        }
    }

    public void Register(IPausable pausable)
    {
        if (pausable == null) return;

        if (_isUpdating)
        {
            // ループ中は直接HashSetを変更せず、保留リストに入れる
            _pendingRemove.Remove(pausable);
            if (!_pausableObjects.Contains(pausable))
            {
                _pendingAdd.Add(pausable);
            }
            return;
        }

        _pausableObjects.Add(pausable);
    }

    public void Unregister(IPausable pausable)
    {
        if (pausable == null) return;

        if (_isUpdating)
        {
            _pendingAdd.Remove(pausable);
            if (_pausableObjects.Contains(pausable))
            {
                _pendingRemove.Add(pausable);
            }
            return;
        }

        _pausableObjects.Remove(pausable);
    }

    public void SetPause(bool pauseState)
    {
        _isPaused = pauseState;
    }

    // 保留していた登録・解除をまとめて反映する
    private void ApplyPendingChanges()
    {
        if (_pendingRemove.Count > 0)
        {
            foreach (var obj in _pendingRemove)
            {
                _pausableObjects.Remove(obj);
            }
            _pendingRemove.Clear();
        }

        if (_pendingAdd.Count > 0)
        {
            foreach (var obj in _pendingAdd)
            {
                _pausableObjects.Add(obj);
            }
            _pendingAdd.Clear();
        }
    }
}
