using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtEffect : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image overlay;
    [SerializeField] private float fadeInDuration = 0.1f;
    [SerializeField] private float fadeOutDuration = 0.2f;

    private void Start()
    {
        GameManager.Instance.OnHurt += Hurt;
    }

    private void Hurt()
    {
        print("Hurt!");
        if(current != null)
        {
            StopCoroutine(current);
        }
        overlay.gameObject.SetActive(true);
        current = StartCoroutine(Thread());
    }
    private Coroutine current;
    private IEnumerator Thread()
    {
        float fit = 0f;
        float fot = 0f;
        float fip;
        float fop;
        Color fullAlpha = Color.white;
        fullAlpha.a = 1f;
        Color noAlpha = Color.white;
        noAlpha.a = 0f;
        overlay.color = noAlpha;
        while (true)
        {
            fit += Time.deltaTime;
            fip = fit / fadeInDuration;
            overlay.color = Color.Lerp(noAlpha, fullAlpha, fip);
            if(fit > fadeInDuration)
            {
                overlay.color = fullAlpha;
                break;
            }
            yield return null;
        }

        while (true)
        {
            fot += Time.deltaTime;
            fop = fot / fadeOutDuration;
            overlay.color = Color.Lerp(fullAlpha, noAlpha, fop);
            if (fot > fadeInDuration)
            {
                overlay.color = noAlpha;
                break;
            }
            yield return null;
        }
        overlay.gameObject.SetActive(false);
        current = null;
        yield break;
    }
}
