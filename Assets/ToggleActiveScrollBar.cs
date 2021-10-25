using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleActiveScrollBar : MonoBehaviour
{
    [SerializeField]
    GameObject m_xScrollArea;

    static List<GameObject> s_xScrollAreas = new List<GameObject>();

    public void Awake()
    {
        if (!s_xScrollAreas.Contains(m_xScrollArea))
        {
            s_xScrollAreas.Add(m_xScrollArea);
        }
    }
    public void OnClick()
    {
        if (m_xScrollArea.activeSelf)
        {
            m_xScrollArea.SetActive(false);
        }
        else
        {
            foreach(GameObject xArea in s_xScrollAreas)
            {
                xArea.SetActive(false);
            }
            m_xScrollArea.SetActive(true);
        }
    }
}
