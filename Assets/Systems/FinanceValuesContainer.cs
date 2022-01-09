using UnityEngine;

public class FinanceValuesContainer : MonoBehaviour
{
    static FinanceValuesContainer s_xInstance;
    [SerializeField]
    FinanceValues m_xValues;
    public static FinanceValues GetFinanceValues()
    {
        if (s_xInstance == null)
        {
            s_xInstance = FindObjectOfType<FinanceValuesContainer>() as FinanceValuesContainer;
        }
        return s_xInstance.m_xValues;
    }
}

[System.Serializable]
public class FinanceValues : SystemValuesBase
{

}
