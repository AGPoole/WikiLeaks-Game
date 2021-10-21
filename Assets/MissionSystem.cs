using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSystem : MonoBehaviour
{
    [SerializeField]
    int m_iTimeBetweenMissions = 10;
    int m_iLastMissionTurn = 0;

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
            NotificationSystem.AddNotification("New Mission! Click to find out more...", "", ClickAction);
        }
    }

    void ClickAction()
    {
        Debug.LogError("Test");
    }
}
