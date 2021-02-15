using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyShareAction : ActionBase
{
    public override void OnClick()
    {
        var xFinanceOwner = m_xOwner.GetComponent<Finance>();
        if (xFinanceOwner != null)
        {
            xFinanceOwner.BuyShare();
            return;
        }
        Debug.LogError("Wrong type to disable");
    }
}
