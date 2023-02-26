using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class WaypointUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private GameObject distanceObj;
    public void Activate(bool value)
    {
        distanceObj.SetActive(value);
    }

    public void UpdateDistance(float value)
    {
        distanceText.text = $"{value.ToString("F0")}m";
    }
}
