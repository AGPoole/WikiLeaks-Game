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
    List<GameObject> m_xActions;

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

        foreach(GameObject xActionObject in m_xActions)
        {
            ActionBase xAction = xActionObject.GetComponent<ActionBase>();
            xAction.Update();
        }
        //Vector3 xTarget = new Vector3(transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
        Vector3 xTarget = Camera.main.transform.position;
        Vector3 v = xTarget - transform.position;
        v.y = v.z = 0.0f;
        transform.LookAt(xTarget - v);
        transform.Rotate(0, 180, 0);
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
        foreach (GameObject xAction2 in m_xActions)
        {
            var Type1 = xAction.GetComponent<ActionBase>().GetType();
            var Type2 = xAction2.GetComponent<ActionBase>().GetType();
            if(Type1 == Type2)
            {
                return;
            }
        }
        GameObject xNewAction = Instantiate(xAction, m_xActionsBase.transform);
        m_xActions.Add(xNewAction);
        var xNewActionComponent = xNewAction.GetComponent<ActionBase>();
        xNewActionComponent.SetOwner(GetParent());
        var axButtons = xNewActionComponent.GetComponentsInChildren<UnityEngine.UI.Button>();
        foreach(var xButton in axButtons)
        {
            var xNav = xButton.navigation;
            xNav.mode = Navigation.Mode.None;
            xButton.navigation = xNav;
        }
    }

    protected SystemBase GetParent()
    {
        return transform.parent.GetComponent<SystemBase>();
    }
}
