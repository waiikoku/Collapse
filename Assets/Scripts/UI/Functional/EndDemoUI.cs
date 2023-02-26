using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDemoUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup cg;

    public void Open()
    {
        LeanTween.alphaCanvas(cg, 1, 0.3f).setEase(LeanTweenType.easeInCirc);
    }

    public void Close()
    {
        LeanTween.alphaCanvas(cg, 0, 0.15f).setEaseLinear();
        Action OnClose = delegate { cg.gameObject.SetActive(false); };
        LeanTween.delayedCall(1f, OnClose);
    }
}
