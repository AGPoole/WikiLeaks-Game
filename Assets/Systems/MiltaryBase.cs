public class MiltaryBase : SystemBase
{
    public override SystemValuesBase GetMyValues()
    {
        return MilitaryBaseValuesContainer.GetMilitaryBaseValues();
    }
}
