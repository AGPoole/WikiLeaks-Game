using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendPerk : PerkBase
{
    public override void OnClick()
    {
        m_xSystemOwner.Defend();
    }
}

