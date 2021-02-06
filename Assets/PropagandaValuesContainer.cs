using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropagandaValuesContainer : MonoBehaviour
{
    public enum ObjectType
    {
        Company,
        Government
    }

    [SerializeField]
    PropagandaValues m_xGovValues;
    [SerializeField]
    PropagandaValues m_xCorpValues;

    static PropagandaValuesContainer s_xInstance;

    public static PropagandaValuesContainer GetPropagandaValuesContainer()
    {
        if (s_xInstance == null)
        {
            s_xInstance = FindObjectOfType<PropagandaValuesContainer>() as PropagandaValuesContainer;
        }
        return s_xInstance;
    }

    public static PropagandaValues GetPropagandaValues(ObjectType eType)
    {
        switch (eType)
        {
            case ObjectType.Company:
                return GetPropagandaValuesContainer().m_xCorpValues;
            default:
            case ObjectType.Government:
                return GetPropagandaValuesContainer().m_xGovValues;
        }

    }
}
