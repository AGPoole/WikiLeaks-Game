using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePoliticsSettingsButton : MonoBehaviour
{
    int m_iCurrentSetting = 2;

    [SerializeField]
    GameObject m_xSearchBarContainer;
    [SerializeField]
    GameObject m_xCandidatesContainer;
    // Start is called before the first frame update
    void Start()
    {
        UpdateWithSettings();
    }

    public void OnClick()
    {
        m_iCurrentSetting = (m_iCurrentSetting + 1) % 3;
        UpdateWithSettings();
    }

    void UpdateWithSettings()
    {
        switch (m_iCurrentSetting)
        {
            case 0:
            {
                m_xSearchBarContainer.SetActive(false);
                m_xCandidatesContainer.SetActive(false);
                break;
            }
            case 1:
            {
                m_xSearchBarContainer.SetActive(true);
                m_xCandidatesContainer.SetActive(false);
                break;
            }
            case 2:
            {
                m_xSearchBarContainer.SetActive(true);
                m_xCandidatesContainer.SetActive(true);
                break;
            }
            default:
            {
                Debug.LogError("Invalid setting for politics display controller");
                break;
            }
        }
    }
}
