using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum Orientation
{
    LEFT,
    RIGHT
}

public class Government : OrganisationBase
{
    //TODO: prevent government from reaching size 0
    [SerializeField]
    float m_fTechLevel = 1;

    [SerializeField]
    UnityEngine.UI.Text m_xTaxText;
    [SerializeField]
    UnityEngine.UI.Text m_xSavingsPerTurnText;

    [SerializeField]
    Transform m_xLeftTransform;
    [SerializeField]
    Transform m_xRightTransform;
    [SerializeField]
    Transform m_xLeaderTransform;

    [SerializeField]
    Candidate m_xLeftCandidate;
    [SerializeField]
    Candidate m_xRightCandidate;
    [SerializeField]
    Candidate m_xLeader;

    [SerializeField]
    GameObject m_xCandidatePrefab;

    [SerializeField]
    int m_uElectionPeriod;

    [SerializeField]
    int m_uLastElection;

    [SerializeField]
    UnityEngine.UI.Slider m_xHappinessSlider;
    [SerializeField]
    UnityEngine.UI.Slider m_xScoreSlider;
    [SerializeField]
    UnityEngine.UI.Text m_xElectionTurnsCounter;

#if (UNITY_EDITOR)
    StreamWriter m_xStreamWriter;
#endif
    [SerializeField]
    Transform m_xLeftTarget;
    [SerializeField]
    Transform m_xRightTarget;

    public override void Init()
    {
        base.Init();
#if (UNITY_EDITOR)
        string xPath = Application.dataPath + "/CSV/Taxes.csv";
        m_xStreamWriter = File.CreateText(xPath);
#endif
        var xGovData = (GovernmentData)m_xMyData;
        xGovData.SetCandidateData(m_xLeftCandidate.GetCandidateData());
        m_xLeftCandidate.SetAsLeader();

        m_xLeftCandidate.SetUp(xGovData.GetCountryData(), this);
        m_xRightCandidate.SetUp(xGovData.GetCountryData(), this);
    }

    float m_fPreviousSavings = 0f;

    [SerializeField]
    float m_fHappinessWeight = 1f;
    [SerializeField]
    float m_fBiasWeight = 1f;
    [Range(-1, 1)]
    [SerializeField]
    float m_fBias = 0f;
    [SerializeField]
    List<PopularityModifier> m_xModifiers;
    bool m_bElectionsEnabled = true;

