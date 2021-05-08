using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCollection : SystemBase
{
    public override SystemValuesBase GetMyValues()
    {
        return DataCollectionValuesContainer.GetDataCollectionValues();
    }
}
