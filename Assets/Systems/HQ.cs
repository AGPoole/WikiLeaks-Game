using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HQ : SystemBase
{
    public override SystemValuesBase GetMyValues()
    {
        return HQValuesContainer.GetHQValues();
    }
}
