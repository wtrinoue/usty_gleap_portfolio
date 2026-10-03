using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Rendering;
[RequireComponent(typeof(SpriteMask))]
[RequireComponent(typeof(SortingGroup))]
public class EffectGenerator : MonoBehaviour
{
    public float effectInterval = 1.0f;
    public GameObject effectActorPrefab;
    private EffectAnimation ea;
    private Dictionary<EffectType, float> keyValuePairs = new Dictionary<EffectType, float>();
    private Dictionary<GameObject, float> sourceEffectDurations = new Dictionary<GameObject, float>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Reset()
    {
        GetComponent<SpriteMask>().maskSource = SpriteMask.MaskSource.SupportedRenderers;
    }
    void Start()
    {
        GameObject effectActor = Instantiate(effectActorPrefab, transform);
        ea = effectActor.GetComponent<EffectAnimation>();
        foreach (EffectType effectType in System.Enum.GetValues(typeof(EffectType)))
        {
            keyValuePairs.Add(effectType, 0.0f);
        }
    }

    public void AddEffect(GameObject source, EffectType effectType, float duration)
    {
        if (sourceEffectDurations.ContainsKey(source)) return;
        bool isFisrtEffect = RotationFinished();
        keyValuePairs[effectType] += duration;
        sourceEffectDurations[source] = duration;
        if (!isFisrtEffect) return;
        StartCoroutine(EffectRotation(effectInterval));
    }

    private bool RotationFinished()
    {
        foreach (var kvp in keyValuePairs)
        {
            if (kvp.Value > 0.0f)
            {
                return false;
            }
        }
        foreach (EffectType effectType in System.Enum.GetValues(typeof(EffectType)))
        {
            keyValuePairs[effectType] = 0f;
        }
        return true;
    }

    private void GenerateEffect(EffectType effectType)
    {
        switch (effectType)
        {
            case EffectType.Poison:
                ea.Poison();
                break;
            case EffectType.Stun:
                ea.Stun();
                break;
        }
    }

    private IEnumerator EffectRotation(float interval)
    {
        while (!RotationFinished())
        {
            foreach (EffectType effectType in System.Enum.GetValues(typeof(EffectType)))
            {
                if (keyValuePairs[effectType] <= 0.0f) continue;
                GenerateEffect(effectType);
                yield return new WaitForSeconds(interval);
                ReduceEffectDuration(interval);
                ReduceSourceEffectDuration(interval);
            }

        }
        ea.Idle();
    }

    private void ReduceEffectDuration(float interval)
    {
        foreach (EffectType et in System.Enum.GetValues(typeof(EffectType)))
        {
            keyValuePairs[et] -= interval;
        }
    }

    private void ReduceSourceEffectDuration(float interval)
    {
        foreach (var source in new List<GameObject>(sourceEffectDurations.Keys))
        {
            sourceEffectDurations[source] -= interval;
            if (sourceEffectDurations[source] <= 0.0f)
            {
                sourceEffectDurations.Remove(source);
            }
        }
    }
}

public enum EffectType
{
    Poison,
    Stun
}
