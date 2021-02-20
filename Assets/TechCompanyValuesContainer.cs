using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class TechCompanyValuesContainer : MonoBehaviour
{
    [SerializeField]
    TechCompanyValues m_xTechCompanyValues;
    static TechCompanyValuesContainer s_xInstance;

    public static TechCompanyValues GetTechCompanyValues()
    {
        if (s_xInstance == null)
        {
            s_xInstance = FindObjectOfType(typeof(TechCompanyValuesContainer)) as TechCompanyValuesContainer;
        }
        return s_xInstance.m_xTechCompanyValues;
    }

    #if (UNITY_EDITOR)
    [ContextMenu("Run Calculations")]
    void RunCalculations()
    {
        var assembly = Assembly.GetAssembly(typeof(SceneView));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);

        int iMaxCorpLevel = 0;
        float fBestCorpTaxLevel = 0f;
        int iMaxGovLevel = 0;
        float fBestGovTaxLevel = 0f;
        float fMaxHappiness = 0f;
        float fBestHappinessTax = 0f;
        for (float fTax = 0.05f; fTax <= 0.85f; fTax += 0.01f)
        {
            var xTechCompValues = TechCompanyValuesContainer.GetTechCompanyValues();
            int iCorpLevel = xTechCompValues.CalculateLevelPlateau(fTax);
            float fGov = xTechCompValues.GetProfitAtLevel(xTechCompValues.CalculateLevelPlateau(fTax)) * fTax;
            int iGovLevel = GovernmentValuesContainer.GetGovernmentValues().CalculateLevelPlateau(fGov);

            if (iCorpLevel > iMaxCorpLevel)
            {
                iMaxCorpLevel = iCorpLevel;
                fBestCorpTaxLevel = fTax;
            }
            if (iGovLevel > iMaxGovLevel)
            {
                iMaxGovLevel = iGovLevel;
                fBestGovTaxLevel = fTax;
            }
            float fHappiness = Manager.CalculateHappiness(iGovLevel, iCorpLevel);
            if (fHappiness > fMaxHappiness)
            {
                fMaxHappiness = fHappiness;
                fBestHappinessTax = fTax;
            }

            Debug.LogFormat("Tax Rate: {0} Level Plateau: {1} Gov Money: {2} Gov Plateau: {3} Happiness: {4} Diff: {5}", fTax.ToString("0.00"),
                iCorpLevel, fGov.ToString("0.00"),
                iGovLevel.ToString(), fHappiness.ToString("0.00"),
                (iCorpLevel - iGovLevel).ToString());
        }
        Debug.LogFormat("The best tax rate for corporations was {0}, giving a max level of {1}", fBestCorpTaxLevel.ToString("0.00"), iMaxCorpLevel.ToString());
        Debug.LogFormat("The best tax rate for happiness was {0}, giving a max happiness of {1}", fBestHappinessTax.ToString("0.00"), fMaxHappiness.ToString("0.00"));
        Debug.LogFormat("The best tax rate for governments was {0}, giving a max level of {1}", fBestGovTaxLevel.ToString("0.00"), iMaxGovLevel.ToString());

        if (fBestCorpTaxLevel > fBestHappinessTax - 0.05f || fBestHappinessTax + 0.05f > fBestGovTaxLevel)
        {
            Debug.LogError("Incorrect ordering of scores");
        }
    }
    #endif
}

[System.Serializable]
public class TechCompanyValues : OrganisationValuesBase
{
    [SerializeField]
    float m_fGrowthCostLinearFactor = 10f;
    [SerializeField]
    float m_fProfitConstantFactor = 100f;
    [SerializeField]
    float m_fProfitLinearFactor = 50f;
    [SerializeField]
    float m_fProfitExponentialDecreaseFactor = 0.98f;
    [SerializeField]
    float m_fTotalCostToLevelUpLinearFactor = 30f;
    [SerializeField]
    float m_fLevelFactor = 1f;
    [SerializeField]
    float m_fShareStdDeviation = 0.3f;
    [SerializeField]
    float m_fShareMin = 1f;
    [SerializeField]
    float m_fShareMax = 100f;

    public override float GetCostsAtLevel(int iLevel)
    {
        return m_fGrowthCostLinearFactor * iLevel * m_fLevelFactor;
    }

    public float GetProfitAtLevel(int iLevel)
    {
        return m_fProfitConstantFactor + (m_fProfitLinearFactor * iLevel * m_fLevelFactor * Mathf.Pow(m_fProfitExponentialDecreaseFactor, iLevel * m_fLevelFactor));
    }
    public override float GetLevelUpRequirementAtLevel(int iLevel)
    {
        return m_fTotalCostToLevelUpLinearFactor * iLevel * m_fLevelFactor;
    }

    public int CalculateLevelPlateau(float fTax)
    {
        int i = 1;
        while (i < 1000)
        {
            if (GetProfitAtLevel(i) * (1 - fTax) < GetCostsAtLevel(i))
            {
                return i;
            }
            i++;
        }
        return 1000;
    }

    public float GetShareStdDev()
    {
        return m_fShareStdDeviation;
    }

    public float GetShareMin()
    {
        return m_fShareMin;
    }
    public float GetShareMax()
    {
        return m_fShareMax;
    }
}