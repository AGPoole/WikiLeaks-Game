﻿using UnityEngine;

public class UnlockWeaponPerk : PerkBase
{
    [SerializeField]
    GameObject m_xWeaponPrefab;
    GameObject m_xWeaponInstance;

    // Whether you need to stay hacked to keep control
    [SerializeField]
    bool m_bPermanent = false;
    [SerializeField]
    int m_iCost = 0;

    protected override void Start()
    {
        base.Start();
        m_xIcon = m_xWeaponPrefab.GetComponent<IWeapon>().GetUnselectedIcon();
    }

    public override void OnClick()
    {
        base.OnClick();
        if (m_iCost > 0)
        {
            if (!m_bPermanent)
            {
                Debug.LogError("Non-permanent weapons should not cost money");
            }
            if (Manager.GetManager().GetMoney() > m_iCost && m_xWeaponInstance == null)
            {
                m_xWeaponInstance = WeaponManager.GetWeaponManager().AddWeapon(m_xWeaponPrefab, m_xSystemOwner);
                Manager.GetManager().ChangeMoney(-m_iCost);
                // TODO: remove this perk so you can only buy once. Currently this is done by checking the instance is null,
                // but it shouldn't show the perk anymore
            }
        }
    }

    public override void OnHacked()
    {
        base.OnHacked();
        if (m_iCost == 0)
        {
            m_xWeaponInstance = WeaponManager.GetWeaponManager().AddWeapon(m_xWeaponPrefab, m_xSystemOwner);
        }
    }
    public override void OnUnhacked()
    {
        base.OnUnhacked();
        if (m_xWeaponInstance != null && !m_bPermanent)
        {
            WeaponManager.GetWeaponManager().RemoveAndDestroyWeapon(m_xWeaponInstance);
            m_xWeaponInstance = null;
        }
    }
}
