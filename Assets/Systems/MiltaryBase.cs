using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiltaryBase : SystemBase
{
    public override SystemValuesBase GetMyValues()
    {
        return MilitaryBaseValuesContainer.GetMilitaryBaseValues();
    }
}
