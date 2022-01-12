using UnityEngine;

[System.Serializable]
public class PropagandaValues : SystemValuesBase
{
    [SerializeField]
    int m_iPropagandaLength = 50;
    [SerializeField]
    float m_fPropangandaStrength = 0.1f;
    [SerializeField]
    int m_iCoolDownLength = 70;
    [SerializeField]
    GameObject m_xPropagandaMessageObject;

    public int GetPropagandaLength()
    {
        return m_iPropagandaLength;
    }
    public float GetPropagandaStrength()
    {
        return m_fPropangandaStrength;
    }
    public int GetCooldownLength()
    {
        return m_iCoolDownLength;
    }

    public GameObject GetPropagandaMessageObject()
    {
        return m_xPropagandaMessageObject;
    }
}
