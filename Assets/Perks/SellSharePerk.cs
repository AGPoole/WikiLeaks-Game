using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellSharePerk : PerkBase
{
    public override void OnClick()
    {
        var xFinanceOwner = m_xSystemOwner.GetComponent<Finance>();
        if (xFinanceOwner != null)
        {
            xFinanceOwner.SellShare();
            return;
        }
        Debug.LogError("Wrong type to disable");
    }
}
