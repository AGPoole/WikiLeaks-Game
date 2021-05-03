using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PerkBase : MonoBehaviour
{
    protected SystemBase m_xSystemOwner;

    protected PerkUI m_xUI;

    [SerializeField]
    string m_xName;
    [SerializeField]
    string m_xDescription;
    [SerializeField]
    protected Sprite m_xIcon;
    [SerializeField]
    bool m_bRequiresHack = true;

    protected virtual void Start()
    {
        
    }
    public virtual void OnClick()
    { }

    public void SetSystemOwner(SystemBase xOwner)
    {
        m_xSystemOwner = xOwner;
    }

    public void SetUI(PerkUI xUI)
    {
        m_xUI = xUI;
    }

    public virtual void Update()
    {

    }

    public virtual void OnNextTurn() 
    { }

    public string GetName()
    {
        return m_xName;
    }

    public Sprite GetIcon()
    {
        return m_xIcon;
    }

    public string GetDescription()
    {
        return m_xDescription;
    }

    public  bool GetRequiresHack()
    {
        return m_bRequiresHack;
    }

    public virtual void OnHacked()
    {

    }

    public virtual void OnUnhacked()
    {

    }
}
