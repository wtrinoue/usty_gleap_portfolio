using System;

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

public class MagicToHpToken : StatusToken
{
    private float hp = 0f;
    public override void ExtractStatus(in StatusVector s)
    {
        hp = s.Calculate(StatusCategory.Magic);
    }

    public override void Calculate(in StatusVector target, StatusVector modified)
    {
        target.Add(StatusCategory.HP, StatusMethod.Base, hp);
    }
}