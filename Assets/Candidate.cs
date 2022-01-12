using UnityEngine;

public class Candidate : MonoBehaviour
{
    CountryData m_xCountryData;
    Government m_xGovernment;
    [SerializeField]
    CandidateData m_xCandidateData;
    [SerializeField]
    bool m_bInPower;
    [SerializeField]
    UnityEngine.UI.Text m_xTaxText;
    [SerializeField]
    UnityEngine.UI.Text m_xHappinessText;
    [SerializeField]
    UnityEngine.UI.Text m_xNameText;
    [SerializeField]
    GameObject m_xExposeButton;

    CandidateValues.Name m_xName;

    void Start()
    {
        m_xName = CandidateValues.GetRandomName(m_xCandidateData.GetOrientation());
        if (m_xNameText != null)
        {
            m_xNameText.text = m_xName.GetString(m_bInPower);
        }
    }

    public CandidateData GetCandidateData()
    {
        return m_xCandidateData;
    }

    public Orientation GetOrientation()
    {
        return GetCandidateData().GetOrientation();
    }

    public void SetUp(CountryData xCountryData, Government xGov)
    {
        m_xCountryData = xCountryData;
        m_xGovernment = xGov;
    }

    public void SetAsLeader()
    {
        m_bInPower = true;
    }

    public void RecalculateVisuals()
    {
        m_xTaxText.text = m_xCandidateData.GetTaxRate().ToString("0.00");
        m_xHappinessText.text = GetPredictedHappiness().ToString("0.00");
        if (m_xName != null)
        {
            m_xNameText.text = m_xName.GetString(m_bInPower);
        }
    }
    public void OnNextTurn(float fPopScore)
    {
        m_xTaxText.text = m_xCandidateData.GetTaxRate().ToString("0.00");
        m_xHappinessText.text = GetPredictedHappiness().ToString("0.00");
        if (m_xName != null)
        {
            m_xNameText.text = m_xName.GetString(m_bInPower);
        }

        if (CandidateValues.GetCandidateMovementEnabled() && m_xGovernment.GetElectionsEnabled())
        {
            //Only one candidate needs to update - just update LEFT and have RIGHT follow
            if (GetCandidateData().GetOrientation() == Orientation.LEFT)
            {
                if (fPopScore > 0.5f + CandidateValues.GetChangeBoundary())
                {
                    ChangeTaxRate(0.01f);
                }
                else if (fPopScore < 0.5f - CandidateValues.GetChangeBoundary())
                {
                    ChangeTaxRate(-0.01f);
                }
            }
            else
            {
                var xOpponentData = m_xGovernment.GetCandidate(Orientation.LEFT).GetCandidateData();
                GetCandidateData().SetTaxRate(xOpponentData.GetTaxRate() - CandidateValues.GetTaxDifference());
            }
        }
        RecalculateVisuals();

        // TODO: do this every frame
        var axPropagandas = FindObjectsOfType<Propaganda>();
        m_xExposeButton.SetActive(false);
        foreach (var xProp in axPropagandas)
        {
            if (xProp.IsHacked())
            {
                m_xExposeButton.SetActive(true);
            }
        }
    }

    int m_iLastCalculated = -1;
    float m_fLastValue = 0;
    [SerializeReference]
    [Range(1, 1000)]
    int m_iLookAhead = 100;
    [SerializeField]
    int m_iCalculateionDelay = 10;
    public float GetPredictedHappiness()
    {
        // only calculate this once a turn, for now. May be good to recalculate if the player does something that could change it
        if (m_iLastCalculated < Manager.GetTurnNumber() - m_iCalculateionDelay)
        {
            m_iLastCalculated = Manager.GetTurnNumber();

            CountryData xFake = m_xCountryData.GetFake();
            xFake.GetGovernmentData().SetCandidateData(GetCandidateData());
            for (int i = 0; i < m_iLookAhead; i++)
            {
                xFake.OnNextTurn();
            }
            m_fLastValue = xFake.GetPopulationData().GetHappiness();
        }
        return m_fLastValue;
    }

    public void ChangeTaxRate(float fChange)
    {
        m_xCandidateData.SetTaxRate(m_xCandidateData.GetTaxRate() + fChange);
    }

    public void ExposeScandal()
    {
        CandidateValues.Scandal xScandal = CandidateValues.GetRandomScandal();

        NotificationSystem.AddNotification(string.Format("Breaking news: {0}", xScandal.GetScandalText(m_xName)));
        PopularityModifier xPopMod = new PopularityModifier(CandidateValues.GetScandalEffect(), m_xCandidateData.GetOrientation(),
            CandidateValues.GetScandalLength(), xScandal.GetScandalText(m_xName));
        m_xGovernment.AddModifier(xPopMod);
    }
}

[System.Serializable]
public class CandidateData
{
    [SerializeField]
    float m_fTaxRate;
    [SerializeField]
    float m_fPowerPercentage;
    [SerializeField]
    Orientation m_eOrientation;

    public Orientation GetOrientation()
    {
        return m_eOrientation;
    }

    public float GetTaxRate()
    {
        return m_fTaxRate;
    }

    public float GetPowerPercentage()
    {
        return m_fPowerPercentage;
    }
    public void SetTaxRate(float fTaxRate)
    {
        if (m_eOrientation == Orientation.LEFT)
        {
            m_fTaxRate = Mathf.Clamp(fTaxRate, CandidateValues.GetMinTax() + CandidateValues.GetTaxDifference(), CandidateValues.GetMaxTax());
        }
        else
        {
            m_fTaxRate = Mathf.Clamp(fTaxRate, CandidateValues.GetMinTax(), CandidateValues.GetMaxTax() - CandidateValues.GetTaxDifference());
        }
    }

    public void SetPoliticalOrientation(Orientation eOrientation)
    {
        m_eOrientation = eOrientation;
    }
}
