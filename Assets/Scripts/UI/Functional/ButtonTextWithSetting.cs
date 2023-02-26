using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTextWithSetting : ButtonTextWithFrame
{
    [Header("Check Box")]
    public Toggle checkBox;
    public bool isCheck = false;
    [Header("Select List")]
    public Button leftBtn;
    public Button rightBtn;
    public Button selectionBtn;
    public TMPro.TextMeshProUGUI selectOption;

    public void AssignText(string message)
    {
        selectOption.text = message;
    }
}
