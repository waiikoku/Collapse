using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MissionManager : Singleton<MissionManager>
{
    [SerializeField] private string assetPath = "Resources/Datas";
    [SerializeField] private MissionData[] missions;

    public MissionData currentMission;

    [SerializeField] private bool directLoad = false;
    private bool isLoaded = false;
    protected override void Awake()
    {
        base.Awake();
        if (directLoad)
        {
            missions = Resources.LoadAll<MissionData>(assetPath);
            isLoaded = true;
        }
        else
        {
            StartCoroutine(Preload());
        }

    }

    private IEnumerator Preload()
    {
        MissionData[] request = null;
        while (request == null)
        {
            request = Resources.LoadAll<MissionData>(assetPath);
            yield return null;
        }
        missions = request;
        isLoaded = true;
    }

    public MissionData[] GetAllMission()
    {
        if (isLoaded == false)
        {
            return null;
        }
        return missions;
    }

    public void LoadMission(int index)
    {
        LevelManager.Instance.LoadScene(missions[index].missionScene);
    }

    public void LoadMission(string missionName)
    {
        int missionIndex = System.Array.FindIndex(missions, mission => missionName == mission.missionName);
        if (missions.Length >= missionIndex)
        {
            currentMission = missions[missionIndex];
            UnityAction<Scene, LoadSceneMode> action = delegate { GameManager.Instance.gameStage = GameManager.GameStage.Gameplay; GameManager.Instance.SpawnPlayer(currentMission.missionName.Contains("Debug")? true:false); SoundManager.Instance.ChangeMusic(currentMission.missionMusicThemeID); };
            LevelManager.Instance.LoadScene(missions[missionIndex].missionScene,action);
        }
        else
        {
            print("Invalid Index! Maybe MissionName is Incorrect?");
        }
    }

    public void ClearMission()
    {
        currentMission = null;
    }
}
