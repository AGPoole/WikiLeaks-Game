using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseMarketShareAction : ActionBase
{
    public override void OnClick()
    {
        ((TechCompanyData)((TechCompany)m_xOwner.GetOwner()).GetData()).ChangeMarketShare(10f);
    }

    public override void SetHacked(bool bHacked)
    {
        base.SetHacked(bHacked);
        gameObject.SetActive(bHacked);
    }
}
