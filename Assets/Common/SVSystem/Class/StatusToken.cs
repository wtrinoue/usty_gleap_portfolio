using System;
using System.Collections.Generic;

public abstract class StatusToken
{
    public abstract void ExtractStatus(in StatusVector s);

    public void Execute(in StatusVector target, StatusVector modified)
    {
        modified.Validate();
        Calculate(target, modified);
    }
    public abstract void Calculate(in StatusVector target, StatusVector modified);
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

// ステータスを複数入れるためのToken
public class PackToken : StatusToken
{
    List<StatusToken> statusTokens = new();

    public void AddStatusToken(StatusToken st)
    {
        statusTokens.Add(st);
    }

    public List<StatusToken> GetTokenList()
    {
        return statusTokens;
    }

    public void SetTokenList(List<StatusToken> statusTokens)
    {
        this.statusTokens = statusTokens;
    }

    public override void ExtractStatus(in StatusVector s)
    {
        foreach (StatusToken st in statusTokens)
        {
            st.ExtractStatus(s);
        }
    }
    public override void Calculate(in StatusVector target, StatusVector modified)
    {
        foreach (StatusToken st in statusTokens)
        {
            st.Calculate(target, modified);
        }
    }
}

public class DamageToken : StatusToken
{
    float sourceAttack = 0f;
    public override void ExtractStatus(in StatusVector s)
    {
        sourceAttack += s.Calculate(StatusCategory.Attack);
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
    private Action action;
    public void SetAction(Action action)
    {
        this.action = action;
    }
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

public class LimitHPToken : StatusToken
{
    private float maxHP;

    public override void ExtractStatus(in StatusVector s)
    {
        maxHP = s.Get(StatusCategory.HP, StatusMethod.Base);
    }

    public override void Calculate(in StatusVector target, StatusVector modified)
    {
        float currentHP = target.Get(StatusCategory.HP, StatusMethod.Base);
        if (currentHP > maxHP)
        {
            target.Set(StatusCategory.HP, StatusMethod.Base, maxHP);
        }
    }
}

public class LimitMagicByHPToken : StatusToken
{
    private float maxMagic;
    public override void ExtractStatus(in StatusVector s)
    {
        maxMagic = s.Get(StatusCategory.HP, StatusMethod.Base);
    }

    public override void Calculate(in StatusVector target, StatusVector modified)
    {
        float currentMagic = target.Get(StatusCategory.Magic, StatusMethod.Base);
        if (currentMagic > maxMagic)
        {
            target.Set(StatusCategory.Magic, StatusMethod.Base, maxMagic);
        }
    }
}

public class MagicToken : StatusToken
{
    private float magic = 0f;
    public override void ExtractStatus(in StatusVector s)
    {
        magic = s.Calculate(StatusCategory.Magic);
    }

    public override void Calculate(in StatusVector target, StatusVector modified)
    {
        target.Add(StatusCategory.Magic, StatusMethod.Base, magic);
    }
}

public class HpToMagicToken : StatusToken
{
    private float magic = 0f;
    public override void ExtractStatus(in StatusVector s)
    {
        magic = s.Calculate(StatusCategory.HP);
    }

    public override void Calculate(in StatusVector target, StatusVector modified)
    {
        target.Add(StatusCategory.Magic, StatusMethod.Base, magic);
    }
}

// 決められたカテゴリーのステータスをすべて削除するトークン
public class ResetToken : StatusToken
{
    private StatusCategory sc;
    public ResetToken(StatusCategory category)
    {
        sc = category;
    }
    public override void ExtractStatus(in StatusVector s) { }
    public override void Calculate(in StatusVector target, StatusVector modified)
    {
        target.Set(sc, StatusMethod.Base, 0f);
        target.Set(sc, StatusMethod.Add, 0f);
        target.Set(sc, StatusMethod.Multiply, 0f);
    }
}

// カテゴリーの行を別のカテゴリーに複製するトークン
public class CopyToken : StatusToken
{
    private StatusCategory sourceSC;
    private StatusCategory targetSC;
    public CopyToken(StatusCategory ssc, StatusCategory tsc)
    {
        sourceSC = ssc;
        targetSC = tsc;
    }
    public override void ExtractStatus(in StatusVector s) { }

    public override void Calculate(in StatusVector target, StatusVector modified)
    {
        float tmpBase = target.Get(sourceSC, StatusMethod.Base);
        float tmpAdd = target.Get(sourceSC, StatusMethod.Add);
        float tmpMultiply = target.Get(sourceSC, StatusMethod.Multiply);
        target.Set(targetSC, StatusMethod.Base, tmpBase);
        target.Set(targetSC, StatusMethod.Add, tmpAdd);
        target.Set(targetSC, StatusMethod.Multiply, tmpMultiply);
    }
}