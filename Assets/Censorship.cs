﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Censorship : SystemBase
{
    [SerializeField]
    Government m_xOwner;

    public override void OnNextTurn(int iOwnerLevel)
    {
        base.OnNextTurn(iOwnerLevel);
        if (m_iLevel == 0)
        {
            return;
        }
        var xCensorshipValues = CensorshipValuesContainer.GetCensorshipValues();
        xCensorshipValues.UseNotification();
        // will this work with children?
        var xTechComp = m_xOwner.GetCountry().GetTechCompany();
        if (xTechComp.GetTechCompanyData().GetSize() < m_xOwner.GetGovernmentData().GetSize() * xCensorshipValues.GetRatioRequirement())
        {
            var xPropagandaSystems = xTechComp.GetSystemsOfType(typeof(Propaganda));
            foreach (var xSys in xPropagandaSystems)
            {
                var xPropSys = (Propaganda)xSys;
                xPropSys.ModifyLevel(-1, 30);
            }
        }
    }

    protected override void SetLevel(int iLevel, int iTimer)
    {
        base.SetLevel(iLevel, iTimer);
        if (m_iLevel == 2)
        {
            m_xOwner.DisableElections();
        }
        else
        {
            m_xOwner.EnableElections();
        }
    }

    protected override SystemValuesBase GetMyValues()
    {
        return CensorshipValuesContainer.GetCensorshipValues();
    }
}
