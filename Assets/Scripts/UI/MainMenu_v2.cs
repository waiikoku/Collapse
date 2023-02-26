using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu_v2 : MonoBehaviour
{
    public bool uiAnimation = false;

    [Header("Main")]
    [SerializeField] private Button btnPlay;
    [SerializeField] private Button btnLoadout;
    [SerializeField] private Button btnOption;
    [SerializeField] private Button btnExit;
    [SerializeField] private Button btnBack;

    [SerializeField] private GameObject mainPanel;
    [SerializeField] private Animator mainAnim;
    [SerializeField] private GameObject localMainLabel;
    private bool isMain = true;

    [Header("Loadout")]
    [SerializeField] private CanvasGroup cg_loadoutPanel;
    [SerializeField] private GameObject loadoutPanel;
    [SerializeField] private LoadoutUI[] loadouts;
    [SerializeField] private TextMeshProUGUI statusText;
    private bool isLoadout = false;
    [Header("Options")]
    [SerializeField] private CanvasGroup cg_optionPanel;
    [SerializeField] private GameObject optionPanel;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider effectSlider;
    private bool isOptions = false;
    [Header("Mission")]
    [SerializeField] private CanvasGroup cg_missionPanel;
    [SerializeField] private GameObject missionPanel;
    [SerializeField] private GameObject missionDetailPanel;
    [SerializeField] private MissionUI[] missions;
    [SerializeField] private MissionDetailUI missionDetail;
    [SerializeField] private MissionData selectMission;
    [SerializeField] private RectTransform mapContainer;
    private bool isMisison = false;

    private void Start()
    {
        SoundManager.Instance.ChangeMusic(0);
        StartCoroutine(StartDelay());
        btnLoadout.onClick.AddListener(delegate { ActivateMainPanel(false); ActivateLoadoutPanel(true); ActivateBackButton(true); });
        btnOption.onClick.AddListener(delegate { ActivateMainPanel(false); ActivateOptionPanel(true); ActivateBackButton(true); });

        btnBack.onClick.AddListener(delegate { DoBack(); });

        btnPlay.onClick.AddListener(delegate { ActivateMainPanel(false); ActivateMissionPanel(true); ActivateBackButton(true); });
        //Direct Start
        //btnPlay.onClick.AddListener(delegate { MissionManager.Instance.LoadMission(selectMission.missionName); });
        btnExit.onClick.AddListener(delegate { Application.Quit(); });
        StartCoroutine(TryLoad_Missions());
        musicSlider.onValueChanged.AddListener(SoundManager.Instance.AdjustMusicVolume);
        effectSlider.onValueChanged.AddListener(SoundManager.Instance.AdjustSfxVolume);
        musicSlider.value = SoundManager.Instance.MusicVolume;
        effectSlider.value = SoundManager.Instance.SfxVolume;
        DisplayLoadout();
    }
    
    private IEnumerator TryLoad_Missions()
    {
        MissionData[] mda;
        while (true)
        {
            mda = MissionManager.Instance.GetAllMission();
            if(mda != null)
            {
                break;
            }
            yield return null;
        }
        print("Menu Got Missions!");
        for (int i = 0; i < missions.Length; i++)
        {
            if(mda.Length <= i)
            {
                missions[i].LoadData(null);
                continue;
            }
            int index = i;
            missions[i].LoadData(mda[i]);
            missions[i].AddListener(delegate { selectMission = mda[index]; missionDetail.Select(selectMission); missionDetailPanel.SetActive(true); });
        }
        print("Missions has Setup!");
    }

    private IEnumerator StartDelay()
    {
        yield return new WaitForEndOfFrame();
        mainAnim.SetBool("SlideIn", true);
    }

    private void ActivateMainPanel(bool value)
    {
        mainPanel.SetActive(value);
        localMainLabel.SetActive(value);
        if (uiAnimation)
        {
            mainAnim.Play("Idle");
            mainAnim.SetBool("SlideIn", value);
        }
        isMain = value;
    }

    private void ActivateOptionPanel(bool value)
    {
        if (uiAnimation)
        {
            if (value == true)
            {
                optionPanel.SetActive(true);
            }
            LeanTween.alphaCanvas(cg_optionPanel, value ? 0f : 1f, 0f);
            LeanTween.alphaCanvas(cg_optionPanel, value ? 1f : 0f, 0.4f).setEase(LeanTweenType.linear);
            if (value == false)
            {
                Action OnActivate = delegate { optionPanel.SetActive(false); };
                LeanTween.delayedCall(0.4f, OnActivate);
            }
        }
        else
        {
            optionPanel.SetActive(value);
        }
        isOptions = value;
    }

    private void ActivateLoadoutPanel(bool value)
    {
        if (uiAnimation)
        {
            if (value == true)
            {
                loadoutPanel.SetActive(true);
            }
            LeanTween.alphaCanvas(cg_loadoutPanel, value ? 0f : 1f, 0f);
            LeanTween.alphaCanvas(cg_loadoutPanel, value ? 1f : 0f, 0.4f).setEase(LeanTweenType.linear);
            if (value == false)
            {
                Action OnActivate = delegate { loadoutPanel.SetActive(false); };
                LeanTween.delayedCall(0.4f, OnActivate);
            }
        }
        else
        {
            loadoutPanel.SetActive(value);
        }
        isLoadout = value;
    }

    private void ActivateMissionPanel(bool value)
    {
        if (uiAnimation)
        {
            if (value == true)
            {
                missionPanel.SetActive(true);
            }
            LeanTween.alphaCanvas(cg_missionPanel, value ? 0f : 1f, 0f);
            LeanTween.alphaCanvas(cg_missionPanel, value ? 1f : 0f, 0.4f).setEase(LeanTweenType.linear);
            if (value == false)
            {
                Action OnActivate = delegate { missionPanel.SetActive(false); };
                LeanTween.delayedCall(0.4f, OnActivate);
            }
        }
        else
        {
            missionPanel.SetActive(value);
        }
        isMisison = value;
    }

    private void ActivateBackButton(bool value)
    {
        btnBack.gameObject.SetActive(value);
    }

    private void DisplayLoadout()
    {
        CharacterData mainPlayer = Resources.Load<CharacterData>("Characters/RobeMan");
        List<GunData> wd = new List<GunData>();
        List<ArmorData> ad = new List<ArmorData>();
        List<GadgetData> gd = new List<GadgetData>();
        for (int i = 0; i < loadouts.Length; i++)
        {
            switch (loadouts[i].Type)
            {
                case LoadoutUI.LoadoutType.Weapon:
                    if (loadouts[i].Item is GunData)
                    {
                        wd.Add(loadouts[i].Item as GunData);
                    }
                    break;
                case LoadoutUI.LoadoutType.Armor:
                    ad.Add(loadouts[i].Item as ArmorData);
                    break;
                case LoadoutUI.LoadoutType.Gadget:
                    gd.Add(loadouts[i].Item as GadgetData);
                    break;
                default:
                    break;
            }
        }
        statusText.text = "";
        statusText.text += $"HP {mainPlayer.MaximumHp} | Armor {ad[0].Stat.ArmorPoint}points" + System.Environment.NewLine;
        statusText.text += $"SP {mainPlayer.MaximumSp} | Spd {mainPlayer.Speed}m/s" + System.Environment.NewLine;
        for (int i = 0; i < wd.Count; i++)
        {
            statusText.text += $"{wd[i].ItemName}: {wd[i].Stat.MinDamage}~{wd[i].Stat.MaxDamage}Dmg" + System.Environment.NewLine;
            statusText.text += $"{wd[i].MaxAmmo}Ammo {wd[i].MaxMagazine}Magazine" + System.Environment.NewLine;
        }
        for (int i = 0; i < gd.Count; i++)
        {
            if (gd[i] is MedicineData)
            {
                MedicineData md = (MedicineData)gd[i];
                statusText.text += $"{md.ItemName}: Restore {md.RestoreHp}Hp" + System.Environment.NewLine;
            }
        }
    }

    private void DoBack()
    {
        if (isMain)
        {
            return;
        }
        if (isOptions)
        {
            isMain = true;
            ActivateOptionPanel(false);
        }
        if (isLoadout)
        {
            isMain = true;
            ActivateLoadoutPanel(false);
        }
        if (isMisison)
        {
            isMain = true;
            ActivateMissionPanel(false);
        }
        if (isMain)
        {
            ActivateBackButton(false);
            ActivateMainPanel(true);
        }
    }

    private IEnumerator DelayExecute(Action callback,float delay)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
}
