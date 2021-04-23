using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class ActionBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected SystemBase m_xOwner;

    [SerializeField]
    GameObject m_xDescription;

    protected virtual void Start()
    {
        GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(delegate { OnClick(); });
    }
    public abstract void OnClick();

    public void SetOwner(SystemBase xOwner)
    {
        m_xOwner = xOwner;
    }

    public virtual void Update()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_xDescription.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_xDescription.SetActive(false);
    }
}
