using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : Singleton<OptionMenu>
{

    [Header("References")]
    [SerializeField] private GraphicManager gm;
    [SerializeField] private SoundManager sm;

    [Header("Panel")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject graphicsPanel;
    [SerializeField] private GameObject soundPanel;

    [Header("Button")]
    [SerializeField] private Button btn_Resume;
    [SerializeField] private Button btn_Restart;
    [SerializeField] private Button btn_Option;
    [SerializeField] private Button btn_ReturnToMenu;
    [SerializeField] private Button btn_Back;


    [SerializeField] private Button btn_Video;
    [SerializeField] private Button btn_Sound;

    [Header("State")]
    public MenuState m_MenuState;
    public OptionState m_OptionState;
    public ControlState m_ControlState;
    public VideoState m_VideoState;

    [Header("UI Event")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private bool isPause = false;
    public enum MenuState
    {
        Main,
        Option,
        Loadout
    }
    #region Options
    public enum OptionState
    {
        Main,
        Controls,
        Video,
        Sound,
        Advanced,
        Credits
    }

    public enum ControlState
    {
        Main,
        Edit
    }

    public enum VideoState
    {
        Main,
        Resolution,
        Advanced
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        if (isDuplicate)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        musicSlider.onValueChanged.AddListener(SoundManager.Instance.AdjustMusicVolume);
        sfxSlider.onValueChanged.AddListener(SoundManager.Instance.AdjustSfxVolume);

        btn_Resume.onClick.AddListener(delegate { ActivatePauseMenu(false); });
        btn_Restart.onClick.AddListener(delegate { LevelManager.Instance.ReloadCurrentScene(); });
        btn_ReturnToMenu.onClick.AddListener(delegate { Time.timeScale = 1f; GameManager.Instance.ActivatHUD(false); LevelManager.Instance.LoadMain(delegate { GameManager.Instance.gameStage = GameManager.GameStage.Main; SoundManager.Instance.ChangeMusic(0); ActivatePauseMenu(false); Cursor.visible = true; Cursor.lockState = CursorLockMode.Confined; }); });

        btn_Option.onClick.AddListener(delegate { ActivateOption(true); ChangeBackListener(delegate { ActivateOption(false); }); });
        btn_Video.onClick.AddListener(delegate { ChangeBackListener(delegate { ActivateVideo(false); ActivateOption(true); ChangeBackListener(delegate { ActivateOption(false); }); }); ActivateVideo(true); });
        btn_Sound.onClick.AddListener(delegate { ChangeBackListener(delegate { ActivateSound(false); ActivateOption(true); ChangeBackListener(delegate { ActivateOption(false); }); }); ActivateSound(true); });
    }

    private void ChangeBackListener(UnityEngine.Events.UnityAction callback)
    {
        btn_Back.onClick.RemoveAllListeners();
        btn_Back.onClick.AddListener(callback);
    }

    public void SetActive(bool value)
    {
        mainPanel.SetActive(!value);
        optionsPanel.SetActive(value);
    }

    private void ActivateOption(bool value)
    {
        mainPanel.SetActive(!value);
        optionsPanel.SetActive(value);
        btn_Back.gameObject.SetActive(value);
        m_MenuState = value ? OptionMenu.MenuState.Option : OptionMenu.MenuState.Main;
    }

    private void ActivateVideo(bool value)
    {
        optionsPanel.SetActive(!value);
        graphicsPanel.SetActive(value);
        m_OptionState = value ? OptionMenu.OptionState.Video : OptionMenu.OptionState.Main;
    }

    private void ActivateSound(bool value)
    {
        optionsPanel.SetActive(!value);
        soundPanel.SetActive(value);
        m_OptionState = value ? OptionMenu.OptionState.Sound : OptionMenu.OptionState.Main;
    }

    public void SwitchPause()
    {
        ActivatePauseMenu(!isPause);
    }

    public void ActivatePauseMenu(bool value)
    {
        print($"Pause : {value}");
        isPause = value;
        Time.timeScale = value ? 0f : 1f;
        menuPanel.SetActive(value);
        Cursor.visible = value;
        Cursor.lockState = value ? CursorLockMode.Confined : CursorLockMode.None;
    }

    public bool IsPause()
    {
        return isPause;
    }
}
