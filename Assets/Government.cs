using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum Orientation
{
    LEFT,
    RIGHT
}

public class Government : MonoBehaviour
{

    [SerializeField]
    float m_fTechLevel = 1;
    [SerializeField]
    GovernmentData m_xGovernmentData;

    [SerializeField]
    UnityEngine.UI.Text m_xTaxText;
    [SerializeField]
    UnityEngine.UI.Text m_xSizeText;
    [SerializeField]
    UnityEngine.UI.Text m_xSavingsText;
    [SerializeField]
    UnityEngine.UI.Text m_xLevelUpRequirementText;
    [SerializeField]
    UnityEngine.UI.Text m_xSavingsPerTurnText;
    [SerializeField]
    UnityEngine.UI.Slider m_xLevelUpSlider;
    [SerializeField]
    UnityEngine.UI.Text m_xTimeTilNextElectionText;

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
    Country m_xCountry;

    StreamWriter m_xStreamWriter;
    void Start()
    {
        string xPath = Application.dataPath + "/CSV/Taxes.csv";
        m_xStreamWriter = File.CreateText(xPath);

        m_xGovernmentData.SetCandidateData(m_xLeftCandidate.GetCandidateData());
        m_xLeftCandidate.SetAsLeader();

        m_xLeftCandidate.SetUp(m_xGovernmentData.GetCountryData(), this);
        m_xRightCandidate.SetUp(m_xGovernmentData.GetCountryData(), this);
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
    [SerializeField]
    List<SystemBase> m_xSystems;
    bool m_bElectionsEnabled = true;

    public void OnNextTurn()
    {
        UpdateSystems();

        if (GetTimeTillNextElection() < 100)
        {
            m_xLeader.transform.parent = m_xLeader.GetOrientation() == Orientation.LEFT ? m_xLeftTransform : m_xRightTransform;
            m_xLeader.transform.localPosition = Vector3.zero;
            if (GetOpposition() != null)
            {
                GetOpposition().gameObject.SetActive(true);
            }

            m_xTimeTilNextElectionText.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            m_xTimeTilNextElectionText.transform.parent.gameObject.SetActive(false);
            m_xLeader.transform.parent = m_xLeaderTransform;
            m_xLeader.transform.localPosition = Vector3.zero;
            if (GetOpposition() != null)
            {
                GetOpposition().gameObject.SetActive(false);
            }
        }

        float fCurrentHappiness = m_xGovernmentData.GetCountryData().GetPopulationData().GetHappiness();
        float fLeftHappiness = m_xLeftCandidate.GetPredictedHappiness();
        float fRightHappiness = m_xRightCandidate.GetPredictedHappiness();

        if (DebugSettings.ShouldDetectFakeChanges()
            && fCurrentHappiness != m_xGovernmentData.GetCountryData().GetPopulationData().GetHappiness())
        {
            Debug.LogError("Mock country has changed the real world");
        }

        float fLeftScore = 0f;
        float fRightScore = 0f;
        CalculatePopularityScores(ref fLeftScore, ref fRightScore, fLeftHappiness, fRightHappiness);

        if (GetElectionsEnabled())
        {
            m_xHappinessSlider.value = fLeftHappiness / (fLeftHappiness + fRightHappiness);
            m_xScoreSlider.value = fLeftScore / (fLeftScore + fRightScore);

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

                m_xGovernmentData.SetCandidateData(m_xLeader.GetCandidateData());
            }
            m_xLeftCandidate.OnNextTurn(fLeftScore / (fLeftScore + fRightScore));
            m_xRightCandidate.OnNextTurn(fRightScore / (fLeftScore + fRightScore));
        }
        else
        {
            float fLeaderScore = m_xLeader.GetCandidateData().GetOrientation() == Orientation.LEFT ? fLeftScore : fRightScore;
            m_xLeader.OnNextTurn(fLeaderScore / fLeftScore + fRightScore);
        }

        RefreshGUI();
        if (GetTimeTillNextElection() == 5)
        {
            NotificationSystem.AddNotification("5 turns until the election");
        }

        UpdateModifiers();

        if (Manager.GetTurnNumber() % 100 == 0)
        {
            WriteToFile();
        }
    }

    public void UpdateSystems()
    {
        foreach (SystemBase m_xSys in m_xSystems)
        {
            m_xSys.OnNextTurn(m_xGovernmentData.GetSize());
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
            if(fTotal > GetGovernmentData().GetSize())
            {
                int iToRemove = UnityEngine.Random.Range(0, m_xSystems.Count - 1);
                m_xSystems[iToRemove].LevelDown();
            }
        } while (fTotal > GetGovernmentData().GetSize());
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
        if(fTotal + fCheapestCost <= GetGovernmentData().GetSize() + 5)
        {
            xCheapest.LevelUp();
        }
    }

