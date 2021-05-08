using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealWeapon : WeaponBase<SystemBase>
{
    [SerializeField]
    float m_fStealFactor = 0.5f;
    [SerializeField]
    float m_fMarketShareChange = -10f;
    protected override bool UseInternal(SystemBase xSys)
    {
        // TODO: version for government
        // TODO: limit on smaller companies/build resistance/make income gain proportional to company size
        if(xSys.GetOwner() is TechCompany)
        {
            Manager.GetManager().ChangeMoney(GetMoney(xSys));
            TechCompany xTechCompanyOwner = xSys.GetOwner() as TechCompany;
            xTechCompanyOwner.ChangeMarketShare(m_fMarketShareChange);
        }
        return true;
    }

    public override void OnPointerOver(SystemBase tType)
    {
        MouseTextBox.AddText(string.Format("${0}", GetMoney(tType)));
    }

    int GetMoney(SystemBase xTarget)
    {
        return (int)(m_fStealFactor * xTarget.GetOwner().GetData().GetSize());
    }
}
