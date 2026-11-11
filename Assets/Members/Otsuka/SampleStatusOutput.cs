using UnityEngine;
using System.Collections.Generic;
public class SampleStatusOutput : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // HPバーに関わる関数群
    public float GetMaxHp()
    {
        return 10.0f;
    }

    public float GetCurrentHp()
    {
        return 10.0f * (Mathf.Sin(Time.time) + 1.0f) / 2.0f;
    }

    // MPバーに関わる関数群
    public float GetMaxMp()
    {
        return 10.0f;
    }

    public float GetCurrentMp()
    {
        return 10.0f * (Mathf.Sin(Time.time) + 1.0f) / 2.0f;
    }

    // 武器選択バー
    public WeaponType GetSelectedWeaponType()
    {
        return (WeaponType)(Mathf.FloorToInt(Time.time) % 3);
    }

    public List<WeaponType> GetAvailableWeaponTypes()
    {
        return new List<WeaponType> { WeaponType.Blade, WeaponType.Bomb, WeaponType.Bow };
    }


    public float GetMaxCoolTime(WeaponType weaponType)
    {
        return 10.0f;
    }

    public float GetCurrentCoolTime(WeaponType weaponType)
    {
        return 10.0f * (Mathf.Sin(Time.time) + 1.0f) / 2.0f;
    }

    public float GetMaxUltimatelyCoolTime(WeaponType weaponType)
    {
        return 10.0f;
    }

    public float GetCurrentUltimatelyCoolTime(WeaponType weaponType)
    {
        return 10.0f * (Mathf.Sin(Time.time) + 1.0f) / 2.0f;
    }

    // 墓石選択バー
    public GraveType GetSelectedGraveType()
    {
        return (GraveType)(Mathf.FloorToInt(Time.time) % 5);
    }

    public int GetAvailableGraveCount(GraveType graveType)
    {
        return Mathf.FloorToInt(Time.time) % 4;
    }
}

public enum WeaponType
{
    Blade,
    Bomb,
    Bow,
    Naginata,
    Spear,
    Boomerang
}

public enum GraveType
{
    Poison,
    PowerBuff,
    Plazma,
    Turret,
    Curling
}