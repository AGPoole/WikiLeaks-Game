using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finance : SystemBase
{
    [SerializeField]
    UnityEngine.UI.Text m_xSharesText;
    [SerializeField]
    TechCompany m_xOwner;
    protected override SystemValuesBase GetMyValues()
    {
        return FinanceValuesContainer.GetFinanceValues();
    }

    [SerializeField]
    int iSharesBought = 0;

    public override void OnNextTurn(int iOwnerLevel)
    {
        base.OnNextTurn(iOwnerLevel);
        m_xSharesText.text = iSharesBought.ToString();
    }

    public void BuyShare()
    {
        float fCost = m_xOwner.GetTechCompanyData().GetSize();
        if (fCost <= Manager.GetManager().GetMoney())
        {
            iSharesBought += 1;
            Manager.GetManager().ChangeMoney(-fCost);
            m_xSharesText.text = iSharesBought.ToString();
        }
    }

    public void SellShare()
    {
        if (iSharesBought > 0)
        {
            iSharesBought -= 1;
            Manager.GetManager().ChangeMoney(m_xOwner.GetTechCompanyData().GetSize());
            m_xSharesText.text = iSharesBought.ToString();
        }
    }
}
