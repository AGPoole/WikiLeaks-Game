using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    protected float m_fBaseDefenceMax = 10;
    [SerializeField]
    protected float m_fBaseDefenceRefreshRate = 0.1f;
    [SerializeField]
    protected float m_fAdditionalShielddeteriorationRate = 0.1f;
    [SerializeField]
    protected float m_fAdditionalShieldsMax = 5;

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

    public float GetBaseDefenceMax()
    {
        return m_fBaseDefenceMax;
    }
    public float GetBaseDefenceRefreshRate()
    {
        return m_fBaseDefenceRefreshRate;
    }
    public float GetAdditionalShielddeteriorationRate()
    {
        return m_fAdditionalShielddeteriorationRate;
    }
    public float GetAdditionalShieldsMax()
    {
        return m_fAdditionalShieldsMax;
    }
}
