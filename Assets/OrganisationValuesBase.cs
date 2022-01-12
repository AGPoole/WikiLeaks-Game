[System.Serializable]
public abstract class OrganisationValuesBase
{
    public abstract float GetCostsAtLevel(int iLevel);

    public abstract float GetLevelUpRequirementAtLevel(int iLevel);
}
