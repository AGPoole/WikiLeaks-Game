using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase<TargetType> : MonoBehaviour, IWeapon
{
    [SerializeField]
    protected int m_iRechargeTime;
    [SerializeField]
    UnityEngine.UI.Slider m_xSlider;
    [SerializeField]
    Sprite m_xSelectedIcon;
    [SerializeField]
    Sprite m_xUnselectedIcon;

    UnityEngine.UI.Image m_xImage;
    int m_iLastUseTime;

    protected virtual void Start()
    {
        m_xImage = GetComponent<UnityEngine.UI.Image>();
    }

    protected virtual void Update()
    {
        // TODO: make continuous
        m_xSlider.value = Mathf.Clamp01(1 -((Manager.GetTurnNumber() - m_iLastUseTime) / (float)m_iRechargeTime));

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
        return Manager.GetTurnNumber() > m_iLastUseTime + m_iRechargeTime;
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
}

public interface IWeapon 
{
}
