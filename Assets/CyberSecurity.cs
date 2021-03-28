using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberSecurity : SystemBase
{
    List<SystemBase> m_xSystemQueue;
    float m_fSystemTimer;
    float m_fSystemTimerReset = 0.1f;

    protected override void Start()
    {
        base.Start();
        m_xSystemQueue = new List<SystemBase>();
    }

    protected override SystemValuesBase GetMyValues()
    {
        return CyberSecurityValuesContainer.GetCyberSecurityValues();
    }

    int iApplyDefenceTimer = 0;
    public override void OnNextTurn(int iLevel)
    {
        base.OnNextTurn(iLevel);
        if (!m_bDisabledByPlayer)
        {
            iApplyDefenceTimer -= 1;
        }
        if (iApplyDefenceTimer <= 0)
        {
            var xSystemBases = GetAllSystems();
            for(int i=xSystemBases.Count-1; i>=0; i--)
            {
                if(xSystemBases[i]==this || xSystemBases[i].GetLevel() == 0)
                {
                    xSystemBases.RemoveAt(i);
                }
            }
            int iTotal = 0;
            foreach(var xSys in xSystemBases)
            {
                iTotal += CyberSecurityValuesContainer.GetCyberSecurityValues().GetScoreAtDistance((xSys.transform.position - transform.position).magnitude);
            }

            for (int i=0; i<CyberSecurityValuesContainer.GetCyberSecurityValues().GetDefenceGainAtLevel(m_iLevel, Manager.GetManager().GetTechLevel()); i++)
            {
                int iChoice = Random.Range(0, iTotal);
                int iScoreSoFar = 0;
                foreach (var xSys in xSystemBases)
                {
                    iScoreSoFar += CyberSecurityValuesContainer.GetCyberSecurityValues().GetScoreAtDistance((xSys.transform.position - transform.position).magnitude);
                    if (iScoreSoFar > iChoice)
                    {
                        m_xSystemQueue.Add(xSys);
                        break;
                    }
                }
            }
            iApplyDefenceTimer = CyberSecurityValuesContainer.GetCyberSecurityValues().GetLaunchTime();
        }
    }

    protected override void Update()
    {
        base.Update();
        // TODO: suspend if paused
        m_fSystemTimer -= Time.deltaTime;
        if (m_xSystemQueue.Count > 0 && m_fSystemTimer<0)
        {
            GameObject xMessage = Instantiate(CyberSecurityValuesContainer.GetCyberSecurityValues().GetMessagePrefab(), transform);
            xMessage.GetComponent<MessageIcon>().SetCallBack(m_xSystemQueue[0].Defend);
            xMessage.GetComponent<MessageIcon>().SetTarget(m_xSystemQueue[0].transform);
            m_fSystemTimer = m_fSystemTimerReset;
            m_xSystemQueue.RemoveAt(0);
        }
    }

    bool m_bDisabledByPlayer = false;
    public void SetDisabledByPlayer(bool bDisabled)
    {
        m_bDisabledByPlayer = bDisabled;
    }

    public bool IsDisabledByPlayer()
    {
        return m_bDisabledByPlayer;
    }
}
