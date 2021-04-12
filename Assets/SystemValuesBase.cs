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
    protected int m_iBaseDefenceMax = 10;
    [SerializeField]
    protected float m_fBaseDefenceRefreshTime = 0.1f;
    [SerializeField]
    protected float m_fAdditionalShielddeteriorationTime = 0.1f;

    [SerializeField]
    List<GameObject> m_xActions;

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
    public void SetUpUIActions(SystemUI xUI)
    {
        foreach(var xAction in m_xActions)
        {
            xUI.AddAction(xAction);
        }
    }
}
