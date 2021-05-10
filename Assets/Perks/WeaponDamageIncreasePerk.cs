using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamageIncreasePerk : PerkBase
{
    [SerializeField]
    int m_iDamageIncrease;

    public override void OnHacked()
    {
        base.OnHacked();
        WeaponManager.GetWeaponManager().AddDamageModifier(this);
    }

    public override void OnUnhacked()
    {
        base.OnUnhacked();
        WeaponManager.GetWeaponManager().RemoveDamageModifier(this);
    }

    public virtual float GetModifiedValue (float fValue)
    {
        return fValue+m_iDamageIncrease;
    }
}
