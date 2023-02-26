using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonTextWithSlider : ButtonTextWithFrame
{
    public Slider slider;
    public Color slideColor = Color.white;
    public Image slideImg;
    public TMPro.TextMeshProUGUI progressText;
    public TMPro.TextMeshProUGUI progressHLText;
    public TMPro.TextMeshProUGUI labelText;
    public TMPro.TextMeshProUGUI highlightlabelText;
    public RectTransform highLightLabel;

    private void Awake()
    {
        slider.onValueChanged.AddListener(delegate { UpdateProgress(slider.value); TrackPosition(slider.value); });
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveAllListeners();
    }

    public void DirectUpdate(float value)
    {
        slider.value = value;
    }

    private void TrackPosition(float value)
    {
        float size = highLightLabel.sizeDelta.x;
        float halfSize = size / 2f;
        Vector2 pos = new Vector2((size * (1 - value)) + (halfSize * (1 - value)),0);
        highLightLabel.anchoredPosition = pos;
    }

    private void UpdateProgress(float progress)
    {
        int proInt = (int)(progress * 100f);
        string format = string.Format("{0}%", proInt);
        progressText.text = format;
        progressHLText.text = format; 
    }

    protected override void Normal()
    {
        Color tempCol = frameColor;
        tempCol.a = 0f;
        image.color = tempCol;
        progressText.color = normalColor;
        progressHLText.color = highlightColor;
        labelText.color = normalColor;
        highlightlabelText.color = highlightColor;
        float[] hsv = new float[3];
        Color.RGBToHSV(frameColor, out hsv[0], out hsv[1], out hsv[2]);
        Color convert = Color.HSVToRGB(hsv[0], hsv[1], hsv[2] + 0.3f);
        slideColor = convert;
        slideImg.color = slideColor;
    }

    protected override void Highlight()
    {
        Color tempCol = frameColor;
        tempCol.a = 0.5f;
        image.color = tempCol;
    }



    public override void OnPointerDown(PointerEventData eventData)
    {
        if (hasExit) return;
    }
}
