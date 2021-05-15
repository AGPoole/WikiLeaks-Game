using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Add ShieldsWeaponBase
public class DisarmWeapon : WeaponBase<DefenceIcon>
{
    [Range(0, 1)]
    [SerializeField]
    float m_fProbDecrease = 0.1f;
    [SerializeField]
    int m_iDamageDecrease = 10;
    protected override bool UseInternal(DefenceIcon xIcon)
    {
        if (xIcon.HasTripWire())
        {
            xIcon.DisarmTripWire(m_fProbDecrease, m_iDamageDecrease);
            return true;
        }
        return false;
    }
}
