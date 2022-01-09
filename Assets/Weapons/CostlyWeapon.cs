using System.Collections.Generic;
using UnityEngine;

// TODO: Add ShieldsWeaponBase
public class CostlyWeapon : WeaponBase<DefenceIcon>
{
    [System.Serializable]
    class DamageData
    {
        public int m_iRequiredMoney;
        public int m_iDamage;
    }

    [SerializeField]
    List<DamageData> m_xDamageData;
    protected override bool UseInternal(DefenceIcon xIcon)
    {
        return xIcon.Attack(WeaponManager.GetWeaponManager().GetModifiedDamage(CalculateDamage()), IsDetectable());
    }

    int CalculateDamage()
    {
        int iMoney = Manager.GetManager().GetMoney();

        if (m_xDamageData.Count == 0)
        {
            Debug.LogError("No damage data");
            return 0;
        }

        int iIndex = 0;
        while (iIndex + 1 < m_xDamageData.Count && m_xDamageData[iIndex + 1].m_iRequiredMoney <= iMoney)
        {
            iIndex += 1;
        }
        return m_xDamageData[iIndex].m_iDamage;

    }
}
