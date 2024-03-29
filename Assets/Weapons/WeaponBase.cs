﻿using UnityEngine;
using UnityEngine.EventSystems;

public abstract class WeaponBase<TargetType> : MonoBehaviour, IWeapon, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    protected int m_iRechargeTime;
    [SerializeField]
    UnityEngine.UI.Slider m_xSlider;
    [SerializeField]
    Sprite m_xSelectedIcon;
    [SerializeField]
    Sprite m_xUnselectedIcon;
    [SerializeField]
    UnityEngine.UI.Text m_xDescriptionText;
    [SerializeField]
    string m_xDescription;

    [SerializeField]
    protected SystemBase m_xOwner;
    [SerializeField]
    bool m_bDetectable = true;

    UnityEngine.UI.Image m_xImage;
    int m_iLastUseTime;

    protected virtual void Start()
    {
        m_xImage = GetComponent<UnityEngine.UI.Image>();
    }

    protected virtual void Update()
    {
        // TODO: make continuous
        m_xSlider.value = Mathf.Clamp01(1 - ((Manager.GetTurnNumber() - m_iLastUseTime) / (float)WeaponManager.GetWeaponManager().GetModifiedRechargeTime(m_iRechargeTime)));

        if (WeaponManager.GetWeaponManager().GetSelectedWeapon() == this)
        {
            m_xImage.sprite = m_xSelectedIcon;
        }
        else
        {
            m_xImage.sprite = m_xUnselectedIcon;
        }
    }

    public bool IsReady()
    {
        return Manager.GetTurnNumber() > m_iLastUseTime + WeaponManager.GetWeaponManager().GetModifiedRechargeTime(m_iRechargeTime);
    }

    public bool Use(TargetType tType)
    {
        if (IsReady())
        {
            bool bUseSuccessful = UseInternal(tType);
            if (bUseSuccessful)
            {
                m_iLastUseTime = Manager.GetTurnNumber();
            }
            return bUseSuccessful;
        }
        else
        {
            return false;
        }
    }

    protected abstract bool UseInternal(TargetType tType);

    public void Select()
    {
        WeaponManager.GetWeaponManager().SetSelectedWeapon(gameObject);
    }

    public virtual void OnPointerOver(TargetType tType)
    {
    }

    public void SetOwner(SystemBase xOwner)
    {
        m_xOwner = xOwner;
    }

    public SystemBase GetOwner()
    {
        return m_xOwner;
    }

    public Sprite GetUnselectedIcon()
    {
        return m_xUnselectedIcon;
    }

    public bool IsDetectable() { return m_bDetectable; }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_xDescriptionText.transform.parent.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_xDescriptionText.transform.parent.gameObject.SetActive(true);
        m_xDescriptionText.text = GetDescription();
    }

    public virtual string GetDescription()
    {
        return string.Format("Recovery Time: {0}\n", m_iRechargeTime) + m_xDescription;
    }
}

public interface IWeapon
{
    void SetOwner(SystemBase xOwner);
    SystemBase GetOwner();
    Sprite GetUnselectedIcon();

    bool IsDetectable();
}
