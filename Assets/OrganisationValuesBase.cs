using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class OrganisationValuesBase
{
    public abstract float GetCostsAtLevel(int iLevel);

    public abstract float GetLevelUpRequirementAtLevel(int iLevel);
}
