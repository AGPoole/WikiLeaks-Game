using UnityEngine;

public class CyberSecurityDisableButton : MonoBehaviour
{
    [SerializeField]
    CyberSecurity m_xCyberSec;
    [SerializeField]
    UnityEngine.UI.Text m_xText;
    bool m_bDisabled = false;

    public void OnClick()
    {
        m_bDisabled = !m_bDisabled;
        m_xCyberSec.SetDisabledByPlayer(m_bDisabled);
        m_xText.text = m_bDisabled ? "Enable" : "Disable";
    }
}
