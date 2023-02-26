using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicManager : MonoBehaviour
{
    [Header("Main")]
    public Vector2 Resolution = new Vector2(1920, 1080);
    public bool Fullscreen = true;

    public enum GraphicQuality
    {
        VeryLow,
        Low,
        Medium,
        High,
        VeryHigh
    }
    public enum AnisotropicMultipier
    {
        Off,
        X2,
        X4,
        X8,
        X16
    }
    public enum AntiAliasing
    {
        Off,
        FXAA,
        SMAA
    }
    [Header("Advanced")]
    public bool Vsync = false;
    public bool DepthOfField = true;
    public GraphicQuality TextureQuality = GraphicQuality.High;
    public GraphicQuality ShadowQuality = GraphicQuality.High;
    public AnisotropicMultipier AnisotropicFiltering = AnisotropicMultipier.X8;
    public GraphicQuality AnimationQuality = GraphicQuality.High;
    public AntiAliasing m_antiAliasing = AntiAliasing.FXAA;
    [Range(0.0f, 1.0f)]
    public float FieldOfView = 0f;
    private readonly int[] preloadBytes = {32, 64, 128, 256, 512, 1024, 2048, 4096};
    public int ByteIndex = 0;


}
