using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisablePerk : PerkBase
{
    [SerializeField]
    Sprite m_xOnSprite;
    [SerializeField]
    Sprite m_xOffSprite;

    protected override void Start()
    {
        base.Start();
        var xSecOwner = m_xSystemOwner.GetComponent<CyberSecurity>();
        if (xSecOwner != null)
        {
            m_xUI.SetSprite(xSecOwner.IsDisabledByPlayer() ? m_xOnSprite : m_xOffSprite);
        }
    }
    public override void OnClick()
    {
        var xDisableOwner = m_xSystemOwner.GetComponent<IDisablable>();
        if (xDisableOwner != null)
        {
            m_xUI.SetSprite(xDisableOwner.IsDisabledByPlayer() ? m_xOnSprite : m_xOffSprite);
            xDisableOwner.SetDisabledByPlayer(!xDisableOwner.IsDisabledByPlayer());
            return;
        }
        else
        {
            Debug.LogError("Wrong type to disable");
        }
    }
}
