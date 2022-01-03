using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        gameObject.SetActive(false);
    }

    public static NotificationSystem GetNotificationSystem()
    {
        return s_xNotificationSystem;
    }

    public static Notification AddNotification(string xString, string xDescription="", UnityAction xOnClick = null, UnityAction xOnDestroy = null)
    {
        GameObject xNewNotificationGameObject = Instantiate(s_xNotificationSystem.m_xNotificationPrefab, s_xNotificationSystem.m_xScrollArea.transform);
        s_xNotificationSystem.m_xNotifications.Add(xNewNotificationGameObject);
        Notification xNewNotification = xNewNotificationGameObject.GetComponent<Notification>();
        xNewNotification.SetText(xString);
        xNewNotification.SetDescription(xDescription);
        xNewNotification.SetOnClick(xOnClick);
        xNewNotification.SetOnDestroy(xOnDestroy);
        xNewNotificationGameObject.name = xString;
        return xNewNotification;
    }

    public static void OnNextTurn()
    {
        if (s_xNotificationSystem == null)
        {
            return;
        }
        while (s_xNotificationSystem.m_xNotifications.Count >= s_xNotificationSystem.m_iMaxNotifications)
        {
            DestroyNotificationAtIndex(0);
        }
        foreach (GameObject xNot in s_xNotificationSystem.m_xNotifications)
        {
            xNot.GetComponent<Notification>().OnNextTurn();
        }
    }

    public static void DestroyNotification(GameObject xNotification)
    {
        int iIndex = s_xNotificationSystem.m_xNotifications.IndexOf(xNotification);
        if(iIndex != -1)
        {
            DestroyNotificationAtIndex(iIndex);
        }
    }

    static void DestroyNotificationAtIndex(int iIndex)
    {
        GameObject xGameObject = s_xNotificationSystem.m_xNotifications[iIndex];
        Destroy(xGameObject);
        s_xNotificationSystem.m_xNotifications.RemoveAt(iIndex);
    }
}
