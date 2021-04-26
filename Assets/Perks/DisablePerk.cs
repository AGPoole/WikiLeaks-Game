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
        {
            var xSecOwner = m_xSystemOwner.GetComponent<CyberSecurity>();
            if (xSecOwner != null)
            {
                m_xUI.SetSprite(xSecOwner.IsDisabledByPlayer() ? m_xOnSprite : m_xOffSprite);
                xSecOwner.SetDisabledByPlayer(!xSecOwner.IsDisabledByPlayer());
                return;
            }
        }
        {
            var xAVOwner = m_xSystemOwner.GetComponent<AntiVirusSystem>();
            if (xAVOwner != null)
            {
                m_xUI.SetSprite(xAVOwner.IsDisabledByPlayer() ? m_xOnSprite : m_xOffSprite);
                xAVOwner.SetDisabledByPlayer(!xAVOwner.IsDisabledByPlayer());
                return;
            }
        }
        Debug.LogError("Wrong type to disable");
    }
}
