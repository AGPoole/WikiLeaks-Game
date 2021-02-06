using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SystemBase : MonoBehaviour
{
    [SerializeField]
    protected UnityEngine.UI.Text m_xTitleText;
    [SerializeField]
    protected int m_iLevel = 0;
    [SerializeField]
    protected UnityEngine.UI.Text m_xLevelText;
    [SerializeField]
    protected UnityEngine.UI.Text m_xDefencesText;
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
    }

    protected virtual void Update()
    {
        if (GetLevel() > 0f)
        {
            m_xTitleText.color = m_bHacked ? Color.green : Color.white;
        }
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
        m_xDefencesText.text = m_fDefences.ToString("0");
    }

    public int GetLevel() { return m_iLevel; }
    protected void SetLevel(int iLevel) 
    {
        SetLevel(iLevel, GetDefaultTimer());
    }
    protected virtual void SetLevel(int iLevel, int iTimer)
    {
        if(iLevel <= 0)
        {
            iLevel = 0;
            OnLevelReachesZero();
        }
        if(iLevel > GetMaxLevel())
        {
            iLevel = GetMaxLevel();
        }
        if(iLevel==1 && m_iLevel == 0)
        {
            OnLevelReachesOne();
        }
        m_iLevel = iLevel;
        Color c = m_xTitleText.color;
        m_xTitleText.color = new Color(c.r, c.g, c.b, m_iLevel > 0 ? 1f : 0.4f);
        m_xLevelText.text = m_iLevel.ToString();
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

    protected virtual void OnLevelReachesZero() 
    {
        m_xLevelText.gameObject.SetActive(false);
        m_xDefencesText.gameObject.SetActive(false);
    }
    protected virtual void OnLevelReachesOne() 
    {
        m_xLevelText.gameObject.SetActive(true);
        m_xDefencesText.gameObject.SetActive(true);
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
