using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SystemBase : MonoBehaviour
{
    [SerializeField]
    SystemUI m_xUI;
    [SerializeField]
    protected int m_iLevel = 0;
    [SerializeField]
    bool m_bHacked = false;

    protected int m_iLevelChangeTimer;

    protected int m_iHackLevel = 0;

    // TODO: this shouldn't be a float
    protected float m_fDefences = 0;

    protected virtual void Start()
    {
        SetLevel(m_iLevel, GetDefaultTimer());
        m_fDefences = GetMyValues().GetBaseDefenceMax();
        if(m_xUI == null)
        {
            Debug.LogError(string.Format("Missing UI object on {0}", gameObject.name));
            return;
        }
        GetMyValues().SetUpUIActions(m_xUI);
    }

    protected virtual void Update()
    {
        
    }
    public virtual void OnNextTurn(int iOwnerLevel)
    {
        if (m_fDefences < GetMyValues().GetBaseDefenceMax())
        {
            m_fDefences = Mathf.Min(m_fDefences + GetMyValues().GetBaseDefenceRefreshRate(), GetMyValues().GetBaseDefenceMax());
        }else if(m_fDefences > GetMyValues().GetBaseDefenceMax())
        {
            m_fDefences = Mathf.Max(m_fDefences - GetMyValues().GetAdditionalShielddeteriorationRate(), GetMyValues().GetBaseDefenceMax());
        }
    }

    public bool IsHacked() { return m_bHacked; }
    public int GetLevel() { return m_iLevel; }
    public float GetDefences() { return m_fDefences; }
    protected void SetLevel(int iLevel) 
    {
        SetLevel(iLevel, GetDefaultTimer());
    }
    protected virtual void SetLevel(int iLevel, int iTimer)
    {
        if(iLevel <= 0)
        {
            iLevel = 0;
            OnDeactivation();
        }
        if(iLevel > GetMaxLevel())
        {
            iLevel = GetMaxLevel();
        }
        if(iLevel==1 && m_iLevel == 0)
        {
            OnActivation();
        }
        m_iLevel = iLevel;

        m_iLevelChangeTimer = iTimer;
    }

    public void LevelDown()
    {
        if (m_iLevel == 0)
        {
            return;
        }
        SetLevel(m_iLevel - 1);
    }
    public void LevelUp()
    {
        if (m_iLevel == GetMaxLevel())
        {
            return;
        }
        SetLevel(m_iLevel + 1);
    }

    public virtual void ModifyLevel(int iModifier)
    {
        SetLevel(m_iLevel + iModifier, GetDefaultTimer());
    }

    //TODO: change this
    public virtual void ModifyLevel(int iModifier, int iTimer)
    {
        SetLevel(m_iLevel + iModifier, m_iLevelChangeTimer + iTimer);
    }

    protected int GetMaxLevel()
    {
        return GetMyValues().GetMaxLevel();
    }

    protected int GetDefaultTimer()
    {
        return 1;
    }

    public int GetLevelUpCost()
    {
        if (m_iLevel == GetMaxLevel())
        {
            return int.MaxValue;
        }
        return GetMyValues().GetLevelUpCost(m_iLevel);
    }
    public int GetCurrentCost()
    {
        if (m_iLevel > 0)
        {
            return GetMyValues().GetLevelUpCost(m_iLevel-1);
        }
        else
        {
            return 0;
        }
    }

    protected virtual void OnDeactivation() 
    {
        m_xUI.OnDeactivation();
    }
    protected virtual void OnActivation() 
    {
        m_xUI.OnActivation();
    }
    protected abstract SystemValuesBase GetMyValues();

    public void Attack()
    {
        m_fDefences = Mathf.Max(m_fDefences - 1, 0);
        if (Mathf.Approximately(m_fDefences, 0f))
        {
            m_bHacked = true;
        }
    }

    public void Defend()
    {
        m_fDefences = Mathf.Min(m_fDefences + 1, GetMyValues().GetBaseDefenceMax() + GetMyValues().GetAdditionalShieldsMax());
    }

    public bool GetIsHacked()
    {
        return m_bHacked;
    }
}
