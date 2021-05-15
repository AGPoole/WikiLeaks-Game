using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceIcon : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.Text m_xDefenceText;
    Edge m_xOwner;

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

    [SerializeField]
    GameObject m_xTripWireContainer;
    [SerializeField]
    UnityEngine.UI.Text m_xTripWireProbability;
    [SerializeField]
    UnityEngine.UI.Text m_xTripWireDamage;

    [SerializeField]
    bool m_bHasTripWire;
    [Range(0, 1)]
    [SerializeField]
    float m_fTripWireProbability;
    [SerializeField]
    int m_iTripWireDamage;

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
        if (m_xCyberSecurityOwner != null)
        {
            SetDefenceDegradationTime(m_xCyberSecurityOwner.GetAdditionalDefenceDegradationTime());
            SetDefenceRechargeTime(m_xCyberSecurityOwner.GetDefenceRechargeTime());
            SetMaxDefense(m_xCyberSecurityOwner.GetMaxDefenceForEdge(m_xOwner));
            if (m_iMaxDefence == 0)
            {
                m_xOwner.RemoveDefenceIcon(this);
            }
        }
        if (!Manager.GetIsPaused())
        {
            if (m_iDefence < m_iMaxDefence)
            {
                // TODO: make recharges happen per turn
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
        m_xDefenceText.text = m_iDefence.ToString();
        m_xTripWireContainer.SetActive(m_bHasTripWire);
        if (m_bHasTripWire)
        {
            m_xTripWireDamage.text = string.Format("-{0}", m_iTripWireDamage);
            m_xTripWireProbability.text = string.Format("{0}%", (m_fTripWireProbability*100).ToString("0"));
        }
    }

    public int GetDefence()
    {
        return m_iDefence;
    }

    public bool Attack(int iDamage, bool bDetectable)
    {
        if (!m_xOwner.IsReachable(this))
        {
            return false;
        }
        m_iDefence-=iDamage;
        m_fRechargeTimer = 0;
        if (bDetectable && m_bHasTripWire && Random.Range(0f, 1f)<m_fTripWireProbability)
        {
            Manager.GetManager().ChangeAlert(-m_iTripWireDamage);
            m_bHasTripWire = false;
        }
        if (m_iDefence <= 0)
        {
            m_iDefence = 0;
            m_xOwner.CheckDefences();
        }
        return true;
    }

    // TODO: remove this
    public void Defend()
    {
        if (m_iDefence < m_iMaxDefence)
        {
            m_iDefence++;
        }
    }

    public void SetCyberSecurityOwner(CyberSecurity xSec)
    {
        m_xCyberSecurityOwner = xSec;
        SetMaxDefense(m_xCyberSecurityOwner.GetMaxDefenceForEdge(m_xOwner));
    }

    public CyberSecurity GetCyberSecurityOwner()
    {
        return m_xCyberSecurityOwner;
    }

    public void OnClick()
    {
        if (WeaponManager.GetWeaponManager().GetSelectedWeapon() is WeaponBase<DefenceIcon>) 
        { 
            var xWeapon = (WeaponBase<DefenceIcon>)WeaponManager.GetWeaponManager().GetSelectedWeapon();
            xWeapon.Use(this);
        }
    }

    public void AddTripWire(float fProb, int iDamage)
    {
        m_bHasTripWire = true;
        m_fTripWireProbability = fProb;
        m_iTripWireDamage = iDamage;
    }

    public bool HasTripWire()
    {
        return m_bHasTripWire;
    }

    public void DisarmTripWire(float fProbDecrease, int iDamageDecrease)
    {
        if(fProbDecrease<0f || iDamageDecrease < 0)
        {
            Debug.LogError("Incorrect values put into disarm");
        }
        if (m_bHasTripWire)
        {
            m_fTripWireProbability -= fProbDecrease;
            m_iTripWireDamage -= iDamageDecrease;
            if(m_iTripWireDamage<=0 || m_fTripWireProbability <= 0)
            {
                m_bHasTripWire = false;
            }
        }
    }
}