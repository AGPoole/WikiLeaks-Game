using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockWeaponPerk : PerkBase
{
    [SerializeField]
    GameObject m_xWeaponPrefab;
    GameObject m_xWeaponInstance;

    public override void OnHacked()
    {
        base.OnHacked();
        m_xWeaponInstance = WeaponManager.GetWeaponManager().AddWeapon(m_xWeaponPrefab);
    }
    public override void OnUnhacked()
    {
        base.OnUnhacked();
        WeaponManager.GetWeaponManager().RemoveAndDestroyWeapon(m_xWeaponInstance);
        m_xWeaponInstance = null;
    }
}
