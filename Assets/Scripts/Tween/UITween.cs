using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UITween : MonoBehaviour
{
    [SerializeField] private GameObject test;
    [SerializeField] private GameObject test2;
    [SerializeField] private Vector2 uiPos;
    [SerializeField] private float duration = 1f;
    private void Start()
    {
        LeanTween.moveLocalX(test, uiPos.x, duration);
        LeanTween.scale(test2, Vector3.one * 1.3f, 1f).setDelay(0.5f).setEaseOutElastic();
        LeanTween.scale(test2, Vector3.one, 1f).setDelay(1f + 0.5f).setEase(LeanTweenType.easeInElastic);
        //LeanTween.moveLocalX(test2, uiPos.x, 0.7f).setDelay(2f).setEaseInOutCirc();
    }
}
