using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public abstract class ActionBase : MonoBehaviour
{
    protected SystemBase m_xOwner;

    protected virtual void Start()
    {
        GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { OnClick(); });
    }
    public abstract void OnClick();

    public void SetOwner(SystemBase xOwner)
    {
        m_xOwner = xOwner;
    }
}
