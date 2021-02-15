using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAction : ActionBase
{
    public override void OnClick()
    {
        var xSecOwner = m_xOwner.GetComponent<CyberSecurity>();
        if (xSecOwner != null)
        {
            xSecOwner.SetDisabledByPlayer(!xSecOwner.IsDisabledByPlayer());
            return;
        }
        Debug.LogError("Wrong type to disable");
    }
}
