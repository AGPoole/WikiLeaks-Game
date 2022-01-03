using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionUI : MonoBehaviour
{
    MissionBase m_xMission;

    [SerializeField]
    UnityEngine.UI.Text m_xDescriptionText;

    public void SetMission(MissionBase xMission)
    {
        m_xMission = xMission;
        m_xDescriptionText.text = xMission.GetAcceptedDescription();
    }

    public void QuitMission()
    {
        m_xMission.OnQuit();
    }
}
