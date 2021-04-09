using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Rename this
[RequireComponent(typeof(LineRenderer))]
public class Vertex : MonoBehaviour
{
    static List<Vertex> s_xAllVertices;

    SystemBase m_xStart;
    SystemBase m_xEnd;

    [SerializeField]
    GameObject m_xDefenceIconInstance;
    [SerializeField]
    int m_iDefence = 10;
    [SerializeField]
    UnityEngine.UI.Text m_xDefenceText;

    LineRenderer m_xRenderer;
    // Start is called before the first frame update
    void Start()
    {
        m_xRenderer = GetComponent<LineRenderer>();
        if(s_xAllVertices==null)
        {
            s_xAllVertices = new List<Vertex>();
        }
        s_xAllVertices.Add(this);
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

    void Update()
    {
        m_xDefenceText.text = m_iDefence.ToString();  
    }

    public bool Contains(SystemBase xSys)
    {
        return m_xStart == xSys || m_xEnd == xSys;
    }

    public static Vertex GetConnection(SystemBase xSystem1, SystemBase xSystem2)
    {
        if (s_xAllVertices == null)
        {
            s_xAllVertices = new List<Vertex>();
        }
        for(int i = s_xAllVertices.Count-1; i>=0; i--)
        {
            Vertex xVert = s_xAllVertices[i];
            if(xVert.Contains(xSystem1) && xVert.Contains(xSystem2))
            {
                return xVert;
            }
            //if(!SystemBase.ShouldBeConnected(xVert.m_xStart, xVert.m_xEnd))
            //{
            //    s_xAllVertices.RemoveAt(i);
            //    Destroy(xVert.gameObject);
            //}
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
        m_iDefence--;
        if (m_iDefence < 0)
        {
            m_iDefence = 0;
        }
    }

    void OnDestroy()
    {
        s_xAllVertices.Remove(this);
    }
}
