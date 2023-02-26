using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startpoint : Singleton<Startpoint>
{
    [SerializeField] private Transform m_transform;

    protected override void Awake()
    {
        base.Awake();
        m_transform = transform;
    }

    public Transform GetPoint()
    {
        return m_transform;
    }
}
