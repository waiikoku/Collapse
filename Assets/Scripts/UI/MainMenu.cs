using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button btn_Start;
    [SerializeField] private Button btn_Option;
    [SerializeField] private Button btn_Quit;
    [SerializeField] private Button btn_Back;

    [SerializeField] private Button btn_Loadout;

    [SerializeField] private Button btn_Controls;
    [SerializeField] private Button btn_Video;
    [SerializeField] private Button btn_Sound;
    [SerializeField] private Button btn_SoundReset;
    [SerializeField] private Button btn_Advanced;
    [SerializeField] private Button btn_Credits;

    [SerializeField] private Button[] btn_Missions;

    [SerializeField] private MissionUI[] missionUIs;
    [SerializeField] private MissionDetailUI missionDetailUI;

    [Header("Panel")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private GameObject loadoutPanel_Bg;
    [SerializeField] private GameObject loadoutPanel;
    [SerializeField] private GameObject planPanel;
    [SerializeField] private GameObject planSelectPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject graphicsPanel;
    [SerializeField] private GameObject soundPanel;
    [SerializeField] private GameObject buttonBack;

    [Header("State")]
    public OptionMenu.MenuState MenuState;
    public OptionMenu.OptionState OptionState;
    public OptionMenu.ControlState ControlState;
    public OptionMenu.VideoState VideoState;

    [Header("UI Event")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        musicSlider.onValueChanged.AddListener(SoundManager.Instance.AdjustMusicVolume);
        sfxSlider.onValueChanged.AddListener(SoundManager.Instance.AdjustSfxVolume);
        SoundManager.Instance.ChangeMusic(0);

        MissionData[] preload = MissionManager.Instance.GetAllMission();
        for (int i = 0; i < missionUIs.Length; i++)
        {
            int index = i;
            if (preload.Length - 1 >= i)
            {
                missionUIs[index].LoadData(preload[i]);
            }
            else
            {
                missionUIs[index].LoadData(null);
            }
            missionUIs[index].AddListener(delegate { buttonBack.SetActive(false); SelectMission(missionUIs[index].missionData); ActivateSelectPlan(true); });
        }
    }

    private void OnEnable()
    {
        btn_Start.onClick.AddListener(
            delegate
            {
                buttonBack.SetActive(true); 
                ChangeBackListener(
                delegate
                {
                    buttonBack.SetActive(false);
                    ActivatePlan(false);
                    ChangeBackListener(
                    delegate 
                    { 
                        ActivateOption(false);
                    });
                }); 
                ActivatePlan(true); 
            });

        btn_Loadout.onClick.AddListener(delegate { ActivateLoadout(true); ChangeBackListener(delegate { ActivateLoadout(false); }); });
        btn_Option.onClick.AddListener(delegate { ActivateOption(true); ChangeBackListener(delegate { ActivateOption(false); }); });

        //btn_Controls.onClick.AddListener(delegate { ActivateControls(true); });
        btn_Video.onClick.AddListener(delegate { ChangeBackListener(delegate { ActivateVideo(false); ActivateOption(true); ChangeBackListener(delegate { ActivateOption(false); }); }); ActivateVideo(true); });
        btn_Sound.onClick.AddListener(delegate { ChangeBackListener(delegate { ActivateSound(false); ActivateOption(true); ChangeBackListener(delegate { ActivateOption(false); }); }); ActivateSound(true); });
        //btn_Sound.onClick.AddListener(delegate { ActivateSound(true); });

        btn_Quit.onClick.AddListener(QuitGame);
    }

    private void OnDestroy()
    {

        btn_Option.onClick.RemoveListener(delegate { ActivateOption(true); });
        btn_Quit.onClick.RemoveListener(QuitGame);

        btn_Back.onClick.RemoveAllListeners();
        //btn_Controls.onClick.RemoveListener(delegate { ActivateControls(true); });
        btn_Video.onClick.RemoveListener(delegate { ChangeBackListener(delegate { ActivateVideo(false); ActivateOption(false); }); ActivateVideo(true); });
        btn_Sound.onClick.RemoveListener(delegate { ChangeBackListener(delegate { ActivateSound(false); ActivateOption(false); }); ActivateSound(true); });
        //btn_Sound.onClick.RemoveListener(delegate { ActivateSound(true); });

        musicSlider.onValueChanged.RemoveListener(SoundManager.Instance.AdjustMusicVolume);
        sfxSlider.onValueChanged.RemoveListener(SoundManager.Instance.AdjustSfxVolume);
    }

    private void ActivatePlan(bool value)
    {
        mainPanel.SetActive(!value);
        mapPanel.SetActive(value);
        planPanel.SetActive(value);
    }

    private void ActivateSelectPlan(bool value)
    {
        planSelectPanel.SetActive(value);
    }


    private void ChangeBackListener(UnityEngine.Events.UnityAction callback)
    {
        btn_Back.onClick.RemoveAllListeners();
        btn_Back.onClick.AddListener(callback);
    }

    private void ActivateOption(bool value)
    {
        mainPanel.SetActive(!value);
        optionsPanel.SetActive(value);
        buttonBack.SetActive(value);
        MenuState = value ? OptionMenu.MenuState.Option : OptionMenu.MenuState.Main;
    }

    private void ActivateLoadout(bool value)
    {
        mainPanel.SetActive(!value);
        loadoutPanel.SetActive(value);
        loadoutPanel_Bg.SetActive(value);
        buttonBack.SetActive(value);
        MenuState = value ? OptionMenu.MenuState.Loadout : OptionMenu.MenuState.Main;
    }

    private void ActivateControls(bool value)
    {
        optionsPanel.SetActive(!value);
        MenuState = value ? OptionMenu.MenuState.Option : OptionMenu.MenuState.Main;

    }

    private void ActivateVideo(bool value)
    {
        optionsPanel.SetActive(!value);
        graphicsPanel.SetActive(value);
        OptionState = value ? OptionMenu.OptionState.Video : OptionMenu.OptionState.Main;
    }

    private void ActivateSound(bool value)
    {
        optionsPanel.SetActive(!value);
        soundPanel.SetActive(value);
        OptionState = value ? OptionMenu.OptionState.Sound : OptionMenu.OptionState.Main;
    }

    private MissionData currentSelection;
    public void SelectMission(MissionData data)
    {
        currentSelection = data;
        missionDetailUI.Select(currentSelection);
    }
    private void QuitGame()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
    }

}
