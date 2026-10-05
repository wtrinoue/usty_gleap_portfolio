using System.Collections.Generic;
using UnityEngine;

public class StatusContainer : MonoBehaviour
{
    [Header("StatusMatrix (ステータスの初期値)")]
    public StatusMatrix statusMatrix;
    public StatusVector statusVector { get; private set; }

    public ModifierContainer modifierContainer { get; } = new ModifierContainer();
    public List<StatusToken> eternalTokens = new();

    private void Awake()
    {
        statusVector = new StatusVector(statusMatrix);
        CustomToken LimitHPToken = new CustomToken((StatusVector target) =>
        {
            float maxHP = statusMatrix.Get(StatusCategory.HP, StatusMethod.Base);
            float currentHP = target.Calculate(StatusCategory.HP);
            if (currentHP > maxHP)
            {
                target.Set(StatusCategory.HP, StatusMethod.Base, maxHP);
            }
        });
        eternalTokens.Add(LimitHPToken);
    }

    private void Update()
    {
        modifierContainer.Update(Time.deltaTime);
        modifierContainer.ApplyEffect(statusVector);
        ExecuteEternalTokens();
    }

    public StatusVector GetStatus()
    {
        return statusVector.Offset(modifierContainer.CalculateBuff());
    }

    public void AddModifier(Modifier modifier)
    {
        modifierContainer.Add(modifier);
    }

    public void RemoveModifier(Modifier modifier)
    {
        modifierContainer.Remove(modifier);
    }

    public void ApplyEffects()
    {
        modifierContainer.ApplyEffect(statusVector);
    }

    public void ApplyOneTimeToken(StatusToken token)
    {
        token.Execute(statusVector, GetStatus());
    }

    public void ApplyEternalToken(StatusToken token)
    {
        eternalTokens.Add(token);
    }

    public void ExecuteEternalTokens()
    {
        foreach (StatusToken token in eternalTokens)
        {
            token.Execute(statusVector, GetStatus());
        }
    }
}

/*
現在のStatusのやり取りは関数の引数にStatusVectorを渡すことで処理をしていたが、そうではなくて、元なるステータスだけを受け取って
*/