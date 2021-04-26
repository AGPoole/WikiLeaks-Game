﻿using System.Collections;
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
    GameObject m_xPerksBase;
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
        m_xPerksBase.SetActive(s_xSelected == this && GetParent().GetLevel()>0);
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

    public void AddPerk(GameObject xPerk)
    {
        foreach (GameObject xPerk2 in m_xPerkUIs)
        {
            var Type1 = xPerk.GetComponent<PerkBase>().GetType();
            var Type2 = xPerk2.GetComponent<PerkUI>().GetPerk().GetType();
            if(Type1 == Type2)
            {
                return;
            }
        }
        
        m_xPerkUIs.Add(PerkUI.CreatePerkUI(xPerk, GetParent(), m_xPerksBase.transform));
    }

    // TODO: this should not be in the UI
    public void ActivatePerks()
    {
        foreach(GameObject xPerkObject in m_xPerkUIs)
        {
            xPerkObject.GetComponent<PerkUI>().GetPerk().OnNextTurn();
        }
    }

    protected SystemBase GetParent()
    {
        return transform.parent.GetComponent<SystemBase>();
    }
}
