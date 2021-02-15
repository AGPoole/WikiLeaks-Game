using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propaganda : SystemBase
{
    [SerializeField]
    PropagandaValuesContainer.ObjectType m_eType; 

    // TODO: remove this
    [SerializeField]
    GameObject m_xActiveIndicator;

    [SerializeField]
    bool m_bActive;
    [SerializeField]
    Government m_xGov;

    int m_iCoolDownCompletionTime=1;

    protected override void OnDeactivation()
    {
        m_bActive = false;
        if(m_xActiveIndicator!=null)
            m_xActiveIndicator.SetActive(false);
        base.OnDeactivation();
    }

    protected override void OnActivation()
    {
        m_bActive = false;
        if (m_xActiveIndicator != null)
            m_xActiveIndicator.SetActive(false);
        m_iCoolDownCompletionTime = Manager.GetTurnNumber() + PropagandaValuesContainer.GetPropagandaValues(m_eType).GetCooldownLength();
        base.OnActivation();
    }

    float m_fActiveIndicatorNextSwitchTime = 0f;
    [SerializeField]
    float m_fActiveIndicatorSwitchDuration;
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (m_bActive && m_iLevel>0)
        {
            m_fActiveIndicatorNextSwitchTime -= Time.deltaTime;
            if(m_fActiveIndicatorNextSwitchTime < 0f)
            {
                if (m_xActiveIndicator != null)
                    m_xActiveIndicator.SetActive(!m_xActiveIndicator.activeSelf);
                m_fActiveIndicatorNextSwitchTime = m_fActiveIndicatorSwitchDuration;
            }
        }
    }

    public override void OnNextTurn(int iLevel)
    {
        base.OnNextTurn(iLevel);
        if (m_iLevel == 0)
        {
            return;
        }
        if (!m_bActive && Manager.GetTurnNumber()>m_iCoolDownCompletionTime)
        {
            m_bActive = true;
            Orientation eOrientation = m_eType == PropagandaValuesContainer.ObjectType.Government ? Orientation.LEFT : Orientation.RIGHT;
            m_xGov.AddModifier(new PropagandaPopMod(PropagandaValuesContainer.GetPropagandaValues(m_eType).GetPropagandaStrength(), eOrientation, PropagandaValuesContainer.GetPropagandaValues(m_eType).GetPropagandaLength(), this));
        }
    }

    public void OnModifierRemoval()
    {
        if (m_iLevel == 0)
        {
            return;
        }
        if (!m_bActive)
        {
            Debug.LogError("Propaganda disabled twice - this shouldn't happen");
        }

        m_bActive = false;
        m_iCoolDownCompletionTime = Manager.GetTurnNumber() + PropagandaValuesContainer.GetPropagandaValues(m_eType).GetCooldownLength();
        if (m_xActiveIndicator != null)
            m_xActiveIndicator.SetActive(false);
    }

    protected override SystemValuesBase GetMyValues()
    {
        return PropagandaValuesContainer.GetPropagandaValues(m_eType);
    }

    class PropagandaPopMod : PopularityModifier
    {
        Propaganda m_xPropaganda;
        public PropagandaPopMod(float fEffect, Orientation eOrientation, int iRemovalTurn, Propaganda xProp)
            : base(fEffect, eOrientation, iRemovalTurn, "Propaganda", true)
        {
            m_xPropaganda = xProp;
        }

        public override void OnRemoval()
        {
            base.OnRemoval();
            m_xPropaganda.OnModifierRemoval();
        }
    }
}
