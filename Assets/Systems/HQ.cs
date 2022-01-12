public class HQ : SystemBase
{
    public override SystemValuesBase GetMyValues()
    {
        return HQValuesContainer.GetHQValues();
    }
}
