using UnityEngine;

public class AntiVirusValuesContainer : MonoBehaviour
{
    static AntiVirusValuesContainer s_xInstance;
    [SerializeField]
    AntiVirusValues m_xValues;
    public static AntiVirusValues GetAntiVirusValues()
    {
        if (s_xInstance == null)
        {
            s_xInstance = FindObjectOfType<AntiVirusValuesContainer>() as AntiVirusValuesContainer;
        }
        return s_xInstance.m_xValues;
    }
}

[System.Serializable]
public class AntiVirusValues : SystemValuesBase
{
    [SerializeField]
    int m_iLaunchTime;
    [SerializeField]
    float m_fDaamageGainPerLevel;
    [SerializeField]
    GameObject m_xMessagePrefab;
    [SerializeField]
    float[] m_afTechDamageBonuses;
    [SerializeField]
    float m_fRange = 30;

    public int GetDamageAtLevel(int iLevel, int iTechLevel)
    {
        float fTechGain = iTechLevel < m_afTechDamageBonuses.Length ?
            m_afTechDamageBonuses[iTechLevel] :
            m_afTechDamageBonuses[m_afTechDamageBonuses.Length - 1];
        return (int)(m_fDaamageGainPerLevel * iLevel + fTechGain);
    }

    public int GetLaunchTime()
    {
        return m_iLaunchTime;
    }

    public GameObject GetMessagePrefab()
    {
        return m_xMessagePrefab;
    }

    public float GetRange()
    {
        return m_fRange;
    }
}