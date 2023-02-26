using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CaptureStatus : MonoBehaviour
{
    [SerializeField] private ControlPoint control;


    [SerializeField] private GameObject captureWindow; //Percentage (Progress)
    [SerializeField] private GameObject defenseWindow; //Timer (Countdown)

    [SerializeField] private Image captureSlide;
    //[SerializeField] private Slider captureProgress;
    [SerializeField] private TextMeshProUGUI defenseTimer;

    private bool isCapture = false;
    private bool isDefense = false;

    private bool deactivated = false;

    private float displayDecayDuration = 1f;
    private float timestamp;
    private void Start()
    {
        float rand = Random.Range(15.0f, 120.0f);
        print(rand + " | " + ConvertTime(rand));
        if (control == null)
        {
            this.enabled = false;
            return;
        }
        control.OnProgress += UpdateProgress;
        control.OnDefense += OnDefense;
        ActivateCapture(false);
        ActivateDefense(false);
    }

    private void LateUpdate()
    {
        if (isCapture == false) return;
        if (deactivated) return;
        if (Time.time > timestamp)
        {
            deactivated = true;
            if (isDefense)
            {
                ActivateDefense(false);
                return;
            }
            if (isCapture)
            {
                ActivateCapture(false);
            }
        }
    }

    private void UpdateProgress(float progress)
    {
        if (isDefense) return;
        captureSlide.fillAmount = progress;
        if(isCapture == false)
        {
            isCapture = true;
            ActivateCapture(true);
        }
        timestamp = Time.time + displayDecayDuration;
        if (deactivated)
        {
            deactivated = false;
        }
    }

    private void UpdateCountdown(float timer)
    {
        if (isDefense == false) return;
        defenseTimer.text = ConvertTime(timer);

        timestamp = Time.time + displayDecayDuration;
        if (deactivated)
        {
            deactivated = false;
        }
    }

    private void OnDefense()
    {
        ActivateCapture(false);
        ActivateDefense(true);
        UpdateProgress(1f);
        isDefense = true;
        control.OnProgress -= UpdateProgress;
        control.OnProgress += UpdateCountdown;
    }

    private string ConvertTime(float sec)
    {
        int minute = (int)(sec / 60f);
        int second = (int)(sec - (minute * 60f));
        return string.Format("{0,0:00}:{1,0:00}",minute,second);
    }


    private void ActivateCapture(bool value)
    {
        captureWindow.SetActive(value);
    }

    private void ActivateDefense(bool value)
    {
        defenseWindow.SetActive(value);
    }
}
