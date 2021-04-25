using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemUI : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.Text m_xTitleText;
    [SerializeField]
    UnityEngine.UI.Text m_xLevelText;
    [SerializeField]
    GameObject m_xActionsBase;
    [SerializeField]
    List<GameObject> m_xPerkUIs;

    static SystemUI s_xSelected;

    protected virtual void Start()
    {
        m_xTitleText.text = GetParent().gameObject.name;
    }

    public void Select()
    {
        s_xSelected = s_xSelected == this ? null : this;
    }

    // Update is called once per frame
    void Update()
    {
        m_xActionsBase.SetActive(s_xSelected == this && GetParent().GetLevel()>0);
        m_xLevelText.text = GetParent().GetLevel().ToString();
        if (GetParent().GetLevel() > 0f)
        {
            m_xTitleText.color = GetParent().IsHacked() ? Color.green : Color.white;
        }
        Color c = m_xTitleText.color;
        m_xTitleText.color = new Color(c.r, c.g, c.b, GetParent().GetLevel() > 0 ? 1f : 0.4f);

        //Vector3 xTarget = new Vector3(transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
        Vector3 xTarget = Camera.main.transform.position;
        Vector3 v = xTarget - transform.position;
        v.y = v.z = 0.0f;
        transform.LookAt(xTarget - v);
        transform.Rotate(0, 180, 0);

        foreach(GameObject xGameObject in m_xPerkUIs)
        {
            xGameObject.GetComponent<PerkUI>().UpdateActive(GetParent().IsHacked());
        }
    }


    public void OnActivation()
    {
        m_xLevelText.gameObject.SetActive(true);
    }

    public void OnDeactivation()
    {
        m_xLevelText.gameObject.SetActive(false);
    }

    public void AddAction(GameObject xAction)
    {
        foreach (GameObject xAction2 in m_xPerkUIs)
        {
            var Type1 = xAction.GetComponent<ActionBase>().GetType();
            var Type2 = xAction2.GetComponent<PerkUI>().GetAction().GetType();
            if(Type1 == Type2)
            {
                return;
            }
        }
        
        m_xPerkUIs.Add(PerkUI.CreatePerkUI(xAction, GetParent(), m_xActionsBase.transform));
    }

    // TODO: this should not be in the UI
    public void ActivatePerks()
    {
        foreach(GameObject xActionObject in m_xPerkUIs)
        {
            xActionObject.GetComponent<PerkUI>().GetAction().OnNextTurn();
        }
    }

    protected SystemBase GetParent()
    {
        return transform.parent.GetComponent<SystemBase>();
    }
}
