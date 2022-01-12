using UnityEngine;

public class HQValuesContainer : MonoBehaviour
{
    static HQValuesContainer s_xInstance;
    [SerializeField]
    HQValues m_xValues;
    public static HQValues GetHQValues()
    {
        if (s_xInstance == null)
        {
            s_xInstance = FindObjectOfType<HQValuesContainer>() as HQValuesContainer;
        }
        return s_xInstance.m_xValues;
    }
}

[System.Serializable]
public class HQValues : SystemValuesBase
{
}