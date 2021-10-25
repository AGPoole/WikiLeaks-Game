using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSystem : MonoBehaviour
{
    [SerializeField]
    int m_iTimeBetweenMissions = 10;
    int m_iLastMissionTurn = 0;

    [SerializeField]
    GameObject m_xMissionUIObject;
    bool m_bPausedBefore = false;

    MissionBase m_xCurrentlySelectedMission;
    
    //TODO: delete this
    int m_iTestCurrentID=-1;

    static MissionSystem s_xInstance;

    public static MissionSystem GetMissionSystem() { return s_xInstance; }
    public void Start()
    {
        if (s_xInstance == null)
        {
            s_xInstance = this;
        }
        else
        {
            Debug.LogError("2 Mission Systems");
        }
    }

    public void OnNextTurn()
    {
        if(Manager.GetTurnNumber() > m_iLastMissionTurn + m_iTimeBetweenMissions)
        {
            m_iLastMissionTurn = Manager.GetTurnNumber();
            NotificationSystem.AddNotification(string.Format("{0}: New Mission! Click to find out more...", m_iLastMissionTurn), "", () => ClickAction(m_iLastMissionTurn));
        }
    }

    void ClickAction(int iID)
    {
        m_xMissionUIObject.SetActive(true);
        m_bPausedBefore = Manager.GetIsPaused();
        Manager.SetPaused(true);
        // TODO: store the current mission
        // Debug.LogError(iID);
        m_iTestCurrentID = iID;
    }

    public void Close()
    {
        m_xMissionUIObject.SetActive(false);
        Manager.SetPaused(m_bPausedBefore);
        m_xCurrentlySelectedMission = null;
        m_iTestCurrentID = -1;
    }

    public void OnAccept()
    {
        Debug.LogError(m_iTestCurrentID);
        Close();
    }

    public void OnReject()
    {
        Close();
    }
}

abstract class MissionBase
{
    public enum MissionState
    {
        NOT_STARTED,
        ACTIVE,
        FAILED,
        SUCCEEDED
    }

    protected MissionState m_eState;
    protected int m_iStartTurn;

    public void StartMission()
    {
        m_eState = MissionState.ACTIVE;
        m_iStartTurn = Manager.GetTurnNumber();
    }

    public MissionState GetMissionState() { return m_eState; }

    public void OnNextTurn()
    {
        if (m_eState == MissionState.ACTIVE) {
            m_eState = CheckIfComplete();
            if (m_eState == MissionState.SUCCEEDED)
            {
                OnSuccess();
            }else if(m_eState == MissionState.FAILED)
            {
                OnFailure();
            }
        }
    }

    protected abstract MissionState CheckIfComplete();

    protected abstract void OnSuccess();

    protected abstract void OnFailure();
}