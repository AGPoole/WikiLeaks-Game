using UnityEngine;

public class TerrorismMission : MissionBase
{
    [SerializeField]
    int m_iMoneyForSuccess = 100;

    public override bool IsUnlocked()
    {
        var xSystems = SystemBase.GetAllSystems().FindAll((SystemBase xSys) => { return xSys.IsHacked() && xSys.GetType() == typeof(MiltaryBase); });
        return xSystems.Count == 0;
    }

    protected override MissionState CalculateNextState()
    {
        var xSystems = SystemBase.GetAllSystems().FindAll((SystemBase xSys) => { return xSys.IsHacked() && xSys.GetType() == typeof(MiltaryBase); });
        if (xSystems.Count != 0)
        {
            return MissionState.SUCCEEDED;
        }
        else
        {
            return MissionState.ACTIVE;
        }
    }

    protected override void OnFailure()
    {
        Debug.LogError("Failure");
    }

    protected override void OnSuccess()
    {
        Debug.LogError("Success");
        Manager.GetManager().ChangeMoney(m_iMoneyForSuccess);
        // TODO: make this effect the world
    }
}
