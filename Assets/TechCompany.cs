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
    [SerializeField]
    UnityEngine.UI.Text m_xMarketShareText;

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
        ((TechCompanyData)m_xMyData).UpdateShare();
    }

    protected override void UpdateUI()
    {
        base.UpdateUI();
        var xTechCompanyData = (TechCompanyData)m_xMyData;
        m_xProfitText.text = xTechCompanyData.GetProfit().ToString("0.00");
        m_xSavingsGainText.text = xTechCompanyData.GetSavingsGain().ToString("0.00");
        m_xTurnsToLevelUpText.text = xTechCompanyData.GetTimeToLevelUp().ToString("0");
        m_xMarketShareText.text = (xTechCompanyData.GetNormalisedMarketShare()*100).ToString("0.00");
    }

    protected override void SetData()
    {
        if (m_xMyData == null)
        {
            m_xMyData = new TechCompanyData();
        }
    }

    public void ChangeMarketShare(float fChange)
    {
        ((TechCompanyData)GetData()).ChangeMarketShare(fChange);
    }
}

[System.Serializable]
public class TechCompanyData : OrganisationData
{
    float m_fMarketShare = 50;

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
            fTotalProfit += GetNormalisedMarketShare()*GetTechValues().GetProfitAtLevel(m_xCountryData.GetTotalTechCompaniesSize());
            xGovernment.PayTaxes(fTotalProfit * xGovernment.GetTaxRate());
            fTotalProfit -= fTotalProfit * xGovernment.GetTaxRate();
        }
        m_fSavings += fTotalProfit;
        base.OnNextTurn();
        m_xCountryData.GetPopulationData().ContributeToHappiness(fTotalProfit);
    }

    public void UpdateShare()
    {
        // TODO: put values in data
        m_fMarketShare += RandomFromDistribution.RandomNormalDistribution(0f, GetTechValues().GetShareStdDev());
        m_fMarketShare = Mathf.Clamp(m_fMarketShare, GetTechValues().GetShareMin(), GetTechValues().GetShareMax());
    }

    public override OrganisationData ShallowCopy()
    {
        return (TechCompanyData)this.MemberwiseClone();
    }
    public float GetProfit()
    {
        return GetTechValues().GetProfitAtLevel(m_iSize);
    }
    public float GetProfitAfterTax()
    {
        GovernmentData xGovernment = m_xCountryData.GetGovernmentData();
        if (xGovernment == null)
        {
            Debug.LogError("No government");
        }
        return GetTechValues().GetProfitAtLevel(m_iSize) * (1 - xGovernment.GetTaxRate());
    }

    public float GetTimeToLevelUp()
    {
        float fSavingsGain = GetSavingsGain();
        if (fSavingsGain > 0)
        {
            return (GetValues().GetLevelUpRequirementAtLevel(m_iSize) - m_fSavings) / fSavingsGain;
        }else if (fSavingsGain != 0)
        {
            return m_fSavings / (-fSavingsGain);
        }
        return 99999999;
    }

    public float GetSavingsGain()
    {
        return GetProfitAfterTax() - GetValues().GetCostsAtLevel(m_iSize);
    }

    public override OrganisationValuesBase GetValues()
    {
        return TechCompanyValuesContainer.GetTechCompanyValues();
    }

    private TechCompanyValues GetTechValues()
    {
        return (TechCompanyValues)GetValues();
    }

    public float GetMarketShare()
    {
        return m_fMarketShare;
    }
    public float GetNormalisedMarketShare()
    {
        return m_fMarketShare/m_xCountryData.GetTotalShare();
    }

    public void ChangeMarketShare(float fChange)
    {
        // TODO: make this normalise better
        m_fMarketShare = Mathf.Clamp(m_fMarketShare + fChange, GetTechValues().GetShareMin(), GetTechValues().GetShareMax());
    }
}
