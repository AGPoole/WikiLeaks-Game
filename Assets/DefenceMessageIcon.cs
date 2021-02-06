using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DefenceMessageIcon : MessageIcon
{
    protected override void OnTargetReached()
    {
        m_xTarget.gameObject.GetComponent<SystemBase>().Defend();
    }
}
