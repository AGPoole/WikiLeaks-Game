using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseMarketShareAction : ActionBase
{
    public override void OnClick()
    {
        ((TechCompanyData)((TechCompany)m_xSystemOwner.GetOwner()).GetData()).ChangeMarketShare(10f);
    }
}
