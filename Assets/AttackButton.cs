using UnityEngine;

public class AttackButton : MonoBehaviour
{
    [SerializeField]
    GameObject m_xParent;

    public void OnClick()
    {
        m_xParent.GetComponent<SystemBase>().Attack(true);
    }
}
