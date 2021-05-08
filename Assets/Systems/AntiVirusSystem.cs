using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiVirusSystem : SystemBase, IDisablable
{
    SystemBase m_xSystemTarget;
    float m_fSystemTimer;
    float m_fSystemTimerReset = 0.1f;

    public override SystemValuesBase GetMyValues()
    {
        return AntiVirusValuesContainer.GetAntiVirusValues();
    }

    int iApplyDefenceTimer = 0;
    public override void OnNextTurn(int iLevel)
    {
        base.OnNextTurn(iLevel);
        if (!m_bDisabledByPlayer && !IsForceDisabled())
        {
            iApplyDefenceTimer -= 1;
        }
        if (iApplyDefenceTimer <= 0)
        {
            var xSystemBases = GetAllSystems();
            if (!IsValidTarget(m_xSystemTarget))
            {
                m_xSystemTarget = null;
            }
            for (int i = xSystemBases.Count - 1; i >= 0; i--)
            {
                if (!IsValidTarget(xSystemBases[i]))
                {
                    xSystemBases.RemoveAt(i);
                }
                else if ((m_xSystemTarget==null
                    || GetDistanceTo(xSystemBases[i]) < GetDistanceTo(m_xSystemTarget))
                    && GetDistanceTo(xSystemBases[i]) <= AntiVirusValuesContainer.GetAntiVirusValues().GetRange())
                {
                    m_xSystemTarget = xSystemBases[i];
                }
            }

            iApplyDefenceTimer = AntiVirusValuesContainer.GetAntiVirusValues().GetLaunchTime();
        }
    }

    bool IsValidTarget(SystemBase xTarget)
    {
        return xTarget != null
            && xTarget != this
            && xTarget.GetLevel() > 0
            && xTarget.IsHacked();
    }

    protected override void Update()
    {
        base.Update();
        // TODO: suspend if paused
        m_fSystemTimer -= Time.deltaTime;
        if (m_xSystemTarget!=null && m_fSystemTimer < 0)
        {
            GameObject xMessage = Instantiate(AntiVirusValuesContainer.GetAntiVirusValues().GetMessagePrefab(), transform);

            // copy variable to avoid C# lambda issues
            var xTargetVariable = m_xSystemTarget;
            // TODO: change this
            xMessage.GetComponent<MessageIcon>().SetCallBack(
                () =>
                {
                    xTargetVariable.Attack(false, AntiVirusValuesContainer.GetAntiVirusValues().GetDamageAtLevel(GetLevel(), Manager.GetManager().GetTechLevel()), true);
                }
            );
            xMessage.GetComponent<MessageIcon>().SetTarget(m_xSystemTarget.transform);

            m_fSystemTimer = m_fSystemTimerReset;
            //TODO: change this - this system does not need system timer
            m_xSystemTarget = null;
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

    int m_iNoLongerForceDisabled = 0;
    public void ForceDisable(int iNumTurns)
    {
        m_iNoLongerForceDisabled = Manager.GetTurnNumber() + iNumTurns;
    }

    public bool IsForceDisabled()
    {
        return Manager.GetTurnNumber() < m_iNoLongerForceDisabled;
    }
}
