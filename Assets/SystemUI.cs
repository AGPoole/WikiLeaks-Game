using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField]
    Color m_xDisabledColor;
    [SerializeField]
    Color m_xHackedColor = Color.green;
    [SerializeField]
    Color m_xUnhackedColor = Color.white;

    static SystemUI s_xSelected;

    protected virtual void Start()
    {
        m_xTitleText.text = GetParent().gameObject.name;
    }

    public void Select()
    {
        if (WeaponManager.GetWeaponManager().GetSelectedWeapon() is WeaponBase<SystemBase>)
        {
            var xWeapon = (WeaponBase<SystemBase>)WeaponManager.GetWeaponManager().GetSelectedWeapon();
            xWeapon.Use(GetParent());
        }
        else
        {
            s_xSelected = s_xSelected == this ? null : this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_xPerksBase.SetActive(s_xSelected == this && GetParent().GetLevel() > 0);
        m_xLevelText.text = GetParent().GetLevel().ToString();
        if (GetParent().GetLevel() > 0f)
        {
            m_xTitleText.color = GetParent().IsHacked() ? m_xHackedColor : m_xUnhackedColor;
            if (GetParent().GetComponent<IDisablable>() != null
                && GetParent().GetComponent<IDisablable>().IsForceDisabled())
            {
                m_xTitleText.color = m_xDisabledColor;
            }
        }
        Color c = m_xTitleText.color;
        m_xTitleText.color = new Color(c.r, c.g, c.b, GetParent().GetLevel() > 0 ? 1f : 0.4f);
        m_xLevelText.color = m_xTitleText.color;

        Vector3 xTarget = Camera.main.transform.position;
        Vector3 v = xTarget - transform.position;
        v.y = v.z = 0.0f;
        transform.LookAt(xTarget - v);
        transform.Rotate(0, 180, 0);

        foreach (GameObject xGameObject in m_xPerkUIs)
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
            if (Type1 == Type2)
            {
                return;
            }
        }

        m_xPerkUIs.Add(PerkUI.CreatePerkUI(xPerk, GetParent(), m_xPerksBase.transform));
    }

    // TODO: this should not be in the UI
    public void ActivatePerks()
    {
        foreach (GameObject xPerkObject in m_xPerkUIs)
        {
            xPerkObject.GetComponent<PerkUI>().GetPerk().OnNextTurn();
        }
    }

    public void OnHacked()
    {
        foreach (GameObject xPerkObject in m_xPerkUIs)
        {
            xPerkObject.GetComponent<PerkUI>().GetPerk().OnHacked();
        }
    }
    public void OnUnhacked()
    {
        foreach (GameObject xPerkObject in m_xPerkUIs)
        {
            xPerkObject.GetComponent<PerkUI>().GetPerk().OnUnhacked();
        }
    }

    protected SystemBase GetParent()
    {
        return transform.parent.GetComponent<SystemBase>();
    }
}
