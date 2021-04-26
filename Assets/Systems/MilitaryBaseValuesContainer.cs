using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilitaryBaseValuesContainer : MonoBehaviour
{
    static MilitaryBaseValuesContainer s_xInstance;
    [SerializeField]
    MilitaryBaseValues m_xValues;
    public static MilitaryBaseValues GetMilitaryBaseValues()
    {
        if (s_xInstance == null)
        {
            s_xInstance = FindObjectOfType<MilitaryBaseValuesContainer>() as MilitaryBaseValuesContainer;
        }
        return s_xInstance.m_xValues;
    }
}

[System.Serializable]
public class MilitaryBaseValues : SystemValuesBase
{
}