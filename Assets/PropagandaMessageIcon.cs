using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropagandaMessageIcon : MessageIcon
{
    Government m_xGovernment;
    public void SetGovernment(Government xGov)
    {
        m_xGovernment = xGov;
    }
    protected override void OnTargetReached()
    {
        var xPopMod = m_xSource.gameObject.GetComponent<Propaganda>().CreatePopularityModifier();
        m_xGovernment.AddModifier(xPopMod);
    }
}
