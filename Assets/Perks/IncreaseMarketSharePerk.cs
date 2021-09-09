using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseMarketSharePerk : PerkBase
{
    public override void OnClick()
    {
        ((TechCompany)m_xSystemOwner.GetOwner()).ChangeMarketShare(10f, true);
    }
}
