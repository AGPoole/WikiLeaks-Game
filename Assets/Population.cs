using UnityEngine;

public class Population : MonoBehaviour
{
    [SerializeField]
    PopulationData m_xPopulationData;

    [SerializeField]
    UnityEngine.UI.Text m_xHappinessText;
    [SerializeField]
    UnityEngine.UI.Text m_xTaxText;
    [SerializeField]
    UnityEngine.UI.Text m_xCorpText;
    Government m_xGovernment;

    public void SetGovernment(Government xGov)
    {
        m_xGovernment = xGov;
    }

    public PopulationData GetPopulationData()
    {
        return m_xPopulationData;
    }

    int m_iNextRevolutionTurn = 0;
    public void OnNextTurn()
    {
        m_xHappinessText.text = m_xPopulationData.GetHappiness().ToString("0.00");
        m_xTaxText.text = m_xPopulationData.GetGovHappiness().ToString("0.00");
        m_xCorpText.text = m_xPopulationData.GetCorpHappiness().ToString("0.00");

        if (m_xPopulationData.GetHappiness() < 0.6f && Manager.GetTurnNumber() > m_iNextRevolutionTurn)
        {
            bool bSuccess = m_xGovernment.OnRevolution();
            m_iNextRevolutionTurn = Manager.GetTurnNumber();
            m_iNextRevolutionTurn += bSuccess ? 1000 : 100;
            NotificationSystem.AddNotification(bSuccess ? "Successful revolution" : "Failed revolution");
        }
    }
}

[System.Serializable]
public class PopulationData
{
    [SerializeField]
    float m_fHappiness;
    float m_fCorpHappiness;
    float m_fGovHappiness;

    float m_fTaxMoneyThisTurn;
    float m_fCorpMoneyThisTurn;

    float m_fTaxMoneyLastTurn;
    float m_fCorpMoneyLastTurn;

    CountryData m_xCountryData;

    public void SetCountryData(CountryData xCountryData)
    {
        m_xCountryData = xCountryData;
    }

    public void OnNextTurn()
    {
        m_fHappiness = Manager.CalculateHappiness(m_xCountryData.GetGovernmentData().GetSize(), m_xCountryData.GetTotalTechCompaniesSize(), true);

        // 10/12/2020 - currently the overall happiness should just be the sum of these 2, so it is ok to sum them. If that ever changes, this should be updates
        m_fCorpHappiness = Manager.CalculateHappiness(0, m_xCountryData.GetTotalTechCompaniesSize(), true);
        m_fGovHappiness = Manager.CalculateHappiness(m_xCountryData.GetGovernmentData().GetSize(), 0, true);
        if (Mathf.Abs(m_fCorpHappiness + m_fGovHappiness - m_fHappiness) > 0.005)
        {
            Debug.LogError("Happiness from Gov and Corp do not sum to correct total");
        }

        m_fTaxMoneyLastTurn = m_fTaxMoneyThisTurn;
        m_fCorpMoneyLastTurn = m_fCorpMoneyThisTurn;
        m_fTaxMoneyThisTurn = 0;
        m_fCorpMoneyThisTurn = 0;
    }

    // ToDo: remove this and corp/gov money variables
    [SerializeField]
    float m_fGovernmentFactor;
    public void PayTaxes(float fTaxes)
    {
        m_fTaxMoneyThisTurn += fTaxes;
    }

    [SerializeField]
    float m_fCorporateFactor;
    public void ContributeToHappiness(float fContribution)
    {
        m_fCorpMoneyThisTurn += fContribution;
    }

    public PopulationData ShallowCopy()
    {
        return (PopulationData)this.MemberwiseClone();
    }

    public float GetHappiness()
    {
        return m_fHappiness;
    }

    // Only use for visuals
    public float GetCorpMoneyLastTurn()
    {
        return m_fCorpMoneyLastTurn;
    }
    // Only use for visuals
    public float GetTaxMoneyLastTurn()
    {
        return m_fTaxMoneyLastTurn;
    }

    public float GetCorpHappiness()
    {
        return m_fCorpHappiness;
    }
    public float GetGovHappiness()
    {
        return m_fGovHappiness;
    }
}