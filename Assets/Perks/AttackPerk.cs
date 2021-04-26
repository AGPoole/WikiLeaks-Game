using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPerk : PerkBase
{
    public override void OnClick()
    {
        m_xSystemOwner.Attack(true);
    }
}
