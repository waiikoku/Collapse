using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI status;

    public void AssignInfo(Sprite sprite,string message)
    {
        icon.sprite = sprite;
        status.text = message;
    }
}
