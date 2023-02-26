using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forcefield : MonoBehaviour
{
    [SerializeField] private ControlPoint cpForcefield;
    [SerializeField] private Renderer rend;
    [SerializeField] private Material mat;
    [SerializeField] private string colorKey = "_Color";
    [SerializeField] private Color nonCapCol;
    private HSV hsv_NCC;
    [SerializeField] private Color CapturingCol;
    private HSV hsv_CTC;
    private Color lerpColor;
    private float currentHue;
    private float currentSaturate;
    private float currentValue;
    private void Start()
    {
        mat = rend.material;
        mat.SetColor(colorKey, nonCapCol);
        cpForcefield.OnProgress += UpdateProgress;
        hsv_NCC = ConvertHSV(nonCapCol);
        hsv_CTC = ConvertHSV(CapturingCol);
    }

    private void UpdateProgress(float value)
    {
        currentHue = Mathf.Lerp(hsv_NCC.h, hsv_CTC.h, value);
        currentSaturate = Mathf.Lerp(hsv_NCC.s, hsv_CTC.s, value);
        currentValue = Mathf.Lerp(hsv_NCC.v, hsv_CTC.v, value);
        lerpColor = Color.HSVToRGB(currentHue, currentSaturate, currentValue);
        mat.SetColor(colorKey, lerpColor);
    }

    private HSV ConvertHSV(Color color)
    {
        HSV hsv = new HSV();
        Color.RGBToHSV(color, out hsv.h, out hsv.s, out hsv.v);
        return hsv;
    }

    private Color ConvertRGB(HSV hsv)
    {
        return Color.HSVToRGB(hsv.h, hsv.s, hsv.v);
    }

    [System.Serializable]
    public struct HSV
    {
        public float h;
        public float s;
        public float v;
    }
}
