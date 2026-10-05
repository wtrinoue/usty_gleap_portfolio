using System;
using System.Diagnostics;
using NUnit.Framework.Internal.Filters;

public abstract class StatusToken
{
    public Action action = () => { };
    public abstract void ExtractStatus(in StatusVector s);

    public void SetAction(Action a)
    {
        action = a;
    }

    public void Execute(in StatusVector target, StatusVector modified)
    {
        modified.Validate();
        Calculate(target, modified);
    }
    abstract public void Calculate(in StatusVector target, StatusVector modified);
}

// ステータスに対する自由な変更をラムダ式で登録する。
public class CustomToken : StatusToken
{
    private new readonly Action<StatusVector> action;

    public override void ExtractStatus(in StatusVector s) { }

    public CustomToken(Action<StatusVector> action)
    {
        this.action = action;
    }
    public override void Calculate(in StatusVector target, StatusVector modified)
    {
        action(target);
    }
}

public class DamageToken : StatusToken
{
    float sourceAttack = 0f;
    public override void ExtractStatus(in StatusVector s)
    {
        sourceAttack = s.Calculate(StatusCategory.Attack);
    }
    public override void Calculate(in StatusVector target, StatusVector modified)
    {
        float targetDefense = modified.Calculate(StatusCategory.Defense);
        float damage = sourceAttack - targetDefense;
        UnityEngine.Debug.Log("attack = " + sourceAttack);
        UnityEngine.Debug.Log("defence = " + targetDefense);
        UnityEngine.Debug.Log("damage = " + damage);
        if (damage < 0) { damage = 0; }
        target.Add(StatusCategory.HP, StatusMethod.Base, -damage);
    }
}

public class DeadToken : StatusToken
{
    public override void ExtractStatus(in StatusVector s) { }
    public override void Calculate(in StatusVector target, StatusVector modified)
    {
        UnityEngine.Debug.Log("ダメージ計算中！！");
        float myHP = target.Get(StatusCategory.HP, StatusMethod.Base);
        UnityEngine.Debug.Log(myHP);
        if (myHP <= 0)
        {
            action.Invoke();// ここにやられた時の処理を入れておく。
        }
    }
}