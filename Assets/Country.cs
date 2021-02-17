using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Country : MonoBehaviour
{
    [SerializeField]
    Government m_xGovernment;
    [SerializeField]
    TechCompany m_xTechCompany;
    [SerializeField]
    Population m_xPopulation;

    CountryData m_xCountryData;

    void Awake()
    {
        m_xCountryData = new CountryData(m_xPopulation.GetPopulationData(), (TechCompanyData)m_xTechCompany.GetData(), m_xGovernment.GetGovernmentData());
        m_xPopulation.SetGovernment(m_xGovernment);
    }

    public void OnNextTurn()
    {
        m_xCountryData.OnNextTurn();

        m_xTechCompany.OnNextTurn();
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
    
    public TechCompany GetTechCompany()
    {
        return m_xTechCompany;
    }
}

public class CountryData
{
    PopulationData m_xPopulationData;
    TechCompanyData m_xTechCompanyData;
    GovernmentData m_xGovernmentData;

    public CountryData(PopulationData xPopulationData, TechCompanyData xTechCompanyData, GovernmentData xGovernmentData)
    {
        m_xPopulationData = xPopulationData;
        m_xPopulationData.SetCountryData(this);
        m_xTechCompanyData = xTechCompanyData;
        m_xTechCompanyData.SetCountryData(this);
        m_xGovernmentData = xGovernmentData;
        m_xGovernmentData.SetCountryData(this);
    }

    public void OnNextTurn()
    {
        m_xTechCompanyData.OnNextTurn();
        m_xGovernmentData.OnNextTurn();
        m_xPopulationData.OnNextTurn();
    }

    public GovernmentData GetGovernmentData()
    {
        return m_xGovernmentData;
    }

    public TechCompanyData GetTechCompanyData()
    {
        return m_xTechCompanyData;
    }

    public PopulationData GetPopulationData()
    {
        return m_xPopulationData;
    }

    public CountryData GetFake()
    {
        PopulationData xFakePop = m_xPopulationData.ShallowCopy();
        TechCompanyData xFakeTech = (TechCompanyData)m_xTechCompanyData.ShallowCopy();
        GovernmentData xFakeGov = (GovernmentData)m_xGovernmentData.ShallowCopy();
        return new CountryData(xFakePop, xFakeTech, xFakeGov);
    }
}
