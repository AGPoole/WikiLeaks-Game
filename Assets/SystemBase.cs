using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SystemBase : MonoBehaviour
{
    static List<SystemBase> s_xAllSystems;
    [SerializeField]
    SystemUI m_xUI;
    [SerializeField]
    protected int m_iLevel = 0;
    [SerializeField]
    bool m_bHacked = false;

    protected OrganisationBase m_xOwner;

    protected int m_iLevelChangeTimer;

    protected int m_iHackLevel = 0;

    // TODO: this shouldn't be a float
    protected float m_fDefences = 0;

    protected List<SystemBase> m_axConnectedSystems;

    protected virtual void Start()
    {
        if (s_xAllSystems == null)
        {
            s_xAllSystems = new List<SystemBase>();
        }
        if (m_xOwner == null)
        {
            m_xOwner = transform.parent.GetComponent<OrganisationBase>();
            if (m_xOwner == null)
            {
                Debug.LogError("Incorrect object tree to find parent");
            }
        }
        SetLevel(m_iLevel, GetDefaultTimer());
        m_fDefences = GetMyValues().GetBaseDefenceMax();
        if(m_xUI == null)
        {
            Debug.LogError(string.Format("Missing UI object on {0}", gameObject.name));
            return;
        }
        GetMyValues().SetUpUIActions(m_xUI);
        s_xAllSystems.Add(this);

        m_axConnectedSystems = new List<SystemBase>();
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
    public bool IsHackable() 
    {
        foreach(SystemBase xSys in m_axConnectedSystems)
        {
            if (xSys.IsHacked())
            {
                return true;
            }
        }
        return false;
    }
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

    public void SetOwner(OrganisationBase xOwner)
    {
        m_xOwner = xOwner;
    }

    public OrganisationBase GetOwner()
    {
        return m_xOwner;
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
            Hack();
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

    protected virtual void Hack()
    {
        m_bHacked = true;
        foreach(SystemBase xSys in m_axConnectedSystems)
        {
            if (xSys.m_bHacked)
            {
                Vertex.GetConnection(this, xSys).Hack();
            }
        }
    }

    public static void SetUpVertices()
    {
        for(int i = 0; i<s_xAllSystems.Count; i++)
        {
            for (int j = i+1; j < s_xAllSystems.Count; j++)
            {
                bool bShouldConnected = ShouldBeConnected(s_xAllSystems[i], s_xAllSystems[j]);
                Vertex xVert = Vertex.GetConnection(s_xAllSystems[i], s_xAllSystems[j]);
                if (bShouldConnected && xVert==null)
                {
                    GameObject xGameObject = Instantiate(Manager.GetManager().GetVertexPrefabGameObject());
                    xGameObject.GetComponent<Vertex>().SetEndPoints(s_xAllSystems[i], s_xAllSystems[j]);
                    s_xAllSystems[i].SetConnected(s_xAllSystems[j], true);
                    s_xAllSystems[j].SetConnected(s_xAllSystems[i], true);
                }
                else if (!bShouldConnected && xVert!=null)
                {
                    Destroy(xVert.gameObject);
                    s_xAllSystems[i].SetConnected(s_xAllSystems[j], false);
                    s_xAllSystems[j].SetConnected(s_xAllSystems[i], false);
                }
            }
        }
    }

    public void SetConnected(SystemBase xSys, bool bConnected)
    {
        if (bConnected)
        {
            if (!m_axConnectedSystems.Contains(xSys))
            {
                m_axConnectedSystems.Add(xSys);
            }
            else
            {
                Debug.LogError("Connecting same system twice");
            }
        }
        else
        {
            if (m_axConnectedSystems.Contains(xSys))
            {
                m_axConnectedSystems.Remove(xSys);
            }
            else
            {
                Debug.LogError("Removing same system twice");
            }
        }
    }

    public bool IsConnected(SystemBase xSys)
    {
        return m_axConnectedSystems.Contains(xSys);
    }

    public static bool ShouldBeConnected(SystemBase xSystem1, SystemBase xSystem2)
    {
        return (xSystem1.transform.position - xSystem2.transform.position).magnitude < Manager.GetManager().GetConnectionRange();
    }

    void OnDestroy()
    {
        s_xAllSystems.Remove(this);
    }
}