    public void EnableElections()
    {
        if (GetElectionsEnabled())
        {
            return;
        }
        bool bLeft = m_xLeader.GetOrientation() != Orientation.LEFT;
        Orientation eNewOrientation = bLeft ? Orientation.LEFT: Orientation.RIGHT;
        Candidate xNewCandidate = Instantiate(m_xCandidatePrefab, bLeft ? m_xLeftTransform: m_xRightTransform).GetComponent<Candidate>();
        CandidateData xNewCandidateData = xNewCandidate.GetCandidateData();
        xNewCandidateData.SetTaxRate(bLeft ? m_xLeader.GetCandidateData().GetTaxRate() + CandidateValues.GetTaxDifference() : m_xLeader.GetCandidateData().GetTaxRate() + CandidateValues.GetTaxDifference() - CandidateValues.GetTaxDifference());
        xNewCandidateData.SetPoliticalOrientation(eNewOrientation);
        xNewCandidate.SetUp(m_xCountry.GetCountryData(), this);
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

    void RefreshGUI()
    {
        m_xTaxText.text = m_xGovernmentData.GetTaxRate().ToString("0.00");
        m_xSavingsText.text = m_xGovernmentData.GetSavings().ToString("0.00");
        m_xSizeText.text = m_xGovernmentData.GetSize().ToString();
        float fSavingsDiff = m_xGovernmentData.GetSavings() - m_fPreviousSavings;
        m_fPreviousSavings = m_xGovernmentData.GetSavings();
        m_xSavingsPerTurnText.text = fSavingsDiff.ToString("0.00");
        m_xLevelUpRequirementText.text = GovernmentValues.GetTotalRequirementAtLevel(m_xGovernmentData.GetSize()).ToString("0.00");
        m_xLevelUpSlider.value = m_xGovernmentData.GetSavings() / GovernmentValues.GetTotalRequirementAtLevel(m_xGovernmentData.GetSize());
        m_xTimeTilNextElectionText.text = GetTimeTillNextElection().ToString();
    }

    void WriteToFile()
    {
        float fHappiness = m_xGovernmentData.GetCountryData().GetPopulationData().GetHappiness();
        fHappiness -= 0.45f;
        fHappiness /= 0.55f;
        m_xStreamWriter.WriteLine(string.Format("{0} {1} {2} {3}", 
            m_xGovernmentData.GetTaxRate().ToString("0.00"), 
            fHappiness.ToString("0.00"),
            (m_xGovernmentData.GetSize()/140f).ToString("0.00"),
            (m_xGovernmentData.GetCountryData().GetTechCompanyData().GetSize()/80f).ToString("0.00")));
        m_xStreamWriter.Flush();
    }

    void OnApplicationQuit()
    {
        m_xStreamWriter.Close();
        Debug.Log("Application ending after " + Time.time + " seconds");
    }
    void UpdateModifiers()
    {
        for(int i=m_xModifiers.Count-1; i>=0; i--)
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

    public GovernmentData GetGovernmentData()
    {
        return m_xGovernmentData;
    }

    public Country GetCountry()
    {
        return m_xCountry;
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
        if(xCandidate==null)
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
        xNewCandidate.SetUp(m_xCountry.GetCountryData(), this);
    }

    public int GetTimeTillNextElection()
    {
        return m_uLastElection + m_uElectionPeriod - Manager.GetTurnNumber();
    }

    public bool OnRevolution()
    {
        float fSuccessProb = 0.5f;
        if(UnityEngine.Random.Range(0f, 1f) < fSuccessProb)
        {
            return false;
        }
        EnableElections();
        foreach(var sys in GetSystemsOfType(typeof(Censorship)))
        {
            sys.ModifyLevel(-100, 1000);
        }
        m_xLeftCandidate.GetCandidateData().SetTaxRate(0.35f);
        m_xGovernmentData.ForceSize(1);
        return true;
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
public class GovernmentData
{
    CandidateData m_xCandidateData;
    [SerializeField]
    [Range(0, 1)]
    float m_fApproval = 0.5f;
    [SerializeField]
    int m_iSize = 1;
    [SerializeField]
    float m_fSavings = 0;
    [SerializeField]
    CountryData m_xCountryData;

    public void SetCountryData(CountryData xCountryData)
    {
        m_xCountryData = xCountryData;
    }

    public void OnNextTurn()
    {
        m_fSavings -= GovernmentValues.GetLevelUpCostAtLevel(m_iSize);
        if (m_fSavings > GovernmentValues.GetTotalRequirementAtLevel(m_iSize))
        {
            m_iSize += 1;
            m_fSavings -= GovernmentValues.GetTotalRequirementAtLevel(m_iSize);
        }
        else if(m_fSavings<0 && m_iSize>1)
        {
            m_iSize -= 1;
            m_fSavings += GovernmentValues.GetTotalRequirementAtLevel(m_iSize);
        }
    }

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

    public GovernmentData ShallowCopy()
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

    public int GetSize()
    {
        return m_iSize;
    }

    // TODO: remove this
    public void ForceSize(int iSize)
    {
        m_iSize = iSize;
    }

    public float GetSavings()
    {
        return m_fSavings;
    }

    public float GetPowerPercentage()
    {
        return m_xCandidateData.GetPowerPercentage();
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

    public PopularityModifier(float fEffect, Orientation eOrientation, int iRemovalTime, string xDescription, bool bTimesOut=true)
    {
        m_fEffect = fEffect;
        m_eOrientation = eOrientation;
        m_iRemovalTurn = Manager.GetTurnNumber()+iRemovalTime;
        m_xDescription = xDescription;
        m_bTimesOut = bTimesOut;
    }

    public virtual bool ShouldRemove()
    {
        return m_bTimesOut && Manager.GetTurnNumber() > m_iRemovalTurn;
    }
    public virtual void OnNextTurn() {}

    public virtual void OnRemoval() {}

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