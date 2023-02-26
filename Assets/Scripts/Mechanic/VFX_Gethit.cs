using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_Gethit : MonoBehaviour
{
    [SerializeField] private bool multipleRenderer = false;
    [SerializeField] private Renderer[] rends;
    private List<Material> mats;
    [SerializeField] private string Keyword_Color = "_Color";

    public enum Type
    {
        Player,
        Enemy
    }
    [SerializeField] private Type type;
    [SerializeField] private AgentMovement am;
    [SerializeField] private CharacterCombat cc;

    public enum VFX
    {
        Glow,
        Blink,
        Blood
    }
    [SerializeField] private VFX vfxType;

    [Header("Glow Properties")]
    [SerializeField] private float glowTime;

    [Header("Blink Properties")]
    [SerializeField] private float blinkInDuration;
    [SerializeField] private float blinkOutDuration;
    private float blinkTimer;
    private float blinkPercentage;
    private Color baseColor;
    [SerializeField] private Color blinkColor = Color.white;
    private enum BlinkType
    {
        BlinkIn,
        BlinkOut
    }
    [SerializeField] private BlinkType bt;
    private void Start()
    {
        if (rends[0] == null)
        {
            print("No Renderer!");
            return;
        }
        mats = new List<Material>(rends.Length);
        mats.Add(rends[0].material);
        if (multipleRenderer)
        {
            for (int i = 1; i < rends.Length; i++)
            {
                mats.Add(rends[i].material);
            }
        }
        switch (type)
        {
            case Type.Player:
                if (cc == null) return;
                cc.AddDamagedListenr(AddEvent());
                break;
            case Type.Enemy:
                if (am == null) return;
                am.AddDamagedListenr(AddEvent());
                break;
            default:
                break;
        }
        baseColor = mats[0].GetColor(Keyword_Color);
    }

    private void Glow()
    {

    }

    private void Blink()
    {
        print("Blink!");
        if(blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        blinkCoroutine = StartCoroutine(ThreadBlink());
    }
    private Coroutine blinkCoroutine;
    private IEnumerator ThreadBlink()
    {
        blinkTimer = 0f;
        Color transitionColor = Color.white;
        while (true)
        {
            blinkTimer += Time.deltaTime;
            switch (bt)
            {
                case BlinkType.BlinkIn:
                    transitionColor = Color.Lerp(baseColor, blinkColor, blinkPercentage);
                    blinkPercentage = blinkTimer / blinkInDuration;
                    break;
                case BlinkType.BlinkOut:
                    transitionColor = Color.Lerp(blinkColor,baseColor, blinkPercentage);
                    blinkPercentage = blinkTimer / blinkOutDuration;
                    break;
                default:
                    break;
            }
            foreach (var mat in mats)
            {
                mat.SetColor(Keyword_Color, transitionColor);
            }
            if(blinkPercentage >= 1f)
            {
                blinkCoroutine = null;
                yield break;
            }
            yield return null;
        }
    }

    private void Bloodsplash()
    {

    }

    private Action AddEvent()
    {
        switch (vfxType)
        {
            case VFX.Glow:
                return Glow;
            case VFX.Blink:
                return Blink;
            case VFX.Blood:
                return Bloodsplash;
            default:
                return null;
        }
    }
}
