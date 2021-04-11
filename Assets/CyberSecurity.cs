using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberSecurity : SystemBase
{
    public override SystemValuesBase GetMyValues()
    {
        return CyberSecurityValuesContainer.GetCyberSecurityValues();
    }

    float m_fEdgeRecalculateTimer = 0f;
    protected override void Update()
    {
        const float fEDGE_CALCULATION_TIME_GAP = 0.5f;
        if (!Manager.GetIsPaused() && !m_bDisabledByPlayer)
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
        foreach(Edge xEdge in Edge.GetAllEdges())
        {
            if((xEdge.GetPosition()-transform.position).magnitude < CyberSecurityValuesContainer.GetCyberSecurityValues().GetMaxLength())
            {
                xEdge.RegisterCyberSec(this);
            }
        }
    }

    public int GetMaxDefenceForEdge(Edge xEdge)
    {
        if (m_bDisabledByPlayer)
        {
            return 0;
        }
        return CyberSecurityValuesContainer.GetCyberSecurityValues().GetDefenceGainAtLevelForDistance(
            GetLevel(), 
            Manager.GetManager().GetTechLevel(),
            (xEdge.GetPosition()-transform.position).magnitude);
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
}
