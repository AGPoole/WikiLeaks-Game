using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField]
    int m_iXPosInGrid = 0;
    [SerializeField]
    int m_iYPosInGrid = 0;

    Country m_xCountry;

    [SerializeField]
    int m_iCapacity = 6;

    // TODO: move to another place and make accessible in editor
    int m_iNewBuildingCost=1;

    public OrganisationData GetData() {
        if (m_xMyData == null)
        {
            SetData();
        }
            
        return m_xMyData; 
    }
    protected abstract void SetData();

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        SetData();
        foreach (var xSys in m_xSystems)
        {
            xSys.SetOwner(this);
        }
        m_iCapacity = UnityEngine.Random.Range(1, 10);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        CorrectPosition();
    }

    #if (UNITY_EDITOR)
    [ContextMenu("Correct Position")]
    #endif
    public void CorrectPosition()
    {
        transform.position = Manager.GetManager().GetPositionFromGridCoords(m_iXPosInGrid, m_iYPosInGrid);
    }

    bool m_bBlocked = false;
    bool m_bCanAffordNewSystem = false;
    public virtual void OnNextTurn()
    {
        UpdateSystems();
        UpdateUI();
    }

    int m_iNextSystemUpgradeTime = 0;
    const int iTIME_BETWEEN_UPGRADES = 10;
    protected virtual void UpdateSystems()
    {
        if (m_xSystems.Count < m_iCapacity)
        {
            List<(int, int)> xPossiblePositions = new List<(int, int)>();
            GetAdjacentEmptySystems(ref xPossiblePositions);

            m_bBlocked = xPossiblePositions.Count == 0;
            if (!m_bBlocked)
            {
                (int iX, int iY) = xPossiblePositions[UnityEngine.Random.Range(0, xPossiblePositions.Count - 1)];

                AddNewSystem(iX, iY);
            }
        }
        if (m_xSystems.Count == 0)
        {
            // TODO: destroy this if it's at 0 size
            return;
        }
        foreach (SystemBase m_xSys in m_xSystems)
        {
            m_xSys.OnNextTurn(m_xMyData.GetSize());
        }
        // Calculate Total Cost
        float fTotal = 0;

        do
        {
            // TODO: use deterministic process
            fTotal = 0;
            foreach (SystemBase m_xSys in m_xSystems)
            {
                fTotal += m_xSys.GetCurrentCost();
            }
            if (fTotal > m_xMyData.GetSize())
            {
                int iToRemove = UnityEngine.Random.Range(0, m_xSystems.Count);
                m_xSystems[iToRemove].LevelDown();
            }
        } while (fTotal > m_xMyData.GetSize());

        // Level up systems here
        if (Manager.GetTurnNumber() > m_iNextSystemUpgradeTime)
        {
            List<SystemBase> xAffordableSystems = new List<SystemBase>();

            // prioritise level zeros
            foreach (SystemBase xSys in m_xSystems)
            {
                float fCost = xSys.GetLevelUpCost() - xSys.GetCurrentCost();
                if (xSys.GetLevel()==0 && fTotal + fCost <= m_xMyData.GetSize() + 5)
                {
                    xAffordableSystems.Add(xSys);
                }
            }
            if (xAffordableSystems.Count == 0)
            {
                foreach (SystemBase xSys in m_xSystems)
                {
                    float fCost = xSys.GetLevelUpCost() - xSys.GetCurrentCost();
                    if (fTotal + fCost <= m_xMyData.GetSize() + 5)
                    {
                        xAffordableSystems.Add(xSys);
                    }
                }
            }
            // TODO: deterministic process
            if (xAffordableSystems.Count > 0)
            {
                xAffordableSystems[UnityEngine.Random.Range(0, xAffordableSystems.Count)].LevelUp();
            }
            m_iNextSystemUpgradeTime = Manager.GetTurnNumber() + iTIME_BETWEEN_UPGRADES;
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

    public Country GetCountry()
    {
        if (m_xCountry == null)
        {
            m_xCountry = transform.parent.gameObject.GetComponent<Country>();
        }
        return m_xCountry;
    }

    public void AddNewSystem(int iXPos, int iYPos)
    {
        GameObject xSystemPrefab = null;
        int iCounter = 0;
        const int iCOUNTER_MAX = 10;
        // TODO: do this better
        while (xSystemPrefab == null)
        {
            xSystemPrefab  = Manager.GetManager().GetRandomSystemPrefab();
            if (!xSystemPrefab.GetComponent<SystemBase>().CanBeOwnedByOrganisation(this))
            {
                xSystemPrefab = null;
                iCounter++;
                if (iCounter >= iCOUNTER_MAX)
                {
                    Debug.LogError("Failed to produce valid system");
                    return;
                }
            }
        }

        SystemBase xInstance = Instantiate(xSystemPrefab, transform).GetComponent<SystemBase>();
        m_xSystems.Add(xInstance);

        xInstance.SetOwner(this);
        xInstance.SetPosition(iXPos, iYPos);

        xInstance.Init();
    }

    public void GetAdjacentEmptySystems(ref List<(int, int)> xOutputs, bool bAdditive = false)
    {
        if (!bAdditive)
        {
            xOutputs.Clear();
        }
        if (m_xSystems.Count == 0)
        {
            xOutputs.Add((m_iXPosInGrid, m_iYPosInGrid));
        }
        var xDirections = Enum.GetValues(typeof(Manager.GridDirection)).Cast<Manager.GridDirection>();
        foreach (SystemBase xSystem in m_xSystems) {
            foreach (var xDirection in xDirections)
            {
                (int iX, int iY) = xSystem.GetGridPosition();
                if (Manager.GetAdjacentSystem(iX, iY, xDirection) == null)
                {
                    (int iNewX, int iNewY) = Manager.GetPositionInDirection(iX, iY, xDirection);
                    xOutputs.Add((iNewX, iNewY));
                }
            }
        }
    }
    protected void GetAdjacentOrganisations(ref HashSet<TechCompany> xComps)
    {
        if (xComps.Count != 0)
        {
            Debug.LogWarning("Non-empty hash set passed in for GetAdjacentOrganisations output");
            xComps.Clear();
        }
        var xDirections = Enum.GetValues(typeof(Manager.GridDirection)).Cast<Manager.GridDirection>();
        foreach (SystemBase xSystem in m_xSystems)
        {
            foreach (var xDirection in xDirections)
            {
                (int iX, int iY) = xSystem.GetGridPosition();
                SystemBase xSys = Manager.GetAdjacentSystem(iX, iY, xDirection);
                if (Manager.GetAdjacentSystem(iX, iY, xDirection) != null)
                {
                    OrganisationBase xOrg = xSys.GetOwner();
                    if (xOrg is TechCompany)
                    {
                        xComps.Add((TechCompany) xOrg);
                    }
                }
            }
        }
    }

    public bool AtCapacityOrBlocked()
    {
        return m_xSystems.Count() >= m_iCapacity || m_bBlocked;
    }
    public void SetPosition(int iX, int iY)
    {
        m_iXPosInGrid = iX;
        m_iYPosInGrid = iY;
        CorrectPosition();
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
