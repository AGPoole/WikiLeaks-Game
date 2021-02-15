using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAction : ActionBase
{
    [SerializeField]
    Sprite m_xOnSprite;
    [SerializeField]
    Sprite m_xOffSprite;

    protected override void Start()
    {
        base.Start();
        var xSecOwner = m_xOwner.GetComponent<CyberSecurity>();
        if (xSecOwner != null)
        {
            GetComponentInChildren<UnityEngine.UI.Button>().image.sprite = xSecOwner.IsDisabledByPlayer() ? m_xOnSprite : m_xOffSprite;
        }
    }
    public override void OnClick()
    {
        var xSecOwner = m_xOwner.GetComponent<CyberSecurity>();
        if (xSecOwner != null)
        {
            GetComponentInChildren<UnityEngine.UI.Button>().image.sprite = xSecOwner.IsDisabledByPlayer() ? m_xOnSprite : m_xOffSprite;
            xSecOwner.SetDisabledByPlayer(!xSecOwner.IsDisabledByPlayer());
            return;
        }
        Debug.LogError("Wrong type to disable");
    }

    public override void SetHacked(bool bHacked)
    {
        base.SetHacked(bHacked);
        gameObject.SetActive(bHacked);
    }
}
