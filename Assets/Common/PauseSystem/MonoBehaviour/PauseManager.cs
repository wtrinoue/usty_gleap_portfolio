using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    private readonly HashSet<IPausable> _pausableObjects = new HashSet<IPausable>();
    private bool _isPaused = false;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        // Pauseされている時には実行しない
        if (_isPaused) return;

        // ここですべてのオブジェクトのOnCustomUpdateを回す
        foreach (var obj in _pausableObjects)
        {
            obj.OnCustomUpdate();
        }
    }

    public void Register(IPausable pausable)
    {
        if (!_pausableObjects.Contains(pausable))
        {
            _pausableObjects.Add(pausable);
            Debug.Log("登録されました！");
        }
    }

    public void Unregister(IPausable pausable)
    {
        if (_pausableObjects.Contains(pausable))
        {
            _pausableObjects.Remove(pausable);
        }
    }

    public void SetPause(bool pauseState)
    {
        _isPaused = pauseState;
    }
}
