using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAction : ActionBase
{
    [SerializeField]
    Sprite m_xOnSprite;
    [SerializeField]
    Sprite m_xOffSprite;
    [SerializeField]
    UnityEngine.UI.Image m_xIcon;

    protected override void Start()
    {
        base.Start();
        var xSecOwner = m_xOwner.GetComponent<CyberSecurity>();
        if (xSecOwner != null)
        {
            m_xIcon.sprite = xSecOwner.IsDisabledByPlayer() ? m_xOnSprite : m_xOffSprite;
        }
    }
    public override void OnClick()
    {
        {
            var xSecOwner = m_xOwner.GetComponent<CyberSecurity>();
            if (xSecOwner != null)
            {
                m_xIcon.sprite = xSecOwner.IsDisabledByPlayer() ? m_xOnSprite : m_xOffSprite;
                xSecOwner.SetDisabledByPlayer(!xSecOwner.IsDisabledByPlayer());
                return;
            }
        }
        {
            var xAVOwner = m_xOwner.GetComponent<AntiVirusSystem>();
            if (xAVOwner != null)
            {
                m_xIcon.sprite = xAVOwner.IsDisabledByPlayer() ? m_xOnSprite : m_xOffSprite;
                xAVOwner.SetDisabledByPlayer(!xAVOwner.IsDisabledByPlayer());
                return;
            }
        }
        Debug.LogError("Wrong type to disable");
    }

    public override void Update()
    {
        base.Update();
        gameObject.SetActive(m_xOwner.IsHacked());
    }
}
