using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageIcon : MonoBehaviour
{
    int m_iCreationTurn;
    const int iMAX_TURNS_LIFETIME = 200;
    protected Transform m_xTarget;
    protected Transform m_xSource; 
    [SerializeField]
    float m_fSpeed;

    Action m_xCallBack;
    protected void OnTargetReached()
    {
        m_xCallBack();
    }

    public void SetCallBack(Action xCallBack)
    {
        m_xCallBack = xCallBack;
    }

    public void SetTarget(Transform xTransform)
    {
        Debug.LogFormat("{0} {1}", xTransform.gameObject.name, (xTransform.position - transform.position).magnitude);
        m_xTarget = xTransform;
    }

    public Transform GetTarget()
    {
        return m_xTarget;
    }

    public void SetSource(Transform xTransform)
    {
        m_xSource = xTransform;
    }

    public Transform GetSource()
    {
        return m_xSource;
    }

    void Start()
    {
        m_iCreationTurn = Manager.GetTurnNumber();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Manager.GetIsPaused())
        {
            transform.position = transform.position + Time.deltaTime * m_fSpeed * (m_xTarget.transform.position - transform.position).normalized;
            if ((m_xTarget.transform.position - transform.position).magnitude < 0.1f)
            {
                Destroy(gameObject);
                OnTargetReached();
            }
            if (Manager.GetTurnNumber() > m_iCreationTurn + iMAX_TURNS_LIFETIME)
            {
                Destroy(this);
                Debug.LogError("Did not reach target"); 
            }
        }
    }
}