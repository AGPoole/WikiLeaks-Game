using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SystemValuesBase
{
    [System.Serializable]
    public class LevelRequirements
    {
        [SerializeField]
        int m_iCost;
        [SerializeField]
        int m_iLevelRequirement = 0;
        [SerializeField]
        int m_iTechRequirement = 0;

        public virtual bool CanLevelUp(int iLevel)
        {
            return (m_iLevelRequirement <= iLevel);
        }

        public virtual int GetCost()
        {
            return m_iCost;
        }
    }

    [SerializeField]
    LevelRequirements[] m_xLevelRequirements;

    [SerializeField]
    protected int m_iBaseDefenceMax = 10;
    [SerializeField]
    protected float m_fBaseDefenceRefreshTime = 0.1f;
    [SerializeField]
    protected float m_fAdditionalShielddeteriorationTime = 0.1f;

    [SerializeField]
    List<GameObject> m_xMandatoryPerks;

    [SerializeField]
    List<GameObject> m_xOptionalPerks;
    [SerializeField]
    int m_iNumOptional = 1;

    [Range(0, 1)]
    [SerializeField]
    float m_fTripWireExistanceProbability = 0.1f;
    [SerializeField]
    float m_fTripWireCatchProbability = 0.25f;
    [SerializeField]
    int m_iTripWireDamage = 25;

    [SerializeField]
    int m_iTripWireUpdateTime = 50;

    public int GetLevelUpCost(int iCurrentLevel)
    {
        return m_xLevelRequirements[iCurrentLevel].GetCost();
    }
    public bool CanLevelUp(int iCurrentLevel)
    {
        return m_xLevelRequirements[iCurrentLevel].CanLevelUp(iCurrentLevel);
    }

    public int GetMaxLevel()
    {
        return m_xLevelRequirements.Length;
    }

    public int GetBaseDefenceMax()
    {
        return m_iBaseDefenceMax;
    }
    public float GetBaseDefenceRefreshTime()
    {
        return m_fBaseDefenceRefreshTime;
    }
    public float GetAdditionalShielddeteriorationTime()
    {
        return m_fAdditionalShielddeteriorationTime;
    }
    public void SetUpUIPerks(SystemUI xUI)
    {
        foreach(var xPerk in m_xMandatoryPerks)
        {
            xUI.AddPerk(xPerk);
        }

        // Reservoir sampling
        // TODO: do this in O(subset) rather than O(whole-list)
        int iNumToAdd = m_iNumOptional;
        for(int iNumLeft=m_xOptionalPerks.Count; iNumLeft>0; iNumLeft--)
        {
            if (Random.Range(0f, 1f) < (float)iNumToAdd / (float)iNumLeft)
            {
                iNumToAdd--;
                xUI.AddPerk(m_xOptionalPerks[iNumLeft - 1]);
            }
        }
    }

    public float GetTripWireExistanceProbability()
    {
        return m_fTripWireExistanceProbability;
    }
    public float GetTripWireCatchProbability()
    {
        return m_fTripWireCatchProbability;
    }
    public int GetTripWireDamage()
    {
        return m_iTripWireDamage;
    }

    public int GetTripWireUpdateTime()
    {
        return m_iTripWireUpdateTime;
    }
}
