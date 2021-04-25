using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : ActionBase
{
    public override void OnClick()
    {
        m_xSystemOwner.Attack(true);
    }
}
