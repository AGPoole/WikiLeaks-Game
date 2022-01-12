public class DataCollection : SystemBase
{
    public override SystemValuesBase GetMyValues()
    {
        return DataCollectionValuesContainer.GetDataCollectionValues();
    }
}
