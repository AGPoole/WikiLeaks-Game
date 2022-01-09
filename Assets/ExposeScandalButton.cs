using UnityEngine;

public class ExposeScandalButton : MonoBehaviour
{
    [SerializeField]
    Candidate m_xCandidate;

    public void OnClick()
    {
        m_xCandidate.ExposeScandal();
    }
}
