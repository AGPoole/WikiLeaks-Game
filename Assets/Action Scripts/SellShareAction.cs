using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellShareAction : ActionBase
{
    public override void OnClick()
    {
        var xFinanceOwner = m_xOwner.GetComponent<Finance>();
        if (xFinanceOwner != null)
        {
            xFinanceOwner.SellShare();
            return;
        }
        Debug.LogError("Wrong type to disable");
    }
}
