using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileWeapon : WeaponBase<SystemBase>
{
    [SerializeField]
    int m_iNumTurns = 50;
    protected override bool UseInternal(SystemBase tType)
    {
        IDisablable xDisableSystem = tType.GetComponent<IDisablable>();
        if (xDisableSystem != null)
        {
            xDisableSystem.ForceDisable(m_iNumTurns);
        }
        return xDisableSystem != null;
    }
}
