using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkManager : MonoBehaviour
{
    static PerkManager s_xInstance;

    [SerializeField]
    List<PerkBase> m_xPerks;

    void Start()
    {
        if (s_xInstance == null)
        {
            s_xInstance = this;
        }
    }

    public static PerkManager GetPerkManager()
    {
        return s_xInstance;
    }

    public void OnNextTurn()
    {
        foreach(PerkBase xPerk in m_xPerks)
        {
            xPerk.OnNextTurn();
        }
    }

    public void AddPerk(PerkBase xPerk)
    {
        m_xPerks.Add(xPerk);
    }

    public void RemovePerk(PerkBase xPerk)
    {
        if (!m_xPerks.Contains(xPerk))
        {
            Debug.LogError("Attempting to remove a perk that is not activated");
        }
        m_xPerks.Remove(xPerk);
    }
}
