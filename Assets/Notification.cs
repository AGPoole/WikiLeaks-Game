using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Notification : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    UnityEngine.UI.Text m_xNotificationText;
    [SerializeField]
    UnityEngine.UI.Text m_xAgeText;
    [SerializeField]
    GameObject m_xDescriptionPanel;
    [SerializeField]
    UnityEngine.UI.Text m_xDescriptionText;

    string m_xMyText;
    int m_iCreationTurn;
    string m_xDescription = "";

    UnityAction m_xClickAction;
    UnityAction m_xDestroyAction;

    void Awake()
    {
        m_iCreationTurn = Manager.GetTurnNumber();
    }

    public void OnNextTurn()
    {
        string xAgeText = "";
        int iTime = Manager.GetTurnNumber() - m_iCreationTurn;
        if (iTime == 0)
        {
            xAgeText = "Just now";
        } else if(iTime < 7)
        {
            xAgeText = string.Format("{0} day{1} ago", iTime, iTime==1?"":"s");
        } else if(iTime < 30)
        {
            xAgeText = string.Format("{0} week{1} ago", iTime/7, iTime/7 == 1 ? "" : "s");
        } else
        {
            xAgeText = string.Format("{0} month{1} ago", iTime / 30, iTime / 30 == 1 ? "" : "s");
        }
        m_xAgeText.text = xAgeText;
    }

    public void SetText(string xText)
    {
        m_xMyText = xText;
        m_xNotificationText.text = m_xMyText;
    }
    
    public void SetDescription(string xText)
    {
        m_xDescription = xText;
        m_xDescriptionText.text = m_xDescription;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (m_xDescription != "")
        {
            m_xDescriptionPanel.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_xDescriptionPanel.SetActive(false);
    }

    public void SetOnClick(UnityAction xAction)
    {
        m_xClickAction = xAction;
    }

    public void OnClick()
    {
        m_xClickAction?.Invoke();
    }

    public void SetOnDestroy(UnityAction xAction) { m_xDestroyAction = xAction; }

    public void OnDestroy()
    {
        if (m_xDestroyAction != null)
        {
            m_xDestroyAction.Invoke();
        }
    }
}
