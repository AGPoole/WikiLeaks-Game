using System.Collections;
using System.Collections.Generic;
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
        m_xImage.sprite = Manager.GetManager().GetSpriteAtLevel(iLevel);
        Color c = m_xImage.color;
        c.a = iLevel == 0 ? 0.4f : 1f;
        m_xImage.color = c;
    }
}
