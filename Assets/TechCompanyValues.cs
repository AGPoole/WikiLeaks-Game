using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class TechCompanyValues : MonoBehaviour
{
    [SerializeField]
    float m_fGrowthCostLinearFactor = 10f;
    [SerializeField]
    float m_fProfitConstantFactor = 100f;
    [SerializeField]
    float m_fProfitLinearFactor = 30f;
    [SerializeField]
    float m_fProfitExponentialDecreaseFactor = 0.95f;
    [SerializeField]
    float m_fTotalCostToLevelUpLinearFactor = 300f;
    [SerializeField]
    float m_fLevelFactor = 2f;

    static TechCompanyValues s_xStaticInstance;

    static TechCompanyValues GetTechCompanyValues()
    {
        // WSTODO: in awake, count instances to ensure there is only one
        if (s_xStaticInstance==null)
        {
            s_xStaticInstance = FindObjectOfType(typeof(TechCompanyValues)) as TechCompanyValues;
        }
        return s_xStaticInstance;
    }

    public static float GetLevelUpCostAtLevel(int iLevel)
    {
        return GetTechCompanyValues().m_fGrowthCostLinearFactor * iLevel*GetTechCompanyValues().m_fLevelFactor;
    }

    public static float GetProfitAtLevel(int iLevel)
    {
        s_xStaticInstance = GetTechCompanyValues();
        return s_xStaticInstance.m_fProfitConstantFactor + (s_xStaticInstance.m_fProfitLinearFactor * iLevel * GetTechCompanyValues().m_fLevelFactor * Mathf.Pow(s_xStaticInstance.m_fProfitExponentialDecreaseFactor, iLevel * GetTechCompanyValues().m_fLevelFactor));
    }
    public static float GetTotalRequirementAtLevel(int iLevel)
    {
        return GetTechCompanyValues().m_fTotalCostToLevelUpLinearFactor * iLevel * GetTechCompanyValues().m_fLevelFactor;
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
            int iCorpLevel = TechCompanyValues.CalculateLevelPlateau(fTax);
            float fGov = TechCompanyValues.GetProfitAtLevel(CalculateLevelPlateau(fTax)) * fTax;
            int iGovLevel = GovernmentValues.CalculateLevelPlateau(fGov);

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
    public static int CalculateLevelPlateau(float fTax)
    {
        int i = 1;
        while (i < 1000)
        {
            if (TechCompanyValues.GetProfitAtLevel(i) * (1 - fTax) < TechCompanyValues.GetLevelUpCostAtLevel(i))
            {
                return i;
            }
            i++;
        }
        return 1000;
    }
}
