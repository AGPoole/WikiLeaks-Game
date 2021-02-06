using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TogglePauseButton : MonoBehaviour
{
    [SerializeField]
    Sprite m_xPauseSprite;
    [SerializeField]
    Sprite m_xPlaySprite;
    static TogglePauseButton s_xPauseButton;

    public static TogglePauseButton GetPauseButton()
    {
        if (s_xPauseButton == null)
        {
            s_xPauseButton = FindObjectOfType<TogglePauseButton>();
        }
        return s_xPauseButton;
    }
    public void TogglePause()
    {
        Manager.TogglePaused();
        GetComponent<UnityEngine.UI.Button>().image.sprite=Manager.GetIsPaused() ? m_xPlaySprite : m_xPauseSprite;
    }
}
