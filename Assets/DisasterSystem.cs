using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisasterSystem : MonoBehaviour
{
    [SerializeField]
    Disaster[] m_xDisasters;
    [SerializeField]
    float m_fEventRate = 0.01f;

    static DisasterSystem s_xDisasterSystem;

    public static DisasterSystem GetDisasterSystem()
    {
        if (s_xDisasterSystem == null)
        {
            s_xDisasterSystem = FindObjectOfType<DisasterSystem>() as DisasterSystem;
        }
        return s_xDisasterSystem;
    }

    public static void ActivateDisasters(Country xCountry)
    {
        if (!ModelSettings.GetDisastersEnabled())
        {
            return;
        }
        s_xDisasterSystem = GetDisasterSystem();

        int iSum = 0;
        
        foreach(var xDisaster in s_xDisasterSystem.m_xDisasters)
        {
            iSum += xDisaster.GetProbScore();
        }
        foreach(var xDisaster in s_xDisasterSystem.m_xDisasters)
        {
            if(Random.Range(0f, 1f) < xDisaster.GetProbScore()* s_xDisasterSystem.m_fEventRate / iSum)
            {
                xDisaster.Activate(xCountry);
            }
        }
    }

    [System.Serializable]
    public class Disaster
    {
        [SerializeField]
        Orientation m_eOrientation;
        [SerializeField]
        float m_fEffect;
        [SerializeField]
        int m_iLength;
        [SerializeField]
        string m_xText;
        [SerializeField]
        string m_xDescription;
        [SerializeField]
        int m_iProbabilityScore;

        public int GetProbScore()
        {
            return m_iProbabilityScore;
        }

        public void Activate(Country m_xCountry)
        {
            m_xCountry.GetGovernment().AddModifier(new PopularityModifier(m_fEffect, m_eOrientation, m_iLength, m_xText, true));
            NotificationSystem.AddNotification(m_xText, m_xDescription);
        }
    }
}
