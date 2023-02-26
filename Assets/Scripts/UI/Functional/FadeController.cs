using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FadeController : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    private Coroutine fadeCoroutine;
    private float currentProgress;
    public void Activate(bool value,float duration = 1)
    {
        if(fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeImage.enabled = true;
        fadeCoroutine = StartCoroutine(Fading(value,duration));
    }

    private IEnumerator Fading(bool value, float duration)
    {
        Color lastColor = fadeImage.color;
        while (true)
        {
            if(value)
            {
                currentProgress += (1 / duration) * Time.deltaTime;
                if(currentProgress >= 1f)
                {
                    currentProgress = 1f;
                    lastColor.a = currentProgress;
                    fadeImage.color = lastColor;
                    fadeImage.enabled = false;
                    break;
                }
            }
            else
            {
                currentProgress -= (1 / duration) * Time.deltaTime;
                if (currentProgress <= 0f)
                {
                    currentProgress = 0f;
                    lastColor.a = currentProgress;
                    fadeImage.color = lastColor;
                    fadeImage.enabled = false;
                    break;
                }
            }
            yield return null;
            lastColor.a = currentProgress;
            fadeImage.color = lastColor;
        }
    }
}
