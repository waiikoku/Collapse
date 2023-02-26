using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MissionDetailUI : MonoBehaviour
{
    private MissionData mission;
    [SerializeField] private bool useAnimate = false;
    [SerializeField] private Animator anim;
    [SerializeField] private AnimationClip clip_in;
    [SerializeField] private AnimationClip clip_out;
    [SerializeField] private TextMeshProUGUI msName;
    [SerializeField] private TextMeshProUGUI msDetail;
    [SerializeField] private TextMeshProUGUI msObjectives;
    [SerializeField] private Image msPreview;
    [SerializeField] private Button msAccept;
    [SerializeField] private Button msDecline;
    [SerializeField] private CanvasGroup cg;

    private void OnEnable()
    {
        if(useAnimate)
        {
            msAccept.interactable = false;
            msDecline.interactable = false;
            LeanTween.alphaCanvas(cg, 0, 0f);
            LeanTween.alphaCanvas(cg, 1, 0.4f).setEase(LeanTweenType.linear);
            StartCoroutine(DelayExecute(0.4f, delegate { msAccept.interactable = true; msDecline.interactable = true; }));
        }
        else
        {
            msAccept.interactable = true;
            msDecline.interactable = true;
        }
        /*
        if (useAnimate == false)
        {
            anim.Play("Idle");
            msAccept.interactable = true;
            msDecline.interactable = true;
        }
        else
        {
            msAccept.interactable = false;
            msDecline.interactable = false;
            anim.SetTrigger("In");
    
        }
        */
    }

    public void Select(MissionData data)
    {
        mission = data;
        msName.text = mission.missionName;
        msDetail.text = mission.missionDescription;
        //msObjectives.text = mission.missionObjectives;
        msObjectives.text = "";
        for (int i = 0; i < mission.missionObjectives.Length; i++)
        {
            msObjectives.text += mission.missionObjectives[i] + System.Environment.NewLine;
        }
        msPreview.sprite = mission.missionPreview;

        msAccept.onClick.AddListener(delegate { MissionManager.Instance.LoadMission(mission.missionName); });
        msDecline.onClick.AddListener(delegate { Deactivate(); });
    }

    public void ActivateMissionDetail(bool value)
    {
        gameObject.SetActive(value);
    }
    private Coroutine disableCoroutine;
    private void Deactivate()
    {
        msAccept.interactable = false;
        msDecline.interactable = false;
        if (useAnimate)
        {
            LeanTween.alphaCanvas(cg, 0, 0.4f).setEase(LeanTweenType.linear);
            if (disableCoroutine != null)
            {
                StopCoroutine(disableCoroutine);
            }
            disableCoroutine = StartCoroutine(DelayExecute(0.4f, delegate { ActivateMissionDetail(false); }));
            /*
            if(disableCoroutine != null)
            {
                StopCoroutine(disableCoroutine);
            }
            float duration = clip_out.length;
            anim.SetTrigger("Out");
            print("Disable After : " + duration);
            disableCoroutine = StartCoroutine(DelayExecute(duration, delegate { ActivateMissionDetail(false); }));
            */
        }
        else
        {
            ActivateMissionDetail(false);
        }
    }

    private IEnumerator DelayExecute(float duration,Action callback)
    {
        yield return new WaitForSeconds(duration);
        callback?.Invoke();
        disableCoroutine = null;
        yield break;
    }

}
