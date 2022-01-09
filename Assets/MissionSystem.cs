using System.Collections.Generic;
using UnityEngine;

public class MissionSystem : MonoBehaviour
{
    [SerializeField]
    int m_iTimeBetweenMissions = 10;
    int m_iLastMissionTurn = 0;

    [SerializeField]
    GameObject m_xMissionAcceptanceScreenUIObject;
    bool m_bPausedBefore = false;

    MissionBase m_xCurrentlySelectedMission;

    [SerializeField]
    List<GameObject> m_xMissionPrefabs;

    // TODO: implement max size
    List<MissionBase> m_xActiveMissions = new List<MissionBase>();

    [SerializeField]
    GameObject m_xMissionUIPrefab;
    [SerializeField]
    GameObject m_xMissionUIScrollArea;
    [SerializeField]
    UnityEngine.UI.Text m_xUIPanelTitleText;
    [SerializeField]
    UnityEngine.UI.Text m_xUIPanelDescriptionText;

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
        if (Manager.GetTurnNumber() > m_iLastMissionTurn + m_iTimeBetweenMissions)
        {
            m_iLastMissionTurn = Manager.GetTurnNumber();
            List<GameObject> xValidMissions = m_xMissionPrefabs;
            for (int i = xValidMissions.Count - 1; i >= 0; i--)
            {
                if (!m_xMissionPrefabs[i].GetComponent<MissionBase>().IsUnlocked())
                {
                    xValidMissions.RemoveAt(i);
                }
            }
            if (xValidMissions.Count > 0)
            {
                // TODO: use separate screen for missions
                GameObject xPrefab = xValidMissions[Random.Range(0, xValidMissions.Count)];
                GameObject xMissionGameObject = Instantiate(xPrefab, transform);
                MissionBase xMission = xMissionGameObject.GetComponent<MissionBase>();
                Notification xNotification = NotificationSystem.AddNotification(
                    xMission.GetInitialDescription(), "",
                    () => ClickAction(xMission),
                    () => DestroyAction(xMission));
                xMission.SetNotification(xNotification);
            }
        }
        // iterate backwards so we can safely delete entries
        for (int i = m_xActiveMissions.Count - 1; i >= 0; i--)
        {
            m_xActiveMissions[i].OnNextTurn();
            if (m_xActiveMissions[i].IsFinished())
            {
                Destroy(m_xActiveMissions[i].gameObject);
                m_xActiveMissions.RemoveAt(i);
            }
        }
    }

    void ClickAction(MissionBase xMission)
    {
        m_xMissionAcceptanceScreenUIObject.SetActive(true);
        m_xUIPanelTitleText.text = xMission.GetName();
        m_xUIPanelDescriptionText.text = xMission.GetFullDescription();
        // TODO: set up acceptance screen 
        m_bPausedBefore = Manager.GetIsPaused();
        Manager.SetPaused(true);

        m_xCurrentlySelectedMission = xMission;
    }

    void DestroyAction(MissionBase xMission)
    {
        if (xMission != null && !xMission.HasStarted())
        {
            Destroy(xMission.gameObject);
        }
    }

    public void Close()
    {
        m_xMissionAcceptanceScreenUIObject.SetActive(false);
        Manager.SetPaused(m_bPausedBefore);
        m_xCurrentlySelectedMission = null;
    }

    public void OnAccept()
    {
        m_xCurrentlySelectedMission.StartMission();
        m_xActiveMissions.Add(m_xCurrentlySelectedMission);
        Close();
    }

    public void OnReject()
    {
        Close();
    }

    public GameObject GetMissionUIPrefab() { return m_xMissionUIPrefab; }
    public GameObject GetMissionUIScrollArea() { return m_xMissionUIScrollArea; }
}

public abstract class MissionBase : MonoBehaviour
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

    [SerializeField]
    string m_xName;
    // description to show on initial message
    [SerializeField]
    string m_xInitialDescription;
    // description to show on acceptance screen
    [SerializeField]
    string m_xFullDescription;
    // description to show when you have accepted the mission, in the log
    [SerializeField]
    string m_xAcceptedDescription;

    MissionUI m_xUI;

    Notification m_xNotification;

    // these next variables are kept on the prefabs to dictate how we handle multiple copies of a mission
    [SerializeField]
    bool m_bAllowRepeats = false;
    [SerializeField]
    bool m_bAllowMultiple = false;
    bool m_bNotificationExists = false;

    public bool AllowRepeats() { return m_bAllowRepeats; }
    public bool AllowMultiple() { return m_bAllowMultiple; }

    public bool NotificationExists()
    {
        if (!IsPrefab())
        {
            Debug.LogError("You should only use NotificationExists functions on prefabs");
        }
        return m_bNotificationExists;
    }

    public void SetNotificationExists(bool bExists)
    {

    }

    public void StartMission()
    {
        m_eState = MissionState.ACTIVE;
        m_iStartTurn = Manager.GetTurnNumber();
        MissionSystem xMissionSystem = MissionSystem.GetMissionSystem();
        m_xUI = Instantiate(xMissionSystem.GetMissionUIPrefab(), xMissionSystem.GetMissionUIScrollArea().transform).GetComponent<MissionUI>();
        m_xUI.SetMission(this);
        NotificationSystem.DestroyNotification(m_xNotification.gameObject);
    }

    public MissionState GetMissionState() { return m_eState; }

    public void OnNextTurn()
    {
        if (m_eState == MissionState.ACTIVE)
        {
            m_eState = CalculateNextState();
            if (m_eState == MissionState.SUCCEEDED)
            {
                OnSuccess();
            }
            else if (m_eState == MissionState.FAILED)
            {
                OnFailure();
            }

        }
    }

    public bool IsFinished() { return m_eState == MissionState.FAILED || m_eState == MissionState.SUCCEEDED; }

    public bool HasStarted() { return m_eState != MissionState.NOT_STARTED; }
    protected abstract MissionState CalculateNextState();

    protected abstract void OnSuccess();

    protected abstract void OnFailure();
    public virtual void OnQuit()
    {
        m_eState = MissionState.FAILED;
        OnFailure();
    }

    public string GetName() { return m_xName; }
    public string GetAcceptedDescription() { return m_xAcceptedDescription; }
    public string GetInitialDescription() { return m_xInitialDescription; }
    public string GetFullDescription() { return m_xFullDescription; }

    public void SetNotification(Notification xNotification) { m_xNotification = xNotification; }

    void OnDestroy()
    {
        if (m_xUI != null)
        {
            Destroy(m_xUI.gameObject);
        }
        if (m_xNotification != null)
        {
            NotificationSystem.DestroyNotification(m_xNotification.gameObject);
        }
    }

    public bool IsPrefab() { return gameObject.scene.rootCount == 0; }

    public abstract bool IsUnlocked();
}