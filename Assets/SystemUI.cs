using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemUI : MonoBehaviour
{
    [SerializeField]
    SystemBase m_xParent;
    [SerializeField]
    UnityEngine.UI.Text m_xTitleText;
    [SerializeField]
    UnityEngine.UI.Text m_xLevelText;
    [SerializeField]
    UnityEngine.UI.Text m_xDefencesText;
    [SerializeField]
    GameObject m_xActionsBase;
    [SerializeField]
    List<GameObject> m_xActions;

    static SystemUI s_xSelected;

    protected virtual void Start()
    {
        m_xTitleText.text = m_xParent.gameObject.name;
    }

    public void Select()
    {
        s_xSelected = s_xSelected == this ? null : this;
    }

    // Update is called once per frame
    void Update()
    {
        m_xActionsBase.SetActive(s_xSelected == this);
        m_xLevelText.text = m_xParent.GetLevel().ToString();
        m_xDefencesText.text = m_xParent.GetDefences().ToString("0");
        if (m_xParent.GetLevel() > 0f)
        {
            m_xTitleText.color = m_xParent.IsHacked() ? Color.green : Color.white;
        }
        Color c = m_xTitleText.color;
        m_xTitleText.color = new Color(c.r, c.g, c.b, m_xParent.GetLevel() > 0 ? 1f : 0.4f);
    }


    public void OnActivation()
    {
        m_xLevelText.gameObject.SetActive(true);
        m_xDefencesText.gameObject.SetActive(true);
    }

    public void OnDeactivation()
    {
        m_xLevelText.gameObject.SetActive(false);
        m_xDefencesText.gameObject.SetActive(false);
    }

    public void AddAction(GameObject xAction)
    {
        foreach (GameObject x in m_xActions)
        {
            var Type1 = x.GetType();
            var Type2 = xAction.GetType();
            if(Type1 == Type2)
            {
                return;
            }
        }
        GameObject xNewAction = Instantiate(xAction, m_xActionsBase.transform);
        xAction.GetComponent<ActionBase>().SetOwner(m_xParent);
    }
}
