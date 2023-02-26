using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI timerText;
    [SerializeField] private bool isCountdown = false;
    private float timer;
    private int minute;
    private int second;
    private void LateUpdate()
    {
        timer += Time.deltaTime;

        if (isCountdown)
        {

        }
        else
        {
            second = (int)(timer % 60);
            minute = (int)(timer / 60);
            string minuteText = minute < 10 ? $"0{minute}" : minute.ToString();
            string secondText = second < 10 ? $"0{second}" : second.ToString();
            timerText.text = $"{minuteText}:{secondText}";
        }
    }
}
