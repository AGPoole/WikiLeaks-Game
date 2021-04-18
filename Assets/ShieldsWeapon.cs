using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldsWeapon : WeaponBase<DefenceIcon>
{
    [SerializeField]
    int m_iDamage = 10;
    protected override bool UseInternal(DefenceIcon xIcon)
    {
        return xIcon.Attack(m_iDamage);
    }
}
