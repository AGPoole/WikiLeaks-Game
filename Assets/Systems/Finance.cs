using UnityEngine;

public class Finance : SystemBase
{
    [SerializeField]
    UnityEngine.UI.Text m_xSharesText;
    public override SystemValuesBase GetMyValues()
    {
        return FinanceValuesContainer.GetFinanceValues();
    }

    [SerializeField]
    int iSharesBought = 0;

    public override void OnNextTurn(int iOwnerLevel)
    {
        base.OnNextTurn(iOwnerLevel);
        if (m_xSharesText != null)
            m_xSharesText.text = iSharesBought.ToString();
    }

    public void BuyShare()
    {
        int iCost = m_xOwner.GetData().GetSize();
        if (iCost <= Manager.GetManager().GetMoney())
        {
            iSharesBought += 1;
            Manager.GetManager().ChangeMoney(-iCost);
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
