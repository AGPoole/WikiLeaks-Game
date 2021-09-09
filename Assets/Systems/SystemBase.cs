using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class SystemBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    static List<SystemBase> s_xAllSystems;
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

    protected List<Edge> m_xEdges;

    [SerializeField]
    List<GameObject> m_xHexagonLineRenderers;

    bool m_bPointerOver = false;

    int m_iNextTimeToUpdateTripWires;

    const float fUI_OffsetY = -2.5f;

    GameObject m_xImageContainer;

    void Start()
    {
        Init();
    }

    public virtual void Init()
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
        if (m_xUI == null)
        {
            m_xUI = Instantiate(Manager.GetManager().GetSystemUIPrefab(), transform).GetComponent<SystemUI>();
        }
        GetMyValues().SetUpUIPerks(m_xUI);
        m_xUI.transform.position = transform.position + new Vector3(0, fUI_OffsetY, 0);
        s_xAllSystems.Add(this);

        m_xEdges = new List<Edge>();
        for (int i = 0; i < 6; i++)
        {
            m_xEdges.Add(null);
        }

        CorrectPosition();

        if (m_xHexagonLineRenderers != null) {
            for (int i=m_xHexagonLineRenderers.Count-1; i>=0; i--)
            {
                Destroy(m_xHexagonLineRenderers[i]);
                m_xHexagonLineRenderers.RemoveAt(i);
            }
        }
        m_xHexagonLineRenderers = new List<GameObject>();
        // Use 7 to loop to start
        for (int i = 0; i < 6; i++)
        {
            GameObject xLineRenderer = Instantiate(Manager.GetManager().GetLineRendererPrefabGameObject());
            xLineRenderer.transform.parent = transform;
            const float fAngleDifference = Mathf.PI / 3;
            const float fStartingAngle = -Mathf.PI / 6;
            float fAngle1 = fStartingAngle + (i * fAngleDifference);
            float fAngle2 = fStartingAngle + ((i + 1) * fAngleDifference);
            Vector3 xPos1 = transform.position + (new Vector3(Mathf.Sin(fAngle1), Mathf.Cos(fAngle1), 0f) * Manager.GetManager().GetHexagonEdgeSize());
            Vector3 xPos2 = transform.position + (new Vector3(Mathf.Sin(fAngle2), Mathf.Cos(fAngle2), 0f) * Manager.GetManager().GetHexagonEdgeSize());

            xLineRenderer.GetComponent<LineRenderer>().positionCount = 2;
            xLineRenderer.GetComponent<LineRenderer>().SetPositions(new Vector3[] { xPos1, xPos2 });

            m_xHexagonLineRenderers.Add(xLineRenderer);
        }

        if (m_xImageContainer != null)
        {
            Destroy(m_xImageContainer);
        }
        m_xImageContainer = Manager.GetManager().CreateImagePrefab(transform);
        SpriteRenderer xSpriteRenderer = m_xImageContainer.GetComponent<SpriteRenderer>();
        xSpriteRenderer.sprite = Manager.GetManager().GetSpriteAtLevel(m_iLevel);
        Color c = xSpriteRenderer.color;
        c.a = m_iLevel == 0 ? 0.4f : 1f;
        xSpriteRenderer.color = c;
        m_xUI.gameObject.SetActive(m_iLevel != 0);
    }

    #if (UNITY_EDITOR)
    [ContextMenu("Correct Position")]
    #endif
    public void CorrectPosition()
    {
        transform.position = Manager.GetManager().GetPositionFromGridCoords(m_iXPosInGrid, m_iYPosInGrid);
    }

    public void SetPosition(int iX, int iY)
    {
        m_iXPosInGrid = iX;
        m_iYPosInGrid = iY;
        CorrectPosition();
    }

    protected virtual void Update()
    {
        CorrectPosition();

        for(int i=0; i<6; i++)
        {
            SystemBase xSys = Manager.GetAdjacentSystem(m_iXPosInGrid, m_iYPosInGrid, (Manager.GridDirection)i);
            bool bEnabled = xSys==null || xSys.m_xOwner!=m_xOwner;
            m_xHexagonLineRenderers[i].SetActive(bEnabled);
        }

        if (m_bPointerOver)
        {
            IWeapon xWeapon = WeaponManager.GetWeaponManager().GetSelectedWeapon();
            if (xWeapon is WeaponBase<SystemBase>)
            {
                WeaponBase<SystemBase> xSystemWeapon = xWeapon as WeaponBase<SystemBase>;
                xSystemWeapon.OnPointerOver(this);
            }
        }
    }
    public virtual void OnNextTurn(int iOwnerLevel)
    {
        if (m_bHacked)
        {
            m_xUI.ActivatePerks();
        }
        if (Manager.GetTurnNumber() > m_iNextTimeToUpdateTripWires)
        {
            m_iNextTimeToUpdateTripWires += GetMyValues().GetTripWireUpdateTime();
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

        if (m_xImageContainer != null)
        {
            SpriteRenderer xSpriteRenderer = m_xImageContainer.GetComponent<SpriteRenderer>();
            xSpriteRenderer.sprite = Manager.GetManager().GetSpriteAtLevel(m_iLevel);
            Color c = xSpriteRenderer.color;
            c.a = m_iLevel == 0 ? 0.4f : 1f;
            xSpriteRenderer.color = c;
            m_xUI.gameObject.SetActive(m_iLevel != 0);
        }

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
        if (m_xUI != null)
        {
            m_xUI.OnDeactivation();
        }
    }
    protected virtual void OnActivation() 
    {
        m_xUI.OnActivation();
    }
    public abstract SystemValuesBase GetMyValues();

    // TODO: remove this
    public void Attack(bool bIsPlayer, int iDamage=1, bool bAttackingPlayer=false)
    {
        //if (bIsPlayer)
        //{
        //    if (Manager.GetManager().GetHacksLeft() <= iDamage)
        //    {
        //        return;
        //    }
        //    Manager.GetManager().ChangeHacks(-iDamage);
        //}
        //m_fDefences = Mathf.Max(m_fDefences - iDamage, 0);
        //if (Mathf.Approximately(m_fDefences, 0f))
        //{
        //    if (bAttackingPlayer)
        //    {
        //        UnHack();
        //    }
        //    else
        //    {
        //        Hack();
        //    }
        //}
    }
    public void Defend()
    {
        for (int i = 0; i < 6; i++)
        {
            if (m_xEdges[i] != null)
            {
                m_xEdges[i].Defend();
            }
        }
    }

    #if (UNITY_EDITOR)
    [ContextMenu("Hack")]
    #endif
    public virtual void Hack()
    {
        m_bHacked = true;
        for(int i=0; i<6; i++)
        {
            SystemBase xSys = GetConnectedSystem((Manager.GridDirection)i);
            if (xSys!=null && xSys.m_bHacked)
            {
                m_xEdges[i].Hack();
            }
        }
        m_xUI.OnHacked();
    }

    #if (UNITY_EDITOR)
    [ContextMenu("Unhack")]
    #endif
    public virtual void UnHack()
    {
        m_bHacked = false;
        for (int i = 0; i < 6; i++)
        {
            if (m_xEdges[i] != null)
            {
                m_xEdges[i].UnHack();
            }
        }
        m_xUI.OnUnhacked();
    }

    public static void SetUpEdges()
    {
        for(int i = 0; i<s_xAllSystems.Count; i++)
        {
            SystemBase xSys1 = s_xAllSystems[i];
            for (int iDir = 0; iDir < 6; iDir++)
            {
                int iOpposite = (int)Manager.GetOppositeDirection((Manager.GridDirection)iDir);
                SystemBase xSys2 = Manager.GetAdjacentSystem(xSys1.m_iXPosInGrid, xSys1.m_iYPosInGrid, (Manager.GridDirection)iDir);
                Edge xEdge1 = xSys1 != null ? xSys1.m_xEdges[iDir] : null;
                Edge xEdge2 = xSys2!=null ? xSys2.m_xEdges[iOpposite] : null;
                if (xEdge1 != null && !xEdge1.Contains(xSys2))
                {

                    Debug.LogFormat("({0} {1}) ({2} {3}) {4} ", xEdge1.GetStart().m_iXPosInGrid, xEdge1.GetStart().m_iYPosInGrid, xEdge1.GetEnd().m_iXPosInGrid, xEdge1.GetEnd().m_iYPosInGrid, iDir);
                    Destroy(xEdge1.gameObject);
                    xSys1.m_xEdges[iDir] = null;
                }
                if((xEdge2 != null && !xEdge2.Contains(xSys1)))
                {
                    Destroy(xEdge2.gameObject);
                    xSys2.m_xEdges[iOpposite] = null;
                }
                if (xSys1!=null && xSys2!=null && xEdge1==null && xEdge2==null)
                {
                    GameObject xGameObject = Instantiate(Manager.GetManager().GetEdgePrefabGameObject());
                    xGameObject.GetComponent<Edge>().SetEndPoints(xSys1, xSys2);
                    xSys1.m_xEdges[iDir] = xGameObject.GetComponent<Edge>();
                    xSys2.m_xEdges[iOpposite] = xGameObject.GetComponent<Edge>();
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

    public void RemoveEdgeWithoutDestroy(Edge xEdge)
    {
        for (int i = 0; i < 6; i++)
        {
            if (m_xEdges[i] == xEdge)
            {
                m_xEdges[i] = null;
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
        Edge xEdge = m_xEdges[(int)eDir];
        if (xEdge != null)
        {
            return xEdge.GetStart() == this ? xEdge.GetEnd() : xEdge.GetStart();
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

    // TODO: is this robust? If the pointer leaves in an unexpected way, the pointer over variable will be wrong
    public void OnPointerExit(PointerEventData eventData)
    {
        m_bPointerOver = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_bPointerOver = true;
    }

    // TODO: Could be done with reference to prevent unnecessary copying and instantiation
    protected List<DefenceIcon> GetDefenceIcons()
    {
        List<DefenceIcon> xDefenceIcons = new List<DefenceIcon>();
        foreach(Edge xEdge in m_xEdges)
        {
            if (xEdge != null)
            {
                xDefenceIcons.AddRange(xEdge.GetDefenceIcons());
            }
        }
        return xDefenceIcons;
    }

    public (int, int) GetGridPosition()
    {
        return (m_iXPosInGrid, m_iYPosInGrid);
    }

    public virtual bool CanBeOwnedByOrganisation(OrganisationBase xOrganisation)
    {
        return true;
    }

    public static void GetSystemPositionBounds(ref float fXMin, ref float fXMax, ref float fYMin, ref float fYMax)
    {
        fXMin = -10.0f;
        fXMax = 10.0f;
        fYMin = -10.0f;
        fYMax = 10.0f;
        foreach (SystemBase xSys in s_xAllSystems)
        {
            fXMin = Mathf.Min(xSys.transform.position.x, fXMin);
            fYMin = Mathf.Min(xSys.transform.position.y, fYMin);
            fXMax = Mathf.Max(xSys.transform.position.x, fXMax);
            fYMax = Mathf.Max(xSys.transform.position.y, fYMax);
        }
    }
}

// TODO: think of a better name
public interface IDisablable
{
    void SetDisabledByPlayer(bool bDisabled);
    bool IsDisabledByPlayer();
    void ForceDisable(int iNumTurns);
    bool IsForceDisabled();
}