using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class TechCompany : MonoBehaviour
{
    [SerializeField]
    TechCompanyData m_xTechCompanyData;

    [SerializeField]
    UnityEngine.UI.Text m_xSizeText;
    [SerializeField]
    UnityEngine.UI.Text m_xSavingsText;
    [SerializeField]
    UnityEngine.UI.Text m_xProfitText;
    [SerializeField]
    UnityEngine.UI.Text m_xCostsText;
    [SerializeField]
    UnityEngine.UI.Text m_xSavingsGain;
    [SerializeField]
    UnityEngine.UI.Text m_xLevelUpRequirementText;
    [SerializeField]
    UnityEngine.UI.Text m_xSavingsGainText;
    [SerializeField]
    UnityEngine.UI.Text m_xTurnsToLevelUpText;
    [SerializeField]
    UnityEngine.UI.Slider m_xSlider;
    [SerializeField]
    List<SystemBase> m_xSystems;

    int m_iPreviousSize = 1;

    public void OnNextTurn() 
    {
        UpdateSystems();
        m_xSizeText.text = m_xTechCompanyData.GetSize().ToString();
        m_xProfitText.text = m_xTechCompanyData.GetProfit().ToString("0.00");
        m_xCostsText.text = m_xTechCompanyData.GetCosts().ToString("0.00");
        m_xSavingsText.text = m_xTechCompanyData.GetSavings().ToString("0.00");
        m_xSavingsGainText.text = m_xTechCompanyData.GetSavingsGain().ToString("0.00");
        m_xLevelUpRequirementText.text = TechCompanyValues.GetTotalRequirementAtLevel(m_xTechCompanyData.GetSize()).ToString("0.00");
        m_xTurnsToLevelUpText.text = m_xTechCompanyData.GetTimeToLevelUp().ToString("0");

        m_xSlider.value = m_xTechCompanyData.GetSavings() / TechCompanyValues.GetTotalRequirementAtLevel(m_xTechCompanyData.GetSize());

        int iSize = m_xTechCompanyData.GetSize();
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

    public void UpdateSystems()
    {
        foreach (SystemBase m_xSys in m_xSystems)
        {
            m_xSys.OnNextTurn(m_xTechCompanyData.GetSize());
        }
        // Calculate Total Cost
        float fTotal = 0;
        do
        {
            fTotal = 0;
            foreach (SystemBase m_xSys in m_xSystems)
            {
                fTotal += m_xSys.GetCurrentCost();
            }
            if (fTotal > m_xTechCompanyData.GetSize())
            {
                int iToRemove = UnityEngine.Random.Range(0, m_xSystems.Count - 1);
                m_xSystems[iToRemove].LevelDown();
            }
        } while (fTotal > m_xTechCompanyData.GetSize());
        // Calculate Cheapest
        float fCheapestCost = m_xSystems[0].GetLevelUpCost() - m_xSystems[0].GetCurrentCost();
        var xCheapest = m_xSystems[0];
        foreach (SystemBase xSys in m_xSystems)
        {
            if (xSys.GetLevelUpCost() - xSys.GetCurrentCost() < fCheapestCost)
            {
                fCheapestCost = xSys.GetLevelUpCost() - xSys.GetCurrentCost();
                xCheapest = xSys;
            }
        }
        // UpgradeCheapest, if you can
        if (fTotal + fCheapestCost <= m_xTechCompanyData.GetSize() + 5)
        {
            xCheapest.LevelUp();
        }
    }

    public TechCompanyData GetTechCompanyData()
    {
        return m_xTechCompanyData;
    }

    public List<SystemBase> GetSystemsOfType(System.Type xType)
    {
        List<SystemBase> xItems = new List<SystemBase>();
        foreach(SystemBase sys in m_xSystems)
        {
            if (sys.GetType() == xType)
            {
                xItems.Add(sys);
            }
        }
        return xItems;
    }
}

[System.Serializable]
public class TechCompanyData
{
    [SerializeField]
    int m_iSize = 1;
    [SerializeField]
    float m_fSavings = 100;

    CountryData m_xCountryData;

    public void SetCountryData(CountryData xCountryData)
    {
        m_xCountryData = xCountryData;
    }

    public void OnNextTurn()
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

    public TechCompanyData ShallowCopy()
    {
        return (TechCompanyData)this.MemberwiseClone();
    }

    public int GetSize()
    {
        return m_iSize;
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
    public float GetCosts()
    {
        return TechCompanyValues.GetLevelUpCostAtLevel(m_iSize);
    }
    
    public float GetSavings()
{
        return m_fSavings;
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
}
