using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GovernmentValues : MonoBehaviour
{
    [SerializeField]
    float m_fGrowthCostLinearFactor = 6f;
    [SerializeField]
    float m_fTotalCostToLevelUpLinearFactor = 30f;

    static GovernmentValues s_xStaticInstance;

    static GovernmentValues GetGovernmentValues()
    {
        // WSTODO: in awake, count instances to ensure there is only one
        if (s_xStaticInstance == null)
        {
            s_xStaticInstance = FindObjectOfType(typeof(GovernmentValues)) as GovernmentValues;
        }
        return s_xStaticInstance;
    }

    public static float GetLevelUpCostAtLevel(int iLevel)
    {
        return GetGovernmentValues().m_fGrowthCostLinearFactor * iLevel;
    }

    public static float GetTotalRequirementAtLevel(int iLevel)
    {
        return GetGovernmentValues().m_fTotalCostToLevelUpLinearFactor * iLevel;
    }


    public static int CalculateLevelPlateau(float fIncome)
    {
        int i = 1;
        while (i < 1000)
        {
            if (fIncome < GovernmentValues.GetLevelUpCostAtLevel(i))
            {
                return i;
            }
            i++;
        }
        return 1000;
    }
}

//public abstract class Action<T>
//{
//    public bool Attempt(T t)
//    {
//        if (Random.Range(0, 1) < GetProbability(t))
//        {
//            Perform(t);
//            return true;
//        }
//        return false;
//    }

//    protected abstract float GetProbability(T t);

//    protected abstract void Perform(T t);
//}

//public class GovernmentBiasAction : Action<Government>
//{
//    protected override float GetProbability(Government xGov)
//    {
//        if (xGov.GetTimeTillNextElection() > 30)
//        {
//            return 0f;
//        }
//        if (xGov.GetGovernmentData().GetSize() < 80)
//        {
//            return 0f;
//        }
//        return 1;
//    }

//    protected override void Perform(Government xGov)
//    {
//        throw new System.NotImplementedException();
//    }
//}
