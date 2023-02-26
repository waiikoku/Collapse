using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mission" , menuName = "Custom/Data/Create MissionData")]
public class MissionData : ScriptableObject
{
    public string missionName;
    public string missionScene;
    [TextArea]
    public string missionDescription;
    public string[] missionObjectives;
    public Sprite missionPreview;
    public int missionMusicThemeID;
    public bool locked = false;
}
