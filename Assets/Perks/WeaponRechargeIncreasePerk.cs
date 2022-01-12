using UnityEngine;

public class WeaponRechargeIncreasePerk : PerkBase
{
    [SerializeField]
    float m_fRechargeModifier;

    public override void OnHacked()
    {
        base.OnHacked();
        WeaponManager.GetWeaponManager().AddRechargeModifier(this);
    }

    public override void OnUnhacked()
    {
        base.OnUnhacked();
        WeaponManager.GetWeaponManager().RemoveRechargeModifier(this);
    }

    public virtual float GetModifiedValue(float fValue)
    {
        return m_fRechargeModifier * fValue;
    }
}
