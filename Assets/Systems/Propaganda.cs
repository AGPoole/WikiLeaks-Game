﻿using UnityEngine;

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

    int m_iCoolDownCompletionTime = 1;

    public override void Init()
    {
        base.Init();
        m_xGov = GetOwner().GetCountry().GetGovernment();
    }

    protected override void OnDeactivation()
    {
        m_bActive = false;
        if (m_xActiveIndicator != null)
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
        if (m_bActive && m_iLevel > 0)
        {
            m_fActiveIndicatorNextSwitchTime -= Time.deltaTime;
            if (m_fActiveIndicatorNextSwitchTime < 0f)
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
        if (!m_bActive && Manager.GetTurnNumber() > m_iCoolDownCompletionTime)
        {
            if (m_xGov == null)
            {
                Debug.LogError("Missing Government");
                m_xGov = GetOwner().GetCountry().GetGovernment();
            }
            m_bActive = true;
            GameObject xPropagandaMessage = Instantiate(PropagandaValuesContainer.GetPropagandaValues(m_eType).GetPropagandaMessageObject(), transform);
            xPropagandaMessage.GetComponent<MessageIcon>().SetSource(transform);
            Orientation eOrientation = m_eType == PropagandaValuesContainer.ObjectType.Government ? Orientation.LEFT : Orientation.RIGHT;
            xPropagandaMessage.GetComponent<MessageIcon>().SetTarget(m_xGov.GetTarget(eOrientation));
            xPropagandaMessage.GetComponent<MessageIcon>().SetCallBack(
                () =>
                {
                    var xPopMod = gameObject.GetComponent<Propaganda>().CreatePopularityModifier();
                    m_xGov.AddModifier(xPopMod);
                }
            );
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
            Debug.LogWarning("Propaganda disabled twice - this shouldn't happen");
        }

        m_bActive = false;
        m_iCoolDownCompletionTime = Manager.GetTurnNumber() + PropagandaValuesContainer.GetPropagandaValues(m_eType).GetCooldownLength();
        if (m_xActiveIndicator != null)
            m_xActiveIndicator.SetActive(false);
    }

    public override SystemValuesBase GetMyValues()
    {
        return PropagandaValuesContainer.GetPropagandaValues(m_eType);
    }

    public PopularityModifier CreatePopularityModifier()
    {
        Orientation eOrientation = m_eType == PropagandaValuesContainer.ObjectType.Government ? Orientation.LEFT : Orientation.RIGHT;
        return new PropagandaPopMod(PropagandaValuesContainer.GetPropagandaValues(m_eType).GetPropagandaStrength(), eOrientation, PropagandaValuesContainer.GetPropagandaValues(m_eType).GetPropagandaLength(), this);
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
