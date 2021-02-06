using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSettings : MonoBehaviour
{
    static DebugSettings s_xDebugSettings;

    [SerializeField]
    bool m_bLogTurnEnd;
    [SerializeField]
    bool m_bDetectFakeChanges = true;

    void Awake()
    {
        s_xDebugSettings = this;
    }

    public static bool ShouldLogTurnEnd()
    {
        return s_xDebugSettings.m_bLogTurnEnd;
    }
    
    public static bool ShouldDetectFakeChanges()
    {
        return s_xDebugSettings.m_bDetectFakeChanges;
    }
}
