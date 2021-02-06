using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendButton : MonoBehaviour
{
    [SerializeField]
    GameObject m_xParent;

    public void OnClick()
    {
        m_xParent.GetComponent<SystemBase>().Defend();
    }
}
