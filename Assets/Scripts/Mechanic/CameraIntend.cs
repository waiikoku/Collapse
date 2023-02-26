using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraIntend : Singleton<CameraIntend>
{
    private Transform m_transform;
    private Vector3 camF;
    private Vector3 camR;

    protected override void Awake()
    {
        base.Awake();
        m_transform = transform;
    }

    private void LateUpdate()
    {
        camF = m_transform.forward;
        camF.y = 0;
        camR = m_transform.right;
        camR.y = 0;
        camF.Normalize();
        camR.Normalize();
    }

    public Vector3 GetIntend(Vector2 dir)
    {
        return (camF * dir.y) + (camR * dir.x);
    }
}
