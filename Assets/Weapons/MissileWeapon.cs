using UnityEngine;

public class MissileWeapon : WeaponBase<SystemBase>
{
    [SerializeField]
    int m_iNumTurns = 50;
    protected override bool UseInternal(SystemBase tType)
    {
        // TODO: make this effect company market share
        IDisablable xDisableSystem = tType.GetComponent<IDisablable>();
        if (xDisableSystem != null)
        {
            xDisableSystem.ForceDisable(m_iNumTurns);
            m_xOwner.UnHack();
        }
        return xDisableSystem != null;
    }
}
