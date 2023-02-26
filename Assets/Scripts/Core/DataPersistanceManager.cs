using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPersistanceManager : Singleton<DataPersistanceManager>
{
    protected override void Awake()
    {
        base.Awake();
        if (isDuplicate)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

}
