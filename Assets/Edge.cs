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
    GameObject m_xDefenceIconInstance;
    [SerializeField]
    int m_iDefenceMax = 10;
    int m_iDefence = 10;
    [SerializeField]
    float m_fAdditionalDefenceDegradationTime = 0.5f;
    [SerializeField]
    float m_fDefenceRechargeTime = 0.5f;
    [SerializeField]
    float m_fDefencePauseAtZero = 2f;
    [SerializeField]
    UnityEngine.UI.Text m_xDefenceText;

    List<CyberSecurity> m_xCyberSecs;

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
     }

    public void SetEndPoints(SystemBase xStart, SystemBase xEnd)
    {
        m_xStart = xStart;
        m_xEnd = xEnd;
        m_xRenderer = GetComponent<LineRenderer>();
        m_xRenderer.positionCount = 2;
        m_xRenderer.SetPosition(0, m_xStart.transform.position);
        m_xRenderer.SetPosition(1, m_xEnd.transform.position);
        m_xDefenceIconInstance.transform.position = (xStart.transform.position + xEnd.transform.position) / 2;
    }

    float m_fRechargeTimer = 0;
    float m_fDeteriorationTimer = 0;
    void Update()
    {
        m_xDefenceText.text = m_iDefence.ToString();
        m_iDefenceMax = m_xStart.GetMyValues().GetBaseDefenceMax();
        m_fAdditionalDefenceDegradationTime = m_xStart.GetMyValues().GetAdditionalShielddeteriorationTime();
        m_fDefenceRechargeTime = m_xStart.GetMyValues().GetBaseDefenceRefreshTime();
        if (m_xEnd.GetMyValues().GetBaseDefenceMax() > m_iDefenceMax)
        {
            m_iDefenceMax = m_xEnd.GetMyValues().GetBaseDefenceMax();
        }
        if(m_xEnd.GetMyValues().GetBaseDefenceRefreshTime() < m_fDefenceRechargeTime)
        {
            m_fDefenceRechargeTime = m_xEnd.GetMyValues().GetBaseDefenceRefreshTime();
        }
        if (m_xEnd.GetMyValues().GetAdditionalShielddeteriorationTime() > m_fAdditionalDefenceDegradationTime)
        {
            m_fAdditionalDefenceDegradationTime = m_xEnd.GetMyValues().GetAdditionalShielddeteriorationTime();
        }
        if (!Manager.GetIsPaused())
        {
            //TODO: make Cybersecurity add more
            if (m_iDefence < m_iDefenceMax)
            {
                m_fRechargeTimer += Time.deltaTime;
                if ((m_iDefence != 0 && m_fRechargeTimer > m_fDefenceRechargeTime)
                    || m_fRechargeTimer > m_fDefencePauseAtZero)
                {
                    m_fRechargeTimer -= m_iDefence == 0 ? m_fDefencePauseAtZero : m_fDefenceRechargeTime;
                    m_iDefence++;
                }
            }
            else
            {
                m_fRechargeTimer = 0;
            }
            if (m_iDefence > m_iDefenceMax)
            {
                m_fDeteriorationTimer += Time.deltaTime;
                if (m_fDeteriorationTimer > m_fAdditionalDefenceDegradationTime)
                {
                    m_fDeteriorationTimer -= m_fAdditionalDefenceDegradationTime;
                    m_iDefence++;
                }
            }
            else
            {
                m_fDeteriorationTimer = 0;
            }
        }
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

    public void Attack()
    {
        if (Manager.GetManager().GetHacksLeft() <= 1)
        {
            return;
        }
        Manager.GetManager().ChangeHacks(-1);
        if (m_xStart.IsHacked() || m_xEnd.IsHacked())
        {
            m_iDefence--;
            m_fRechargeTimer = 0;
            if (m_iDefence < 0)
            {
                m_iDefence = 0;
                if (!m_xStart.IsHacked())
                {
                    m_xStart.Hack();
                }
                else if (!m_xEnd.IsHacked())
                {
                    m_xEnd.Hack();
                }
            }
        }
    }

    public void Defend()
    {
        if (m_iDefence < m_iDefenceMax)
        {
            m_iDefence += 1;
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