    public override void OnNextTurn()
    {
        base.OnNextTurn();

        if (GetTimeTillNextElection() < 100)
        {
            RectTransform xRect = m_xLeader.GetComponent<RectTransform>();
            xRect.SetParent(m_xLeader.GetOrientation() == Orientation.LEFT ? m_xLeftTransform : m_xRightTransform, false);
            xRect.localPosition = Vector3.zero;
            xRect.anchoredPosition3D = Vector3.zero;
            xRect.sizeDelta = Vector2.zero;
            xRect.anchorMin = Vector2.zero;
            xRect.anchorMax = Vector2.one;
            if (GetOpposition() != null)
            {
                GetOpposition().gameObject.SetActive(true);
            }
        }
        else
        {
            m_xLeader.transform.parent = m_xLeaderTransform;
            m_xLeader.transform.localPosition = Vector3.zero;
            if (GetOpposition() != null)
            {
                GetOpposition().gameObject.SetActive(false);
            }
        }

        var xGovData = (GovernmentData)m_xMyData;
        float fCurrentHappiness = xGovData.GetCountryData().GetPopulationData().GetHappiness();
        float fLeftHappiness = m_xLeftCandidate.GetPredictedHappiness();
        float fRightHappiness = m_xRightCandidate.GetPredictedHappiness();

        if (DebugSettings.ShouldDetectFakeChanges()
            && fCurrentHappiness != xGovData.GetCountryData().GetPopulationData().GetHappiness())
        {
            Debug.LogError("Mock country has changed the real world");
        }

        float fLeftScore = 0f;
        float fRightScore = 0f;
        CalculatePopularityScores(ref fLeftScore, ref fRightScore, fLeftHappiness, fRightHappiness);

        if (GetElectionsEnabled())
        {
            if (fLeftHappiness + fRightHappiness > 0)
            {
                m_xHappinessSlider.value = fLeftHappiness / (fLeftHappiness + fRightHappiness);
            }
            if (fLeftScore + fRightScore > 0)
            {
                m_xScoreSlider.value = fLeftScore / (fLeftScore + fRightScore);
            }

            // TODO: Change to be based around previous happiness?
            if (GetTimeTillNextElection() <= 0 && GetElectionsEnabled())
            {
                m_uLastElection = Manager.GetTurnNumber();

                bool bLeftWon = fLeftScore > fRightScore;
                Candidate xLoser = bLeftWon ? m_xRightCandidate : m_xLeftCandidate;
                m_xLeader = bLeftWon ? m_xLeftCandidate : m_xRightCandidate;

                m_xLeader.SetAsLeader();
                ReplaceCandidate(xLoser);
                NotificationSystem.AddNotification(string.Format("{0} won the election!", bLeftWon ? "Liberals" : "Conservatives"));

                xGovData.SetCandidateData(m_xLeader.GetCandidateData());
            }
            m_xLeftCandidate.OnNextTurn(fLeftScore / (fLeftScore + fRightScore));
            m_xRightCandidate.OnNextTurn(fRightScore / (fLeftScore + fRightScore));
        }
        else
        {
            float fLeaderScore = m_xLeader.GetCandidateData().GetOrientation() == Orientation.LEFT ? fLeftScore : fRightScore;
            m_xLeader.OnNextTurn(fLeaderScore / fLeftScore + fRightScore);
            m_xElectionTurnsCounter.text = "Elections Have Been Postponed Indefinitely";
        }

        if (GetTimeTillNextElection() == 5)
        {
            NotificationSystem.AddNotification("5 turns until the election");
        }

        UpdateModifiers();

#if (UNITY_EDITOR)
        if (Manager.GetTurnNumber() % 100 == 0 && Manager.ShouldWriteTaxesToFile())
        {
            WriteToFile();
        }
#endif
    }

    public void EnableElections()
    {
        if (GetElectionsEnabled())
        {
            return;
        }
        bool bLeft = m_xLeader.GetOrientation() != Orientation.LEFT;
        Orientation eNewOrientation = bLeft ? Orientation.LEFT : Orientation.RIGHT;
        Candidate xNewCandidate = Instantiate(m_xCandidatePrefab, bLeft ? m_xLeftTransform : m_xRightTransform).GetComponent<Candidate>();
        CandidateData xNewCandidateData = xNewCandidate.GetCandidateData();
        xNewCandidateData.SetTaxRate(bLeft ? m_xLeader.GetCandidateData().GetTaxRate() + CandidateValues.GetTaxDifference() : m_xLeader.GetCandidateData().GetTaxRate() + CandidateValues.GetTaxDifference() - CandidateValues.GetTaxDifference());
        xNewCandidateData.SetPoliticalOrientation(eNewOrientation);
        xNewCandidate.SetUp(GetCountry().GetCountryData(), this);
        if (bLeft)
        {
            m_xLeftCandidate = xNewCandidate;
        }
        else
        {
            m_xRightCandidate = xNewCandidate;
        }

        m_bElectionsEnabled = true;
        m_xHappinessSlider.gameObject.SetActive(true);
        m_xScoreSlider.gameObject.SetActive(true);
    }

    public Candidate GetOpposition()
    {
        if (m_xLeader.GetOrientation() == Orientation.LEFT)
        {
            return m_xRightCandidate;
        }
        else
        {
            return m_xLeftCandidate;
        }
    }

    public void DisableElections()
    {
        if (!m_bElectionsEnabled)
        {
            return;
        }
        Candidate xLoser = m_xLeader.GetOrientation() == Orientation.LEFT ? m_xRightCandidate : m_xLeftCandidate;
        Destroy(xLoser.gameObject);

        m_bElectionsEnabled = false;
        m_xHappinessSlider.gameObject.SetActive(false);
        m_xScoreSlider.gameObject.SetActive(false);
    }

