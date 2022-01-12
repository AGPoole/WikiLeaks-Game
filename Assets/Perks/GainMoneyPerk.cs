using UnityEngine;

public class GainMoneyPerk : PerkBase
{
    [SerializeField]
    int m_iMoneyGain = 5;
    [SerializeField]
    int m_iTurnsBetweenMoneyGain = 10;
    int m_iLastMoneyGain = 0;
    public override void OnNextTurn()
    {
        if (Manager.GetTurnNumber() >= m_iLastMoneyGain + m_iTurnsBetweenMoneyGain)
        {
            Manager.GetManager().ChangeMoney(m_iMoneyGain);
            m_iLastMoneyGain = Manager.GetTurnNumber();
        }
    }
}
