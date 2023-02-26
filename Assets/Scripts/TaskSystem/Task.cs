using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Task : MonoBehaviour
{
    public int TaskID = -1;
    public bool IsCompleted;
    public event Action OnComplete;
}
