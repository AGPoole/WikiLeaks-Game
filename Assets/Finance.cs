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
        if(m_xSharesText!=null)
            m_xSharesText.text = iSharesBought.ToString();
    }

    public void BuyShare()
    {
        float fCost = m_xOwner.GetData().GetSize();
        if (fCost <= Manager.GetManager().GetMoney())
        {
            iSharesBought += 1;
            Manager.GetManager().ChangeMoney(-fCost);
            if (m_xSharesText != null)
                m_xSharesText.text = iSharesBought.ToString();
        }
    }

    public void SellShare()
    {
        if (iSharesBought > 0)
        {
            iSharesBought -= 1;
            Manager.GetManager().ChangeMoney(m_xOwner.GetData().GetSize());
            if (m_xSharesText != null)
                m_xSharesText.text = iSharesBought.ToString();
        }
    }
}
