using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : ActionBase
{
    public override void OnClick()
    {
        m_xOwner.Attack();
    }

    public override void Update()
    {
        base.Update();
        gameObject.SetActive(m_xOwner.IsHackable());
    }
}
