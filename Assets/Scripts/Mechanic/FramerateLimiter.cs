using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramerateLimiter : MonoBehaviour
{
    public enum Fps
    {
        unlimit = 0,
        movie = 24,
        normal = 30,
        high = 60,
        extreme = 144,
    }
    [SerializeField] private Fps fps;

    private void OnValidate()
    {
        UpdateFrameRate();
    }

    private void Start()
    {
        UpdateFrameRate();
    }

    private void UpdateFrameRate()
    {
        Application.targetFrameRate = (int)fps;
    }
}
