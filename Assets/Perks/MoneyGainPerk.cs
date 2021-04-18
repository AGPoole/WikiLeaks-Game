using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoneyGainPerk : PerkBase
{
    [SerializeField]
    int m_iMoneyGain = 5;
    [SerializeField]
    int m_iTurnsBetweenMoneyGain = 10;
    int m_iLastMoneyGain = 0;
    public override void OnNextTurn()
    {
        base.OnNextTurn();
        if (Manager.GetTurnNumber() >= m_iLastMoneyGain + m_iTurnsBetweenMoneyGain)
        {
            Manager.GetManager().ChangeMoney(m_iMoneyGain);
            m_iLastMoneyGain = Manager.GetTurnNumber();
        }
    }
}