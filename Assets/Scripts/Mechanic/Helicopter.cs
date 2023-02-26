using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour
{
    [SerializeField] private Animator m_anim;
    private void Start()
    {
        m_anim.SetBool("Fly", true);
    }
}
