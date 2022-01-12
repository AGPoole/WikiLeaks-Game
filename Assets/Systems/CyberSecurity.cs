using System.Collections.Generic;
using UnityEngine;

public class CyberSecurity : SystemBase, IDisablable
{
    public override SystemValuesBase GetMyValues()
    {
        return CyberSecurityValuesContainer.GetCyberSecurityValues();
    }

    float m_fEdgeRecalculateTimer = 0f;
    protected override void Update()
    {
        // TODO: change to number of turns
        const float fEDGE_CALCULATION_TIME_GAP = 2f;
        if (!Manager.GetIsPaused() && !m_bDisabledByPlayer && !IsForceDisabled())
        {
            m_fEdgeRecalculateTimer += Time.deltaTime;
            if (m_fEdgeRecalculateTimer > fEDGE_CALCULATION_TIME_GAP)
            {
                m_fEdgeRecalculateTimer -= fEDGE_CALCULATION_TIME_GAP;
                CalculateEdges();
            }
        }
        base.Update();
    }

    void CalculateEdges()
    {
        // TODO: only iterate over nearby edges
        ref readonly List<Edge> xEdges = ref Edge.GetAllEdges();
        foreach (Edge xEdge in xEdges)
        {
            if ((xEdge.GetPosition() - transform.position).magnitude < CyberSecurityValuesContainer.GetCyberSecurityValues().GetMaxLength())
            {
                xEdge.RegisterCyberSec(this);
            }
        }
    }

    public int GetMaxDefenceForEdge(Edge xEdge)
    {
        if (m_bDisabledByPlayer || IsForceDisabled())
        {
            return 0;
        }
        return CyberSecurityValuesContainer.GetCyberSecurityValues().GetDefenceGainAtLevelForDistance(
            GetLevel(),
            Manager.GetManager().GetTechLevel(),
            (xEdge.GetPosition() - transform.position).magnitude);
    }

    public float GetAdditionalDefenceDegradationTime()
    {
        return CyberSecurityValuesContainer.GetCyberSecurityValues().GetAdditionalDefenceDegradationTime();
    }

    public float GetDefenceRechargeTime()
    {
        return CyberSecurityValuesContainer.GetCyberSecurityValues().GetDefenceRechargeTime();
    }

    bool m_bDisabledByPlayer = false;
    public void SetDisabledByPlayer(bool bDisabled)
    {
        m_bDisabledByPlayer = bDisabled;
        if (!m_bDisabledByPlayer)
        {
            CalculateEdges();
        }
    }

    public bool IsDisabledByPlayer()
    {
        return m_bDisabledByPlayer;
    }

    public bool IsInUse()
    {
        return !m_bDisabledByPlayer && GetLevel() > 0 && !IsForceDisabled();
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
