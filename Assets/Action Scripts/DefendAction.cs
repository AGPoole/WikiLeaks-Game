using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendAction : ActionBase
{
    public override void OnClick()
    {
        m_xOwner.Defend();
    }
}

