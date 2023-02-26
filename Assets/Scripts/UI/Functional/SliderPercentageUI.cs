using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SliderPercentageUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        slider.onValueChanged.AddListener(delegate { text.text = (slider.value * 100f).ToString("F0"); });
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveAllListeners();
    }
}
