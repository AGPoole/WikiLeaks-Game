using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Country : MonoBehaviour
{
    [SerializeField]
    Government m_xGovernment;
    [SerializeField]
    List<TechCompany> m_xTechCompanies;
    [SerializeField]
    Population m_xPopulation;

    CountryData m_xCountryData;

    [SerializeField]
    GameObject m_xEmptyTechCompanyPrefab;

    void Awake()
    {
        m_xCountryData = new CountryData(m_xPopulation.GetPopulationData(), (GovernmentData)m_xGovernment.GetData());
        foreach(var xData in m_xTechCompanies)
        {
            m_xCountryData.AddTechCompanyData((TechCompanyData)xData.GetData());
        }
        m_xPopulation.SetGovernment(m_xGovernment);
    }

    // TODO make adjustable
    const int g_iNUM_TECH_COMPANIES = 15;
    public void OnNextTurn()
    {
        m_xCountryData.OnNextTurn();

        bool bAllAtCapacity = true;
        foreach(var xTechComp in m_xTechCompanies)
        {
            xTechComp.OnNextTurn();
            bAllAtCapacity &= xTechComp.AtCapacityOrBlocked();
        }
        m_xGovernment.OnNextTurn();
        bAllAtCapacity &= m_xGovernment.AtCapacityOrBlocked();
        m_xPopulation.OnNextTurn();

        DisasterSystem.ActivateDisasters(this);

        if (bAllAtCapacity && m_xTechCompanies.Count<g_iNUM_TECH_COMPANIES)
        {
            List<(int, int)> xPerimeterTiles = new List<(int, int)>();
            foreach(TechCompany xTechComp in m_xTechCompanies)
            {
                xTechComp.GetAdjacentEmptySystems(ref xPerimeterTiles, true);
            }
            m_xGovernment.GetAdjacentEmptySystems(ref xPerimeterTiles, true);
            if (xPerimeterTiles.Count > 0) {
                var xTileDistribution = new ProjectMaths.Distribution<(int, int)>(xPerimeterTiles, PositionProbWeighting);
                TechCompany xNewComp = Instantiate(m_xEmptyTechCompanyPrefab, transform).GetComponent<TechCompany>();
                m_xTechCompanies.Add(xNewComp);
                // not strictly uniformly-random, since one tile may appear more than once
                // however, I don't think it matters enough to warrant removing repeats
                (int iX, int iY) = xTileDistribution.Sample();
                xNewComp.SetPosition(iX, iY);
                xNewComp.Init();
                m_xCountryData.AddTechCompanyData((TechCompanyData)xNewComp.GetData());
            }
        }
    }

    public float PositionProbWeighting((int, int) xCoords, int iIndex)
    {
        Vector3 xPos = Manager.GetManager().GetPositionFromGridCoords(xCoords.Item1, xCoords.Item2);
        return 10f / Mathf.Pow( Vector3.Distance(m_xGovernment.transform.position, xPos), 2);
    }

    public CountryData GetCountryData()
    {
        return m_xCountryData;
    }

    public Government GetGovernment()
    {
        return m_xGovernment;
    }
    
    public Population GetPopulation()
    {
        return m_xPopulation;
    }
    
    public List<TechCompany> GetTechCompanies()
    {
        return m_xTechCompanies;
    }
}

public class CountryData
{
    PopulationData m_xPopulationData;
    List<TechCompanyData> m_xTechCompaniesData;
    GovernmentData m_xGovernmentData;

    public CountryData(PopulationData xPopulationData, GovernmentData xGovernmentData)
    {
        m_xTechCompaniesData = new List<TechCompanyData>();
        m_xPopulationData = xPopulationData;
        m_xPopulationData.SetCountryData(this);
        m_xGovernmentData = xGovernmentData;
        m_xGovernmentData.SetCountryData(this);
    }

    public void AddTechCompanyData(TechCompanyData xData)
    {
        m_xTechCompaniesData.Add(xData);
        xData.SetCountryData(this);
    }

    public void OnNextTurn()
    {
        foreach (var xData in m_xTechCompaniesData)
        {
            xData.OnNextTurn();
        }
        m_xGovernmentData.OnNextTurn();
        m_xPopulationData.OnNextTurn();
    }

    public GovernmentData GetGovernmentData()
    {
        return m_xGovernmentData;
    }

    public int GetTotalTechCompaniesSize()
    {
        int iValue = 0;
        foreach(var xData in m_xTechCompaniesData)
        {
            iValue += xData.GetSize();
        }
        return iValue;
    }

    public PopulationData GetPopulationData()
    {
        return m_xPopulationData;
    }

    public CountryData GetFake()
    {
        PopulationData xFakePop = m_xPopulationData.ShallowCopy();
        GovernmentData xFakeGov = (GovernmentData)m_xGovernmentData.ShallowCopy();
        var xFake = new CountryData(xFakePop, xFakeGov);
        foreach(var xData in m_xTechCompaniesData)
        {
            xFake.AddTechCompanyData((TechCompanyData)xData.ShallowCopy());
        }
        return xFake;
    }

    public float GetTotalShare()
    {
        float fValue = 0;
        foreach(var xData in m_xTechCompaniesData)
        {
            fValue += xData.GetMarketShare();
        }
        return fValue;
    }
}
