using UnityEngine;

public class CyberSecurityValuesContainer : MonoBehaviour
{
    static CyberSecurityValuesContainer s_xInstance;
    [SerializeField]
    CyberSecurityValues m_xValues;
    public static CyberSecurityValues GetCyberSecurityValues()
    {
        if (s_xInstance == null)
        {
            s_xInstance = FindObjectOfType<CyberSecurityValuesContainer>() as CyberSecurityValuesContainer;
        }
        return s_xInstance.m_xValues;
    }
}

[System.Serializable]
public class CyberSecurityValues : SystemValuesBase
{
    [SerializeField]
    float[] m_afDefenceForSystemLevels;
    [SerializeField]
    float[] m_afDefenceForTechLevels;
    // Each multiplier is for the next hexagon along
    // TODO: wrte a hexagon-distance function
    [SerializeField]
    float[] m_afDefenceMultiplierForDistances;

    [SerializeField]
    float m_fAdditionalDefenceDegradationTime = 0.5f;
    [SerializeField]
    float m_fDefenceRechargeTime = 0.5f;

    public int GetDefenceGainAtLevelForDistance(int iLevel, int iTechLevel, float fDistance)
    {
        if (iLevel == 0)
        {
            return 0;
        }
        int iNumHexes = (int)(fDistance / Manager.GetManager().GetHexagonEdgeSize());
        float fDistanceMultiplier = iNumHexes < m_afDefenceMultiplierForDistances.Length ?
            m_afDefenceMultiplierForDistances[iNumHexes] :
            0;
        float fTechMultiplier = iTechLevel < m_afDefenceForTechLevels.Length ?
            m_afDefenceForTechLevels[iTechLevel] :
            m_afDefenceForTechLevels[m_afDefenceForTechLevels.Length - 1];
        float fStandard = iLevel < m_afDefenceForSystemLevels.Length ?
            m_afDefenceForSystemLevels[iLevel] :
            m_afDefenceForSystemLevels[m_afDefenceForSystemLevels.Length - 1];
        return (int)(fDistanceMultiplier * fTechMultiplier * fStandard);
    }

    public float GetMaxLength()
    {
        return m_afDefenceMultiplierForDistances.Length * Manager.GetManager().GetHexagonEdgeSize();
    }

    public float GetAdditionalDefenceDegradationTime()
    {
        return m_fAdditionalDefenceDegradationTime;
    }

    public float GetDefenceRechargeTime()
    {
        return m_fDefenceRechargeTime;
    }
}
