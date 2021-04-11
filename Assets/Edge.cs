using System;
using System.Collections;
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
        if(s_xAllEdges==null)
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
    }

    void SetDefenceIconPositions()
    {
        if(m_xStart==null || m_xEnd == null)
        {
            return;
        }
        int iLength = m_xDefenceIconInstances.Count;
        for(int i=0; i<iLength; i++)
        {
            m_xDefenceIconInstances[i].transform.position = m_xStart.transform.position + ((i+1) * (m_xEnd.transform.position - m_xStart.transform.position) / (iLength+1));
        }
    }

    float m_fRechargeTimer = 0;
    float m_fDeteriorationTimer = 0;
    void Update()
    {
        SetMainDefenceIconValues();
    }

    void SetMainDefenceIconValues()
    {
        m_xMainDefenceIcon.SetOwner(this);
        m_xMainDefenceIcon.SetMaxDefense(ProjectMaths.Max(
            m_xStart.GetMyValues().GetBaseDefenceMax(),
            m_xEnd.GetMyValues().GetBaseDefenceMax()));
        m_xMainDefenceIcon.SetDefenceDegradationTime(Mathf.Max(
            m_xStart.GetMyValues().GetAdditionalShielddeteriorationTime(),
            m_xEnd.GetMyValues().GetAdditionalShielddeteriorationTime()
            ));
        m_xMainDefenceIcon.SetDefenceRechargeTime(Mathf.Min(
            m_xStart.GetMyValues().GetBaseDefenceRefreshTime(),
            m_xEnd.GetMyValues().GetBaseDefenceRefreshTime()
            ));
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
        for(int i = s_xAllEdges.Count-1; i>=0; i--)
        {
            Edge xEdge = s_xAllEdges[i];
            if(xEdge.Contains(xSystem1) && xEdge.Contains(xSystem2))
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
        foreach(DefenceIcon xIcon in m_xDefenceIconInstances)
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
        foreach(DefenceIcon xIcon in m_xDefenceIconInstances)
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
