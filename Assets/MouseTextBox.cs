using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTextBox : MonoBehaviour
{
    static MouseTextBox s_xMouseTextInstance;
    [SerializeField]
    Vector3 m_xOffset;
    [SerializeField]
    UnityEngine.UI.Text m_xText;
    [SerializeField]
    List<string> m_xStrings;

    void Start()
    {
        if (s_xMouseTextInstance!=null)
        {
            Debug.LogError("2 mouse text boxes-this should not happen");
        }
        s_xMouseTextInstance = this;
        m_xStrings = new List<string>();
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition + m_xOffset;
        m_xText.enabled = m_xStrings.Count > 0;
        if (m_xStrings.Count > 0)
        {
            m_xText.text = m_xStrings[0];
        }
        // Re-add every frame to prevent glitch where it get stuck on screen
        m_xStrings.Clear();
    }

    public static void AddText(string xText)
    {
        s_xMouseTextInstance.m_xStrings.Add(xText);
    }
}
