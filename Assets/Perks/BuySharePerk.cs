using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuySharePerk : PerkBase
{
    public override void OnClick()
    {
        var xFinanceOwner = m_xSystemOwner.GetComponent<Finance>();
        if (xFinanceOwner != null)
        {
            xFinanceOwner.BuyShare();
            return;
        }
        Debug.LogError("Wrong type to disable");
    }
}
