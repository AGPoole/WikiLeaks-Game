using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleNotifications : MonoBehaviour
{
    [SerializeField]
    GameObject m_xScrollArea;
    public void OnClick()
    {
        m_xScrollArea.SetActive(!m_xScrollArea.activeSelf);
    }
}
