using UnityEngine;

public class GovernmentValuesContainer : MonoBehaviour
{
    [SerializeField]
    GovernmentValues m_xGovValues;
    static GovernmentValuesContainer s_xInstance;

    public static GovernmentValues GetGovernmentValues()
    {
        // TODO: in awake, count instances to ensure there is only one
        if (s_xInstance == null)
        {
            s_xInstance = FindObjectOfType(typeof(GovernmentValuesContainer)) as GovernmentValuesContainer;
        }
        return s_xInstance.m_xGovValues;
    }
}

[System.Serializable]
public class GovernmentValues : OrganisationValuesBase
{
    [SerializeField]
    float m_fGrowthCostLinearFactor = 4.5f;
    [SerializeField]
    float m_fTotalCostToLevelUpLinearFactor = 10f;

    public override float GetCostsAtLevel(int iLevel)
    {
        return m_fGrowthCostLinearFactor * iLevel;
    }

    public override float GetLevelUpRequirementAtLevel(int iLevel)
    {
        return m_fTotalCostToLevelUpLinearFactor * iLevel;
    }


    public int CalculateLevelPlateau(float fIncome)
    {
        int i = 1;
        while (i < 1000)
        {
            if (fIncome < GetCostsAtLevel(i))
            {
                return i;
            }
            i++;
        }
        return 1000;
    }

}
