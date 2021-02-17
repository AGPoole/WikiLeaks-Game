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
        if (m_xCostsText != null)
        {
            m_xCostsText.text = m_xMyData.GetCosts().ToString("0.00");
        }
        if (m_xSavingsText != null)
        {
            m_xSavingsText.text = m_xMyData.GetSavings().ToString("0.00");
        }
        if (m_xLevelUpRequirementText != null)
        {
            m_xLevelUpRequirementText.text = m_xMyData.GetLevelUpRequirementAtLevel(m_xMyData.GetSize()).ToString("0.00");
        }

        m_xSlider.value = m_xMyData.GetSavings() / m_xMyData.GetLevelUpRequirementAtLevel(m_xMyData.GetSize());
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

    public float GetCostsAtLevel(int iLevel) { return GetValues().GetCostsAtLevel(iLevel); }
    public float GetLevelUpRequirementAtLevel(int iLevel) { return GetValues().GetLevelUpRequirementAtLevel(iLevel); }
    public abstract OrganisationData ShallowCopy();

    public virtual void OnNextTurn()
    {
        m_fSavings -= GetCostsAtLevel(m_iSize);
        if (m_fSavings > GetLevelUpRequirementAtLevel(m_iSize))
        {
            m_fSavings -= GetLevelUpRequirementAtLevel(m_iSize);
            m_iSize += 1;
        }
        else if (m_fSavings < 0)
        {
            m_iSize -= 1;
            m_fSavings = GetLevelUpRequirementAtLevel(m_iSize) + m_fSavings;
        }
        //TODO: deal with 0 case
        if (m_iSize < 1)
        {
            m_iSize = 1;
        }
    }

    public void SetCountryData(CountryData xCountryData)
    {
        m_xCountryData = xCountryData;
    }

    public abstract OrganisationValuesBase GetValues();
}
