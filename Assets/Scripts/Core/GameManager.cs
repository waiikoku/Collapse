using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("References")]
    [SerializeField] private LoadoutManager loadouts;
    [SerializeField] private MissionManager missions;
    [SerializeField] private InputManager input;
    [Header("Panel")]
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject diedPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private UnityEngine.UI.Button restartBtn;
    [SerializeField] private UnityEngine.UI.Button[] returnBtn;

    [Header("Attributes")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject playerDebug;
    [SerializeField] private GameObject camManagerPrefab;
    [SerializeField] private CharacterData currentCharacter;
    [SerializeField] private FadeController fadeController;
    public event System.Action OnHurt;
    public event Action OnPlayerDied;
    public event Action<PlayerAgent, CameraManager> OnLoadPlayer;
    public enum GameStage
    {
        Main,
        Gameplay
    }
    public GameStage gameStage = GameStage.Main;
    protected override void Awake()
    {
        base.Awake();
        if (isDuplicate)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        //returnBtn.onClick.AddListener(delegate { print("Returning"); LevelManager.Instance.LoadScene("Main", delegate { ActivateHUDPanel(false); ActivateDiedPanel(false); SoundManager.Instance.ChangeMusic(0); }); });

        foreach (var btn in returnBtn)
        {
            btn.onClick.AddListener(delegate { print("Returning"); Time.timeScale = 1f; LevelManager.Instance.LoadMain(delegate { ActivateHUDPanel(false); ActivateDiedPanel(false); SoundManager.Instance.ChangeMusic(0); }); });
        }
    }
    private PlayerAgent currentPlayer;
    public void ActivatHUD(bool value)
    {
        hud.SetActive(value);
    }

    public CharacterData GetCurrentPlayer()
    {
        return currentCharacter;
    }
    public void SpawnPlayer(bool debug = false)
    {
        Startpoint sp = Startpoint.Instance;
        Transform p = null;
        if (sp != null)
        {
            p = sp.GetPoint();
        }
        GameObject player = Instantiate(debug? playerDebug: playerPrefab, p.position, p.rotation);
        CameraManager cm = SpawnCamera();
        cm.target = player.transform;
        PlayerAgent pa = player.GetComponent<PlayerAgent>();
        currentCharacter = pa.Profile;
        currentPlayer = pa;
        cm.LocalStatus.SetCC(pa.Combat);
        input.SetAgent(pa);
        input.SetCam(cm);
        ActivatHUD(true);
        OnLoadPlayer?.Invoke(pa, cm);
    }

    public void DespawnPlayer(bool includeCamera = false)
    {
        PlayerAgent pa = input.GetAgent();
        if (pa != null)
        {
            Destroy(pa.gameObject);
        }
        input.SetAgent(null);
        CameraManager cm = input.GetCam();
        if (includeCamera)
        {
            if (cm != null)
            {
                Destroy(cm.gameObject);
            }
            input.SetCam(null);
        }
        else
        {
            cm.target = null;
        }
    }

    public void PlayerDisableMovement()
    {
        currentPlayer.Movement.canMove = false;
    }

    public void PlayerDied()
    {
        currentPlayer.Movement.SpeedMultipier(0f);
        ActivateDiedPanel(true);
        SoundManager.Instance.PauseMusic(true);
        AgentMovement[] all_AI = FindObjectsOfType<AgentMovement>();
        foreach (var ai in all_AI)
        {
            ai.SetDestTarget(null);
        }
        Spawner[] spawners = FindObjectsOfType<Spawner>();
        foreach (var spawner in spawners)
        {
            spawner.StopAllCoroutines();
        }
        DespawnPlayer();
        OnPlayerDied?.Invoke();
    }

    private void ActivateHUDPanel(bool value)
    {
        hud.SetActive(value);
    }


    private void ActivateDiedPanel(bool value)
    {
        diedPanel.SetActive(value);
    }

    public void ActivateVictory(bool value)
    {
        victoryPanel.SetActive(value);
        AgentMovement[] all_AI = FindObjectsOfType<AgentMovement>();
        foreach (var ai in all_AI)
        {
            ai.SetDestTarget(null);
        }
        Spawner[] spawners = FindObjectsOfType<Spawner>();
        foreach (var spawner in spawners)
        {
            spawner.StopAllCoroutines();
        }
        DespawnPlayer(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ActiveFade(bool value,float duration)
    {
        fadeController.Activate(value,duration);
    }

    public bool InputAvailable()
    {
        bool a = input.GetAgent();
        bool b = input.GetCam();
        return a & b;
    }

    private CameraManager SpawnCamera()
    {
        GameObject cam = Instantiate(camManagerPrefab);
        return cam.GetComponent<CameraManager>();
    }

    public void PlayerHurt()
    {
        OnHurt?.Invoke();
    }

    public event Action<ItemQuantity> OnAddItem;
    public void AddItems(ItemQuantity[] items)
    {
        print($"Give item to player ({items.Length})");
        for (int i = 0; i < items.Length; i++)
        {
            print($"{items[i].item.ItemName} x{items[i].quantity}");
            OnAddItem?.Invoke(items[i]);
        }
    }
}
