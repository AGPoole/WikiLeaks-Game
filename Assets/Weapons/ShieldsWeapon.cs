using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldsWeapon : WeaponBase<DefenceIcon>
{
    [SerializeField]
    int m_iDamage = 10;
    protected override bool UseInternal(DefenceIcon xIcon)
    {
        return xIcon.Attack(WeaponManager.GetWeaponManager().GetModifiedDamage(m_iDamage), IsDetectable());
    }

    public override string GetDescription()
    {
        return string.Format("Damage: {0}\n", m_iDamage) + base.GetDescription();
    }
}
