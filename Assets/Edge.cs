using System.Collections.Generic;
using UnityEngine;

// TODO: Rename this
[RequireComponent(typeof(LineRenderer))]
public class Edge : MonoBehaviour
{
    static List<Edge> s_xAllEdges;

    [SerializeField]
    SystemBase m_xStart;
    [SerializeField]
    SystemBase m_xEnd;

    [SerializeField]
    GameObject m_xDefenceIconPrefab;
    [SerializeField]
    List<DefenceIcon> m_xDefenceIconInstances;
    [SerializeField]
    DefenceIcon m_xMainDefenceIcon;
    [SerializeField]
    int m_iDefenceMax = 10;

    LineRenderer m_xRenderer;
    // Start is called before the first frame update
    void Start()
    {
        m_xRenderer = GetComponent<LineRenderer>();
        if (s_xAllEdges == null)
        {
            s_xAllEdges = new List<Edge>();
        }
        s_xAllEdges.Add(this);
        UnHack();
        m_xMainDefenceIcon = Instantiate(m_xDefenceIconPrefab, transform).GetComponent<DefenceIcon>();
        m_xDefenceIconInstances.Add(m_xMainDefenceIcon);
        SetDefenceIconPositions();
        SetMainDefenceIconValues();
    }

    public void SetEndPoints(SystemBase xStart, SystemBase xEnd)
    {
        m_xStart = xStart;
        m_xEnd = xEnd;
        m_xRenderer = GetComponent<LineRenderer>();
        m_xRenderer.positionCount = 2;
        m_xRenderer.SetPosition(0, m_xStart.transform.position);
        m_xRenderer.SetPosition(1, m_xEnd.transform.position);
        SetDefenceIconPositions();
        transform.position = (xStart.transform.position + xEnd.transform.position) / 2;
    }

    [SerializeField]
    float m_fIconOffset = 0.01f;
    void SetDefenceIconPositions()
    {
        if (m_xStart == null || m_xEnd == null)
        {
            return;
        }
        int iLength = m_xDefenceIconInstances.Count;
        Vector3 xDifference = m_xEnd.transform.position - m_xStart.transform.position;
        xDifference -= 2 * m_fIconOffset * xDifference.normalized;
        Vector3 xStart = m_xStart.transform.position + m_fIconOffset * xDifference.normalized;
        for (int i = 0; i < iLength; i++)
        {
            m_xDefenceIconInstances[i].transform.position = xStart + ((i + 1) * xDifference / (iLength + 1));
        }
    }

    float m_fRechargeTimer = 0;
    float m_fDeteriorationTimer = 0;
    void Update()
    {
        SetMainDefenceIconValues();
    }

    public static ref readonly List<Edge> GetAllEdges()
    {
        return ref s_xAllEdges;
    }

    // TODO: remove this function
    void SetMainDefenceIconValues()
    {
        m_xMainDefenceIcon.SetOwner(this);
    }

    public bool Contains(SystemBase xSys)
    {
        return m_xStart == xSys || m_xEnd == xSys;
    }

    public static Edge GetConnection(SystemBase xSystem1, SystemBase xSystem2)
    {
        if (s_xAllEdges == null)
        {
            s_xAllEdges = new List<Edge>();
        }
        for (int i = s_xAllEdges.Count - 1; i >= 0; i--)
        {
            Edge xEdge = s_xAllEdges[i];
            if (xEdge.Contains(xSystem1) && xEdge.Contains(xSystem2))
            {
                return xEdge;
            }
        }
        return null;
    }

    public void Hack()
    {
        Color xCol = Color.green;
        xCol.a = 0.4f;
        m_xRenderer.startColor = xCol;
        m_xRenderer.endColor = xCol;
    }
    public void UnHack()
    {
        Color xCol = Color.white;
        xCol.a = 0.4f;
        m_xRenderer.startColor = xCol;
        m_xRenderer.endColor = xCol;
    }

    public void CheckDefences()
    {
        foreach (DefenceIcon xIcon in m_xDefenceIconInstances)
        {
            if (xIcon.GetDefence() > 0)
            {
                return;
            }
        }
        if (!m_xStart.IsHacked())
        {
            m_xStart.Hack();
        }
        else if (!m_xEnd.IsHacked())
        {
            m_xEnd.Hack();
        }
    }

    // TODO: remove this
    public void Defend()
    {
        foreach (DefenceIcon xIcon in m_xDefenceIconInstances)
        {
            xIcon.Defend();
        }
    }

    public SystemBase GetStart()
    {
        return m_xStart;
    }

    public SystemBase GetEnd()
    {
        return m_xEnd;
    }

    public void RegisterCyberSec(CyberSecurity xSec)
    {
        // TODO: use a hash-map
        foreach (DefenceIcon xIcon in m_xDefenceIconInstances)
        {
            if (xIcon.GetCyberSecurityOwner() == xSec)
            {
                return;
            }
        }
        DefenceIcon xNew = Instantiate(m_xDefenceIconPrefab, transform).GetComponent<DefenceIcon>();
        m_xDefenceIconInstances.Add(xNew);
        xNew.SetOwner(this);
        xNew.SetCyberSecurityOwner(xSec);
        SetDefenceIconPositions();
    }

    public void RemoveDefenceIcon(DefenceIcon xIcon)
    {
        m_xDefenceIconInstances.Remove(xIcon);
        Destroy(xIcon.gameObject);
        SetDefenceIconPositions();
    }

    public Vector3 GetPosition()
    {
        return (m_xStart.transform.position + m_xEnd.transform.position) / 2;
    }

    public bool IsReachable(DefenceIcon xIcon)
    {
        int iIndex = m_xDefenceIconInstances.IndexOf(xIcon);
        if (iIndex == -1)
        {
            Debug.LogError("Attempting to find if defence icon is reachable in wrong edge");
            return false;
        }
        if (m_xStart.IsHacked())
        {
            if (iIndex == 0)
            {
                return true;
            }
            bool bReachableFromStart = true;
            for (int i = 0; i < iIndex; i++)
            {
                if (m_xDefenceIconInstances[i].GetDefence() > 0)
                {
                    bReachableFromStart = false;
                    break;
                }
            }
            if (bReachableFromStart)
            {
                return true;
            }
        }
        if (!m_xEnd.IsHacked())
        {
            return false;
        }
        if (iIndex == m_xDefenceIconInstances.Count - 1)
        {
            return true;
        }
        for (int i = iIndex + 1; i < m_xDefenceIconInstances.Count; i++)
        {
            if (m_xDefenceIconInstances[i].GetDefence() > 0)
            {
                return false;
            }
        }
        return true;
    }

    public List<DefenceIcon> GetDefenceIcons()
    {
        return m_xDefenceIconInstances;
    }

    void OnDestroy()
    {
        s_xAllEdges.Remove(this);
        if (m_xStart != null)
        {
            m_xStart.RemoveEdgeWithoutDestroy(this);
        }
        if (m_xEnd != null)
        {
            m_xEnd.RemoveEdgeWithoutDestroy(this);
        }
    }
}
