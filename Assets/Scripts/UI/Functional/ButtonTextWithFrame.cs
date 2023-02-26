using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(Image))]
public class ButtonTextWithFrame : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] protected Image image;
    [SerializeField] protected TMPro.TextMeshProUGUI text;
    public Color normalColor = Color.white;
    public Color highlightColor = Color.white;
    public Color frameColor = Color.white;
    protected bool hasExit = false;

#if UNITY_EDITOR
    public void Setup()
    {
        image = gameObject.GetComponent<Image>();
        text = gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }
#endif
    public void UpdateColor()
    {
        Normal();
    }

    protected virtual void Normal()
    {
        Color tempCol = frameColor;
        tempCol.a = 0f;
        image.color = tempCol;
        text.color = normalColor;
    }

    protected virtual void Highlight()
    {
        Color tempCol = frameColor;
        tempCol.a = 0.5f;
        image.color = tempCol;
        text.color = highlightColor;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        hasExit = false;
        Highlight();
        SoundManager.Instance.PlaySfx(0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hasExit = true;
        Normal();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (hasExit) return;
        Normal();
    }
}
