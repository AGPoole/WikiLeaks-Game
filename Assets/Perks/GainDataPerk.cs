using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainDataPerk : PerkBase
{
    [SerializeField]
    int m_iDataGain = 5;
    [SerializeField]
    int m_iTurnsBetweenDataGain = 10;
    int m_iLastDataGain = 0;
    public override void OnNextTurn()
    {
        if (Manager.GetTurnNumber() >= m_iLastDataGain + m_iTurnsBetweenDataGain)
        {
            Manager.GetManager().ChangeData(m_iDataGain);
            m_iLastDataGain = Manager.GetTurnNumber();
        }
    }
}
