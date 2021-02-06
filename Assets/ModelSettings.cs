using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelSettings : MonoBehaviour
{
    static ModelSettings s_xModelSettings;

    [SerializeField]
    bool m_bElectionInfluenceEnabled = true;
    [SerializeField]
    bool m_bDisastersEnabled = true;
    [SerializeField]
    bool m_bElectionsEnabled= true;

    void Awake()
    {
        s_xModelSettings = this;
    }

    public static bool GetElectionInfluenceEnabled()
    {
        return s_xModelSettings.m_bElectionInfluenceEnabled;
    }

    public static bool GetDisastersEnabled()
    {
        return s_xModelSettings.m_bDisastersEnabled;
    }
    public static bool GetElectionsEnabled()
    {
        return s_xModelSettings.m_bElectionsEnabled;
    }
}
