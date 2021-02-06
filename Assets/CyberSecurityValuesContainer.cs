using System.Collections;
using System.Collections.Generic;
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
    int m_iLaunchTime;
    [SerializeField]
    float m_fDefenceGainPerLevel;
    [SerializeField]
    GameObject m_xMessagePrefab;
    [SerializeField]
    int[] m_aiScoreBoundaries;

    public float GetDefenceGainAtLevel(int iLevel)
    {
        return m_fDefenceGainPerLevel * iLevel;
    }

    public int GetLaunchTime()
    {
        return m_iLaunchTime;
    }

    public GameObject GetMessagePrefab()
    {
        return m_xMessagePrefab;
    }

    public int GetScoreAtDistance(float fDistance)
    {
        int iIndex = 0;
        while (iIndex < m_aiScoreBoundaries.Length)
        {
            if (fDistance < m_aiScoreBoundaries[iIndex])
            {
                return m_aiScoreBoundaries.Length - iIndex;
            }
            iIndex++;
        }
        return 0;
    }
}
