using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceIcon : MonoBehaviour
{
    static List<DefenceIcon> s_xDefenceIcons = new List<DefenceIcon>();
    [SerializeField]
    UnityEngine.UI.Text m_xDefenceText;
    Edge m_xOwner;

    CyberSecurity m_xCyberSecurityOwner;
    [SerializeField]
    int m_iDefence = 10;
    [SerializeField]
    int m_iBaseMaxDefence = 0;
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

    [System.Serializable]
    class Backdoor
    {
        public GameObject m_xGameObject;
        public UnityEngine.UI.Text m_xReqirementText;
        public UnityEngine.UI.Text m_xAdditionText;
        public int m_iRequirement;
        public int m_iAddition;
    }

    [SerializeField]
    Backdoor m_xDataBackdoor;
    [SerializeField]
    Backdoor m_xMoneyBackdoor;


    float m_fRechargeTimer = 0;
    float m_fDeteriorationTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        s_xDefenceIcons.Add(this);
        m_iDefence = CalculateActualMaxDefence();
    }

    public void SetOwner(Edge xOwner)
    {
        m_xOwner = xOwner;
    }

    public void SetBaseMaxDefense(int iMax)
    {
        m_iBaseMaxDefence = iMax;
    }

    public int CalculateActualMaxDefence()
    {
        int iMax = m_iBaseMaxDefence;
        if (m_xDataBackdoor.m_iAddition > 0)
        {
            iMax += m_xDataBackdoor.m_iAddition;
        }
        if (m_xMoneyBackdoor.m_iAddition > 0)
        {
            iMax += m_xMoneyBackdoor.m_iAddition;
        }
        return iMax;
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
        if (!Manager.GetIsPaused())
        {
            if (m_iDefence < CalculateActualMaxDefence())
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
            if (m_iDefence > CalculateActualMaxDefence())
            {
                m_fDeteriorationTimer += Time.deltaTime;
                if (m_fDeteriorationTimer > m_fAdditionalDefenceDegradationTime)
                {
                    m_fDeteriorationTimer -= m_fAdditionalDefenceDegradationTime;
                    m_iDefence--;
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

        m_xDataBackdoor.m_xGameObject.SetActive(m_xDataBackdoor.m_iRequirement > 0);
        if (m_xDataBackdoor.m_xGameObject.activeSelf)
        {
            m_xDataBackdoor.m_xReqirementText.text = string.Format("{0}", m_xDataBackdoor.m_iRequirement);
            m_xDataBackdoor.m_xAdditionText.text = string.Format("+{0}", m_xDataBackdoor.m_iAddition);
        }
        
        m_xMoneyBackdoor.m_xGameObject.SetActive(m_xMoneyBackdoor.m_iRequirement > 0);
        if (m_xMoneyBackdoor.m_xGameObject.activeSelf)
        {
            m_xMoneyBackdoor.m_xReqirementText.text = string.Format("${0}", m_xMoneyBackdoor.m_iRequirement);
            m_xMoneyBackdoor.m_xAdditionText.text = string.Format("+{0}", m_xMoneyBackdoor.m_iAddition);
        }
    }

    public static void OnNextTurnAll()
    {
        foreach(DefenceIcon xDefenceIcon in s_xDefenceIcons)
        {
            xDefenceIcon.OnNextTurn();
        }
    }

    static int s_iRefreshTime = 1000;
    int m_iNextRefreshTime = 0;
    void OnNextTurn()
    {
        if (m_xCyberSecurityOwner != null && !m_xCyberSecurityOwner.IsInUse())
        {
            m_xOwner.RemoveDefenceIcon(this);
        }
        if (m_xOwner != null)
        {
            if (Manager.GetTurnNumber() > m_iNextRefreshTime)
            {
                if (m_xCyberSecurityOwner != null && !m_xCyberSecurityOwner.IsDisabledByPlayer() )
                {
                    SetDefenceDegradationTime(m_xCyberSecurityOwner.GetAdditionalDefenceDegradationTime());
                    SetDefenceRechargeTime(m_xCyberSecurityOwner.GetDefenceRechargeTime());
                    SetBaseMaxDefense(m_xCyberSecurityOwner.GetMaxDefenceForEdge(m_xOwner));
                    if (CalculateActualMaxDefence() == 0)
                    {
                        m_xOwner.RemoveDefenceIcon(this);
                    }
                }
                else
                {
                    SystemValuesBase xStartValues = m_xOwner.GetStart().GetMyValues();
                    SystemValuesBase xEndValues = m_xOwner.GetEnd().GetMyValues();
                    SetBaseMaxDefense(ProjectMaths.Max(
                        xStartValues.GetBaseDefenceMax(),
                        xEndValues.GetBaseDefenceMax()
                        ));
                    SetDefenceDegradationTime(Mathf.Max(
                        xStartValues.GetAdditionalShielddeteriorationTime(),
                        xEndValues.GetAdditionalShielddeteriorationTime()
                        ));
                    SetDefenceRechargeTime(Mathf.Min(
                        xStartValues.GetBaseDefenceRefreshTime(),
                        xEndValues.GetBaseDefenceRefreshTime()
                        ));
                    float fTripWireExistenceProb = Mathf.Max(
                        xStartValues.GetTripWireExistanceProbability(),
                        xEndValues.GetTripWireExistanceProbability());
                    if (UnityEngine.Random.Range(0f, 1f) < fTripWireExistenceProb)
                    {
                        float fTripWireCatchProb = Mathf.Max(
                        xStartValues.GetTripWireCatchProbability(),
                        xEndValues.GetTripWireCatchProbability());
                        int iTripWireDamage = ProjectMaths.Max(
                        xStartValues.GetTripWireDamage(),
                        xEndValues.GetTripWireDamage());
                        AddTripWire(fTripWireCatchProb, iTripWireDamage);
                    }
                    else
                    {
                        RemoveTripWire();
                    }
                    float fMoneyRequirementProb = Mathf.Max(
                        xStartValues.GetMoneyRequirementProbability(),
                        xEndValues.GetMoneyRequirementProbability());
                    if (UnityEngine.Random.Range(0f, 1f) < fMoneyRequirementProb)
                    {
                        int iCost = ProjectMaths.Max(
                        xStartValues.GetMoneyRequirementCost(),
                        xEndValues.GetMoneyRequirementCost());
                        // TODO: use better method than choosing best increase and decrease - use same system for both
                        int iIncrease = ProjectMaths.Max(
                        xStartValues.GetMoneyRequirementAddition(),
                        xEndValues.GetMoneyRequirementAddition());
                        int iInitialDecrease = ProjectMaths.Max(
                        xStartValues.GetMoneyRequirementInitialDecrease(),
                        xEndValues.GetMoneyRequirementInitialDecrease());
                        m_iBaseMaxDefence -= iInitialDecrease;
                        m_xMoneyBackdoor.m_iAddition = iIncrease;
                        m_xMoneyBackdoor.m_iRequirement = iCost;
                    }
                    else
                    {
                        m_xMoneyBackdoor.m_iAddition = 0;
                        m_xMoneyBackdoor.m_iRequirement = 0;
                    }
                    float fDataRequirementProb = Mathf.Max(
                        xStartValues.GetDataRequirementProbability(),
                        xEndValues.GetDataRequirementProbability());
                    if (UnityEngine.Random.Range(0f, 1f) < fDataRequirementProb)
                    {
                        int iCost = ProjectMaths.Max(
                        xStartValues.GetDataRequirementCost(),
                        xEndValues.GetDataRequirementCost());
                        // TODO: use better method than choosing best increase and decrease - use same system for both
                        int iIncrease = ProjectMaths.Max(
                        xStartValues.GetDataRequirementAddition(),
                        xEndValues.GetDataRequirementAddition());
                        int iInitialDecrease = ProjectMaths.Max(
                        xStartValues.GetDataRequirementInitialDecrease(),
                        xEndValues.GetDataRequirementInitialDecrease());
                        m_iBaseMaxDefence -= iInitialDecrease;
                        m_xDataBackdoor.m_iAddition = iIncrease;
                        m_xDataBackdoor.m_iRequirement = iCost;
                    }
                    else
                    {
                        m_xDataBackdoor.m_iAddition = 0;
                        m_xDataBackdoor.m_iRequirement = 0;
                    }
                }
                m_iNextRefreshTime += s_iRefreshTime;
            }
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
        if (m_iDefence < CalculateActualMaxDefence())
        {
            m_iDefence++;
        }
    }

    public void SetCyberSecurityOwner(CyberSecurity xSec)
    {
        m_xCyberSecurityOwner = xSec;
        SetBaseMaxDefense(m_xCyberSecurityOwner.GetMaxDefenceForEdge(m_xOwner));
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

    public void RemoveTripWire()
    {
        m_bHasTripWire = false;
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

    public void AttemptDataBackdoor()
    {
        if (Manager.GetManager().GetData() >= m_xDataBackdoor.m_iRequirement)
        {
            Manager.GetManager().ChangeData(-m_xDataBackdoor.m_iRequirement);
            m_xDataBackdoor.m_iAddition = 0;
            m_xDataBackdoor.m_iRequirement = 0;
        }
    }
    public void AttemptMoneyBackdoor()
    {
        if (Manager.GetManager().GetMoney() >= m_xMoneyBackdoor.m_iRequirement)
        {
            Manager.GetManager().ChangeMoney(-m_xMoneyBackdoor.m_iRequirement);
            m_xMoneyBackdoor.m_iAddition = 0;
            m_xMoneyBackdoor.m_iRequirement = 0;
        }
    }

    public void OnDestroy()
    {
        s_xDefenceIcons.Remove(this);
    }
}