    public bool GetElectionsEnabled()
    {
        return ModelSettings.GetElectionsEnabled() && m_bElectionsEnabled;
    }

    void CalculatePopularityScores(ref float fLeftScore, ref float fRightScore,
        float fLeftHappiness, float fRightHappiness)
    {
        fLeftScore = m_fHappinessWeight * (fLeftHappiness / (fLeftHappiness + fRightHappiness));
        fRightScore = m_fHappinessWeight * (fRightHappiness / (fLeftHappiness + fRightHappiness));

        foreach (PopularityModifier xMod in m_xModifiers)
        {
            if (xMod.GetPoliticalOrientation() == Orientation.LEFT)
            {
                fLeftScore += xMod.GetEffect();
            }
            else
            {
                fRightScore += xMod.GetEffect();
            }
        }
        if (m_fBias >= 0)
        {
            fLeftScore += m_fBias * m_fBiasWeight;
        }
        else
        {
            fRightScore -= m_fBias * m_fBiasWeight;
        }
    }

    protected override void UpdateUI()
    {
        base.UpdateUI();
        var xGovData = (GovernmentData)GetData();
        m_xTaxText.text = xGovData.GetTaxRate().ToString("0.00");
        float fSavingsDiff = xGovData.GetSavings() - m_fPreviousSavings;
        m_fPreviousSavings = xGovData.GetSavings();
        m_xSavingsPerTurnText.text = fSavingsDiff.ToString("0.00");
        m_xElectionTurnsCounter.text = String.Format("{0} Turns Until Election", GetTimeTillNextElection());
    }

#if (UNITY_EDITOR)
    void WriteToFile()
    {
        var xGovData = (GovernmentData)GetData();
        float fHappiness = xGovData.GetCountryData().GetPopulationData().GetHappiness();
        fHappiness -= 0.45f;
        fHappiness /= 0.55f;
        m_xStreamWriter.WriteLine(string.Format("{0} {1} {2} {3}",
            xGovData.GetTaxRate().ToString("0.00"),
            fHappiness.ToString("0.00"),
            (xGovData.GetSize() / 140f).ToString("0.00"),
            (xGovData.GetCountryData().GetTotalTechCompaniesSize() / 80f).ToString("0.00")));
        m_xStreamWriter.Flush();
    }
#endif

#if (UNITY_EDITOR)
    void OnApplicationQuit()
    {
        m_xStreamWriter.Close();
        Debug.Log("Application ending after " + Time.time + " seconds");
    }
#endif
    void UpdateModifiers()
    {
        for (int i = m_xModifiers.Count - 1; i >= 0; i--)
        {
            m_xModifiers[i].OnNextTurn();
            if (m_xModifiers[i].ShouldRemove())
            {
                m_xModifiers[i].OnRemoval();
                m_xModifiers.RemoveAt(i);
            }
        }
    }

    public void AddModifier(PopularityModifier xModifier)
    {
        m_xModifiers.Add(xModifier);
    }

    public Candidate GetCandidate(Orientation eOrientation)
    {
        // TODO: change to a hashmap/array
        switch (eOrientation)
        {
            case Orientation.LEFT:
                {
                    if (m_xLeftCandidate.GetCandidateData().GetOrientation() != eOrientation)
                    {
                        Debug.LogError("Incorrect political orientation");
                    }
                    return m_xLeftCandidate;
                }
            case Orientation.RIGHT:
            default:
                {
                    if (m_xRightCandidate.GetCandidateData().GetOrientation() != eOrientation)
                    {
                        Debug.LogError("Incorrect political orientation");
                    }
                    return m_xRightCandidate;
                }

        }
    }

    void ReplaceCandidate(Candidate xCandidate)
    {
        if (xCandidate == null)
        {
            Debug.LogError("Null candidate set to be replaced!");
            return;
        }
        Candidate xNewCandidate = Instantiate(m_xCandidatePrefab, xCandidate.transform.parent).GetComponent<Candidate>();
        xNewCandidate.GetCandidateData().SetTaxRate(xCandidate.GetCandidateData().GetTaxRate());
        xNewCandidate.GetCandidateData().SetPoliticalOrientation(xCandidate.GetCandidateData().GetOrientation());
        switch (xCandidate.GetCandidateData().GetOrientation())
        {
            case Orientation.LEFT:
                {
                    Destroy(xCandidate.gameObject);
                    m_xLeftCandidate = xNewCandidate;
                    break;
                }
            case Orientation.RIGHT:
            default:
                {
                    Destroy(xCandidate.gameObject);
                    m_xRightCandidate = xNewCandidate;
                    break;
                }

        }
        xNewCandidate.SetUp(GetCountry().GetCountryData(), this);
    }

