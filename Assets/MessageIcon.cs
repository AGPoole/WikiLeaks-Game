using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MessageIcon : MonoBehaviour
{
    protected Transform m_xTarget;
    [SerializeField]
    float m_fSpeed;

    public void SetTarget(Transform xTransform)
    {
        Debug.LogFormat("{0} {1}", xTransform.gameObject.name, (xTransform.position - transform.position).magnitude);
        m_xTarget = xTransform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + Time.deltaTime * m_fSpeed * (m_xTarget.transform.position - transform.position).normalized;
        if((m_xTarget.transform.position - transform.position).magnitude < 0.1f)
        {
            Destroy(gameObject);
            OnTargetReached();
        }
    }

    protected abstract void OnTargetReached();
}