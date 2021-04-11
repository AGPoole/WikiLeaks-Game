﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceIcon : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.Text m_xDefenceText;
    Edge m_xOwner;

    bool m_bCyberSecDriven = false;
    CyberSecurity m_xCyberSecurityOwner;
    [SerializeField]
    int m_iDefence = 10;
    [SerializeField]
    int m_iMaxDefence = 0;
    [SerializeField]
    float m_fAdditionalDefenceDegradationTime = 0.5f;
    [SerializeField]
    float m_fDefenceRechargeTime = 0.5f;
    //TODO: show when it's about to recharge
    [SerializeField]
    float m_fDefencePauseAtZero = 10f;

    float m_fRechargeTimer = 0;
    float m_fDeteriorationTimer = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_iDefence = m_iMaxDefence;
    }

    public void SetOwner(Edge xOwner)
    {
        m_xOwner = xOwner;
    }

    public void SetMaxDefense(int iMax)
    {
        m_iMaxDefence = iMax;
    }

    public void SetDefenceRechargeTime(float fRechargeTime)
    {
        m_fDefenceRechargeTime = fRechargeTime;
    }

    public void SetDefenceDegradationTime(float fDegradationTime)
    {
        m_fAdditionalDefenceDegradationTime = fDegradationTime;
    }
    // Update is called once per frame
    void Update()
    {
        m_xDefenceText.text = m_iDefence.ToString();
        if (!Manager.GetIsPaused())
        {
            //TODO: make Cybersecurity add more
            if (m_iDefence < m_iMaxDefence)
            {
                m_fRechargeTimer += Time.deltaTime;
                if ((m_iDefence != 0 && m_fRechargeTimer > m_fDefenceRechargeTime)
                    || m_fRechargeTimer > m_fDefencePauseAtZero)
                {
                    m_fRechargeTimer -= m_iDefence == 0 ? m_fDefencePauseAtZero : m_fDefenceRechargeTime;
                    m_iDefence++;
                }
            }
            else
            {
                m_fRechargeTimer = 0;
            }
            if (m_iDefence > m_iMaxDefence)
            {
                m_fDeteriorationTimer += Time.deltaTime;
                if (m_fDeteriorationTimer > m_fAdditionalDefenceDegradationTime)
                {
                    m_fDeteriorationTimer -= m_fAdditionalDefenceDegradationTime;
                    m_iDefence++;
                }
            }
            else
            {
                m_fDeteriorationTimer = 0;
            }
        }
    }

    public int GetDefence()
    {
        return m_iDefence;
    }

    public void Attack()
    {
        if (Manager.GetManager().GetHacksLeft() < 1)
        {
            return;
        }
        
        if (m_xOwner.GetStart().IsHacked() || m_xOwner.GetEnd().IsHacked())
        {
            Manager.GetManager().ChangeHacks(-1);
            m_iDefence--;
            m_fRechargeTimer = 0;
            if (m_iDefence < 0)
            {
                m_iDefence = 0;
                m_xOwner.CheckDefences();
            }
        }
    }

    // TODO: remove this
    public void Defend()
    {
        if (m_iDefence < m_iMaxDefence)
        {
            m_iDefence++;
        }
    }
}