using UnityEngine;

public class CensorshipValuesContainer : MonoBehaviour
{
    static CensorshipValuesContainer s_xInstance;
    [SerializeField]
    CensorshipValues m_xValues;
    public static CensorshipValues GetCensorshipValues()
    {
        if (s_xInstance == null)
        {
            s_xInstance = FindObjectOfType<CensorshipValuesContainer>() as CensorshipValuesContainer;
        }
        return s_xInstance.m_xValues;
    }
}

[System.Serializable]
public class CensorshipValues : SystemValuesBase
{
    [SerializeField]
    float m_fRatioRequirement;
    [SerializeField]
    [Range(0, 1)]
    float m_fMessageRate;
    [SerializeField]
    string[] m_xMessages;

    public float GetRatioRequirement()
    {
        return m_fRatioRequirement;
    }

    public void UseNotification()
    {
        if (Random.Range(0f, 1f) < m_fMessageRate)
        {
            string[] xStrings = m_xMessages;
            NotificationSystem.AddNotification(xStrings[Random.Range(0, xStrings.Length)]);
        }
    }

    [System.Serializable]
    class LevelRequirementsDeprecated
    {
        [SerializeField]
        public int m_iLevelUp;
        [SerializeField]
        public int m_iLevelDown;
        [SerializeField]
        public int m_iTechRequirement;
    }
}
