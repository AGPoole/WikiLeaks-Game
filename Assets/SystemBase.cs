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
    [SerializeField]
    int m_iXPosInGrid = 0;
    [SerializeField]
    int m_iYPosInGrid = 0;

    protected OrganisationBase m_xOwner;

    protected int m_iLevelChangeTimer;

    protected int m_iHackLevel = 0;

    // TODO: this shouldn't be a float
    protected float m_fDefences = 0;

    protected List<Vertex> m_xVertices;

    List<GameObject> m_xHexagonLineRenderers;

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

        m_xVertices = new List<Vertex>();
        for(int i=0; i<6; i++)
        {
            m_xVertices.Add(null);
        }

        transform.position = Manager.GetManager().GetPositionFromGridCoords(m_iXPosInGrid, m_iYPosInGrid);

        m_xHexagonLineRenderers = new List<GameObject>();
        // Use 7 to loop to start
        for(int i = 0; i<6; i++)
        {
            GameObject xLineRenderer = Instantiate(Manager.GetManager().GetLineRendererPrefabGameObject());
            const float fAngleDifference = Mathf.PI / 3;
            const float fStartingAngle = -Mathf.PI / 6;
            float fAngle1 = fStartingAngle + (i * fAngleDifference);
            float fAngle2 = fStartingAngle + ((i+1) * fAngleDifference);
            Vector3 xPos1 = transform.position + (new Vector3(Mathf.Sin(fAngle1), Mathf.Cos(fAngle1), 0f) * Manager.GetManager().GetHexagonEdgeSize());
            Vector3 xPos2 = transform.position + (new Vector3(Mathf.Sin(fAngle2), Mathf.Cos(fAngle2), 0f) * Manager.GetManager().GetHexagonEdgeSize());

            xLineRenderer.GetComponent<LineRenderer>().positionCount = 2;
            xLineRenderer.GetComponent<LineRenderer>().SetPositions(new Vector3[] { xPos1, xPos2 });

            m_xHexagonLineRenderers.Add(xLineRenderer);
        }
    }

    protected virtual void Update()
    {
        transform.position = Manager.GetManager().GetPositionFromGridCoords(m_iXPosInGrid, m_iYPosInGrid);

        for(int i=0; i<6; i++)
        {
            SystemBase xSys = Manager.GetAdjacentSystem(m_iXPosInGrid, m_iYPosInGrid, (Manager.GridDirection)i);
            bool bEnabled = xSys==null || xSys.m_xOwner!=m_xOwner;
            m_xHexagonLineRenderers[i].SetActive(bEnabled);
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
    }

    public bool IsHacked() { return m_bHacked; }
    public bool IsHackable() 
    {
        for (int i = 0; i < 6; i++)
        {
            SystemBase xSys = GetConnectedSystem((Manager.GridDirection)i);
            if (xSys != null && xSys.m_bHacked)
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

    public void Attack(bool bIsPlayer, int iDamage=1, bool bAttackingPlayer=false)
    {
        if (bIsPlayer)
        {
            if (Manager.GetManager().GetHacksLeft() <= iDamage)
            {
                return;
            }
            Manager.GetManager().ChangeHacks(-iDamage);
        }
        m_fDefences = Mathf.Max(m_fDefences - iDamage, 0);
        if (Mathf.Approximately(m_fDefences, 0f))
        {
            if (bAttackingPlayer)
            {
                UnHack();
            }
            else
            {
                Hack();
            }
        }
    }
    public void Defend()
    {
        m_fDefences = Mathf.Min(m_fDefences + 1, GetMyValues().GetBaseDefenceMax() + GetMyValues().GetAdditionalShieldsMax());
    }
    
    // TODO: make private
    public virtual void Hack()
    {
        m_bHacked = true;
        for(int i=0; i<6; i++)
        {
            SystemBase xSys = GetConnectedSystem((Manager.GridDirection)i);
            if (xSys!=null && xSys.m_bHacked)
            {
                m_xVertices[i].Hack();
            }
        }
    }

    protected virtual void UnHack()
    {
        m_bHacked = false;
        for (int i = 0; i < 6; i++)
        {
            if (m_xVertices[i] != null)
            {
                m_xVertices[i].UnHack();
            }
        }
    }

    public static void SetUpVertices()
    {
        for(int i = 0; i<s_xAllSystems.Count; i++)
        {
            SystemBase xSys1 = s_xAllSystems[i];
            for (int iDir = 0; iDir < 6; iDir++)
            {
                int iOpposite = (int)Manager.GetOppositeDirection((Manager.GridDirection)iDir);
                SystemBase xSys2 = Manager.GetAdjacentSystem(xSys1.m_iXPosInGrid, xSys1.m_iYPosInGrid, (Manager.GridDirection)iDir);
                Vertex xVert1 = xSys1 != null ? xSys1.m_xVertices[iDir] : null;
                Vertex xVert2 = xSys2!=null ? xSys2.m_xVertices[iOpposite] : null;
                if (xVert1 != null && !xVert1.Contains(xSys2))
                {

                    Debug.LogFormat("({0} {1}) ({2} {3}) {4} ", xVert1.GetStart().m_iXPosInGrid, xVert1.GetStart().m_iYPosInGrid, xVert1.GetEnd().m_iXPosInGrid, xVert1.GetEnd().m_iYPosInGrid, iDir);
                    Destroy(xVert1.gameObject);
                    xSys1.m_xVertices[iDir] = null;
                }
                if((xVert2 != null && !xVert2.Contains(xSys1)))
                {
                    Destroy(xVert2.gameObject);
                    xSys2.m_xVertices[iOpposite] = null;
                }
                if (xSys1!=null && xSys2!=null && xVert1==null && xVert2==null)
                {
                    GameObject xGameObject = Instantiate(Manager.GetManager().GetVertexPrefabGameObject());
                    xGameObject.GetComponent<Vertex>().SetEndPoints(xSys1, xSys2);
                    xSys1.m_xVertices[iDir] = xGameObject.GetComponent<Vertex>();
                    xSys2.m_xVertices[iOpposite] = xGameObject.GetComponent<Vertex>();
                }          
            }
        }
    }

    public bool IsConnected(SystemBase xSys)
    {
        for(int i=0; i<6; i++)
        {
            if (GetConnectedSystem((Manager.GridDirection)i) == xSys)
            {
                return true;
            }
        }
        return false;
    }

    public void RemoveVertexWithoutDestroy(Vertex xVert)
    {
        for (int i = 0; i < 6; i++)
        {
            if (m_xVertices[i] == xVert)
            {
                m_xVertices[i] = null;
            }
        }
    }

    public static bool ShouldBeConnected(SystemBase xSystem1, SystemBase xSystem2)
    {
        return (xSystem1.transform.position - xSystem2.transform.position).magnitude < Manager.GetManager().GetConnectionRange();
    }

    void OnDestroy()
    {
        s_xAllSystems.Remove(this);
    }

    public static List<SystemBase> GetAllSystems()
    {
        // a little inefficient but prevents errors from modifying actual lists
        // TODO: look into best C# practices for this-in C++ you'd just use a const list
        return new List<SystemBase>(s_xAllSystems);
    }

    public float GetDistanceTo(SystemBase xOther)
    {
        return (xOther.transform.position - transform.position).magnitude;
    }

    public static SystemBase GetSystemWithCoords(int iX, int iY)
    {
        foreach(SystemBase xSys in s_xAllSystems)
        {
            if(iX==xSys.m_iXPosInGrid && iY == xSys.m_iYPosInGrid)
            {
                return xSys;
            }
        }
        return null;
    }

    public SystemBase GetConnectedSystem(Manager.GridDirection eDir)
    {
        Vertex xVert = m_xVertices[(int)eDir];
        if (xVert != null)
        {
            return xVert.GetStart() == this ? xVert.GetEnd() : xVert.GetStart();
        }
        else
        {
            return null;
        }
    }

    public void GetConnectedSystems(ref List<SystemBase> xOut)
    {
        for(int i = 0; i<6; i++)
        {
            xOut[i] = GetConnectedSystem((Manager.GridDirection)i);
        }
    }
}
