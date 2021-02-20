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

    void Awake()
    {
        m_xCountryData = new CountryData(m_xPopulation.GetPopulationData(), (GovernmentData)m_xGovernment.GetData());
        foreach(var xData in m_xTechCompanies)
        {
            m_xCountryData.AddTechCompanyData((TechCompanyData)xData.GetData());
        }
        m_xPopulation.SetGovernment(m_xGovernment);
    }

    public void OnNextTurn()
    {
        m_xCountryData.OnNextTurn();

        foreach(var xTechComp in m_xTechCompanies)
        {
            xTechComp.OnNextTurn();
        }
        m_xGovernment.OnNextTurn();
        m_xPopulation.OnNextTurn();

        DisasterSystem.ActivateDisasters(this);
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
