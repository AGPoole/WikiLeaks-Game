using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationSystem : MonoBehaviour
{
    static NotificationSystem s_xNotificationSystem;

    [SerializeField]
    GameObject m_xNotificationPrefab;
    [SerializeField]
    GameObject m_xScrollArea;
    
    [SerializeField]
    [Range(1, 200)]
    int m_iMaxNotifications=100;

    // TODO: switch to list of notifications rather than game objects
    List<GameObject> m_xNotifications;

    void Start()
    {
        s_xNotificationSystem = this;
        m_xNotifications = new List<GameObject>();
    }

    public static NotificationSystem GetNotificationSystem()
    {
        return s_xNotificationSystem;
    }

    public static void AddNotification(string xString, string xDescription="")
    {
        GameObject xNewNotification = Instantiate(s_xNotificationSystem.m_xNotificationPrefab, s_xNotificationSystem.m_xScrollArea.transform);
        s_xNotificationSystem.m_xNotifications.Add(xNewNotification);
        xNewNotification.GetComponent<Notification>().SetText(xString);
        xNewNotification.GetComponent<Notification>().SetDescription(xDescription);
        xNewNotification.name = xString;
    }

    public static void OnNextTurn()
    {
        if (s_xNotificationSystem == null)
        {
            return;
        }
        while (s_xNotificationSystem.m_xNotifications.Count >= s_xNotificationSystem.m_iMaxNotifications)
        {
            GameObject xLast = s_xNotificationSystem.m_xNotifications[0];
            Destroy(xLast);
            s_xNotificationSystem.m_xNotifications.RemoveAt(0);
        }
        foreach (GameObject xNot in s_xNotificationSystem.m_xNotifications)
        {
            xNot.GetComponent<Notification>().OnNextTurn();
        }
    }
}
