using UnityEngine;

public class DataCollectionValuesContainer : MonoBehaviour
{
    static DataCollectionValuesContainer s_xInstance;
    [SerializeField]
    DataCollectionValues m_xValues;
    public static DataCollectionValues GetDataCollectionValues()
    {
        if (s_xInstance == null)
        {
            s_xInstance = FindObjectOfType<DataCollectionValuesContainer>() as DataCollectionValuesContainer;
        }
        return s_xInstance.m_xValues;
    }
}

[System.Serializable]
public class DataCollectionValues : SystemValuesBase
{
}