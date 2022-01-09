using UnityEngine;

public class SystemImageContainer : MonoBehaviour
{
    SystemBase m_xSystem;

    [SerializeField]
    UnityEngine.UI.Image m_xImage;

    public void SetSystem(SystemBase xSys)
    {
        m_xSystem = xSys;
        m_xImage.gameObject.name = xSys.name + " image";
    }
    // Update is called once per frame
    public void OnClick()
    {
        m_xSystem.GetUI().Select();
    }

    public void UpdateSpriteAndColour(int iLevel)
    {
        bool bIsGovernment = m_xSystem.GetOwner().GetType() == typeof(Government);
        m_xImage.sprite = Manager.GetManager().GetSpriteAtLevel(iLevel, bIsGovernment);
        Color c = m_xImage.color;
        c.a = iLevel == 0 ? 0.4f : 1f;
        m_xImage.color = c;
    }
}
