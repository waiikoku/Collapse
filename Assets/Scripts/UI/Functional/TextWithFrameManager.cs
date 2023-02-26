using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextWithFrameManager : MonoBehaviour
{
    [SerializeField] private ButtonTextWithFrame[] btwfs;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.white;
    [SerializeField] private Color frameColor = Color.white;

    private void Start()
    {
        UpdateColors();
    }

    private void UpdateColors()
    {
        for (int i = 0; i < btwfs.Length; i++)
        {
            if(btwfs[i] == null)
            {
                print($"Button at {i} is Null!");
                continue;
            }
            btwfs[i].normalColor = normalColor;
            btwfs[i].highlightColor = highlightColor;
            btwfs[i].frameColor = frameColor;
            btwfs[i].UpdateColor();
        }
    }
}
