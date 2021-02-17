using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class TechCompany : OrganisationBase
{
    [SerializeField]
    UnityEngine.UI.Text m_xProfitText;
    [SerializeField]
    UnityEngine.UI.Text m_xSavingsGain;
    [SerializeField]
    UnityEngine.UI.Text m_xSavingsGainText;
    [SerializeField]
    UnityEngine.UI.Text m_xTurnsToLevelUpText;

    int m_iPreviousSize = 1;

    public override void OnNextTurn() 
    {
        base.OnNextTurn();
        var xTechCompanyData = (TechCompanyData)m_xMyData;

        int iSize = xTechCompanyData.GetSize();
        if (iSize > m_iPreviousSize)
        {
            NotificationSystem.AddNotification(string.Format("Tech company has grown to level {0}", iSize.ToString()));
        }
        else if(iSize < m_iPreviousSize)
        {
            NotificationSystem.AddNotification(string.Format("Tech company has shrunk to level {0}", iSize.ToString()));
        }
        m_iPreviousSize = iSize;
    }

    protected override void UpdateUI()
    {
        base.UpdateUI();
        var xTechCompanyData = (TechCompanyData)m_xMyData;
        m_xProfitText.text = xTechCompanyData.GetProfit().ToString("0.00");
        m_xSavingsGainText.text = xTechCompanyData.GetSavingsGain().ToString("0.00");
        m_xTurnsToLevelUpText.text = xTechCompanyData.GetTimeToLevelUp().ToString("0");
    }

    protected override void SetData()
    {
        if (m_xMyData == null)
        {
            m_xMyData = new TechCompanyData();
        }
    }
}

[System.Serializable]
public class TechCompanyData : OrganisationData
{
    public override void OnNextTurn()
    {
        GovernmentData xGovernment = m_xCountryData.GetGovernmentData();
        if(xGovernment == null)
        {
            Debug.LogError("No government");
        }
        float fTotalProfit = 0;
        if (m_iSize > 0.0f)
        {
            fTotalProfit += TechCompanyValues.GetProfitAtLevel(m_iSize);
            xGovernment.PayTaxes(fTotalProfit * xGovernment.GetTaxRate());
            fTotalProfit -= fTotalProfit * xGovernment.GetTaxRate();
        }
        m_fSavings += fTotalProfit;
        m_fSavings -= TechCompanyValues.GetLevelUpCostAtLevel(m_iSize);
        if (m_fSavings > TechCompanyValues.GetTotalRequirementAtLevel(m_iSize))
        {
            m_fSavings -= TechCompanyValues.GetTotalRequirementAtLevel(m_iSize);
            m_iSize += 1;
        }
        else if(m_fSavings<0)
        {
            m_iSize -= 1;
            m_fSavings = TechCompanyValues.GetTotalRequirementAtLevel(m_iSize) + m_fSavings;
        }
        //TODO: deal with 0 case
        if (m_iSize < 1)
        {
            m_iSize = 1;
        }
        m_xCountryData.GetPopulationData().ContributeToHappiness(fTotalProfit);
    }

    public override OrganisationData ShallowCopy()
    {
        return (TechCompanyData)this.MemberwiseClone();
    }
    public float GetProfit()
    {
        return TechCompanyValues.GetProfitAtLevel(m_iSize);
    }
    public float GetProfitAfterTax()
    {
        GovernmentData xGovernment = m_xCountryData.GetGovernmentData();
        if (xGovernment == null)
        {
            Debug.LogError("No government");
        }
        return TechCompanyValues.GetProfitAtLevel(m_iSize) * (1 - xGovernment.GetTaxRate());
    }
    public override float GetCostsAtLevel(int iLevel)
    {
        return TechCompanyValues.GetLevelUpCostAtLevel(iLevel);
    }

    public float GetTimeToLevelUp()
    {
        float fSavingsGain = GetSavingsGain();
        if (fSavingsGain > 0)
        {
            return (TechCompanyValues.GetTotalRequirementAtLevel(m_iSize) - m_fSavings) / fSavingsGain;
        }else if (fSavingsGain != 0)
        {
            return m_fSavings / (-fSavingsGain);
        }
        return 99999999;
    }

    public float GetSavingsGain()
    {
        return GetProfitAfterTax() - TechCompanyValues.GetLevelUpCostAtLevel(m_iSize);
    }

    public override float GetLevelUpRequirementAtLevel(int iLevel)
    {
        return TechCompanyValues.GetLevelUpCostAtLevel(iLevel);
    }
}
