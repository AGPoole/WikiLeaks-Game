using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OrganisationBase : MonoBehaviour
{
    protected OrganisationData m_xMyData;
    [SerializeField]
    List<SystemBase> m_xSystems;
    [SerializeField]
    UnityEngine.UI.Text m_xSizeText;
    [SerializeField]
    UnityEngine.UI.Text m_xSavingsText;
    [SerializeField]
    UnityEngine.UI.Text m_xCostsText;
    [SerializeField]
    UnityEngine.UI.Text m_xLevelUpRequirementText;
    [SerializeField]
    UnityEngine.UI.Slider m_xSlider;

    public OrganisationData GetData() {
        if (m_xMyData == null)
        {
            SetData();
        }
            
        return m_xMyData; 
    }
    protected abstract void SetData();

    // Start is called before the first frame update
    protected virtual void Start()
    {
        SetData();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    public virtual void OnNextTurn()
    {
        UpdateSystems();
        UpdateUI();
    }

    protected virtual void UpdateSystems()
    {
        foreach (SystemBase m_xSys in m_xSystems)
        {
            m_xSys.OnNextTurn(m_xMyData.GetSize());
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
            if (fTotal > m_xMyData.GetSize())
            {
                int iToRemove = UnityEngine.Random.Range(0, m_xSystems.Count - 1);
                m_xSystems[iToRemove].LevelDown();
            }
        } while (fTotal > m_xMyData.GetSize());
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
        if (fTotal + fCheapestCost <= m_xMyData.GetSize() + 5)
        {
            xCheapest.LevelUp();
        }
    }

    protected virtual void UpdateUI()
    {
        m_xSizeText.text = m_xMyData.GetSize().ToString();
        m_xCostsText.text = m_xMyData.GetCosts().ToString("0.00");
        m_xSavingsText.text = m_xMyData.GetSavings().ToString("0.00");
        m_xLevelUpRequirementText.text = TechCompanyValues.GetTotalRequirementAtLevel(m_xMyData.GetSize()).ToString("0.00");

        m_xSlider.value = m_xMyData.GetSavings() / TechCompanyValues.GetTotalRequirementAtLevel(m_xMyData.GetSize());
    }

    public List<SystemBase> GetSystemsOfType(System.Type xType)
    {
        List<SystemBase> xItems = new List<SystemBase>();
        foreach (SystemBase sys in m_xSystems)
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
public abstract class OrganisationData
{
    [SerializeField]
    protected CountryData m_xCountryData;
    [SerializeField]
    protected int m_iSize = 10;
    [SerializeField]
    protected float m_fSavings = 0;

    public int GetSize() { return m_iSize; }
    public float GetSavings() { return m_fSavings; }

    public float GetCosts() { return GetCostsAtLevel(m_iSize); }
    public float GetLevelUpRequirement() { return GetLevelUpRequirementAtLevel(m_iSize); }

    public abstract float GetCostsAtLevel(int iLevel);
    public abstract float GetLevelUpRequirementAtLevel(int iLevel);
    public abstract OrganisationData ShallowCopy();

    public abstract void OnNextTurn();

    public void SetCountryData(CountryData xCountryData)
    {
        m_xCountryData = xCountryData;
    }
}
