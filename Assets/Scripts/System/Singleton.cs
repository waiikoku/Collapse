using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; }
    protected bool isDuplicate = false;

    protected virtual void Awake()
    {
        if(Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            isDuplicate = true;
            Destroy(this);
            return;
        }
    }
}