    public int GetTimeTillNextElection()
    {
        return m_uLastElection + m_uElectionPeriod - Manager.GetTurnNumber();
    }

    public bool OnRevolution()
    {
        float fSuccessProb = 0.5f;
        if (UnityEngine.Random.Range(0f, 1f) < fSuccessProb)
        {
            return false;
        }
        EnableElections();
        foreach (var sys in GetSystemsOfType(typeof(Censorship)))
        {
            sys.ModifyLevel(-100, 1000);
        }
        m_xLeftCandidate.GetCandidateData().SetTaxRate(0.35f);
        ((GovernmentData)m_xMyData).ForceSize(1);
        return true;
    }

    public Transform GetTarget(Orientation eOrientation)
    {
        return eOrientation == Orientation.LEFT ? m_xLeftTarget : m_xRightTarget;
    }

    protected override void SetData()
    {
        if (m_xMyData == null)
        {
            m_xMyData = new GovernmentData();
        }
    }
}

[System.Serializable]
public class GovernmentData : OrganisationData
{
    CandidateData m_xCandidateData;
    [SerializeField]
    [Range(0, 1)]
    float m_fApproval = 0.5f;

    public void PayTaxes(float fTaxes)
    {
        //TODO: split taxes between parts
        m_fSavings += fTaxes;
        m_xCountryData.GetPopulationData().PayTaxes(fTaxes * (1 - m_xCandidateData.GetPowerPercentage()));
    }

    public CountryData GetCountryData()
    {
        return m_xCountryData;
    }

    public override OrganisationData ShallowCopy()
    {
        return (GovernmentData)this.MemberwiseClone();
    }
    public float GetTaxRate()
    {
        return m_xCandidateData.GetTaxRate();
    }

    public void SetCandidateData(CandidateData xCandidateData)
    {
        m_xCandidateData = xCandidateData;
    }

    public float GetApproval()
    {
        return m_fApproval;
    }

    // TODO: remove this
    public void ForceSize(int iSize)
    {
        m_iSize = iSize;
    }

    public float GetPowerPercentage()
    {
        return m_xCandidateData.GetPowerPercentage();
    }

    public override OrganisationValuesBase GetValues()
    {
        return GovernmentValuesContainer.GetGovernmentValues();
    }
}

[System.Serializable]
public class PopularityModifier
{
    [SerializeField]
    protected float m_fEffect;
    [SerializeField]
    protected Orientation m_eOrientation;
    [SerializeField]
    protected int m_iRemovalTurn;
    [SerializeField]
    protected string m_xDescription;
    [SerializeField]
    protected bool m_bTimesOut;

    public PopularityModifier(float fEffect, Orientation eOrientation, int iRemovalTime, string xDescription, bool bTimesOut = true)
    {
        m_fEffect = fEffect;
        m_eOrientation = eOrientation;
        m_iRemovalTurn = Manager.GetTurnNumber() + iRemovalTime;
        m_xDescription = xDescription;
        m_bTimesOut = bTimesOut;
    }

    public virtual bool ShouldRemove()
    {
        return m_bTimesOut && Manager.GetTurnNumber() > m_iRemovalTurn;
    }
    public virtual void OnNextTurn() { }

    public virtual void OnRemoval() { }

    public float GetEffect()
    {
        return m_fEffect;
    }

    public Orientation GetPoliticalOrientation()
    {
        return m_eOrientation;
    }

    public int GetRemovalTurn()
    {
        return m_iRemovalTurn;
    }

    public virtual PopularityModifier Clone()
    {
        return MemberwiseClone() as PopularityModifier;
    }
}