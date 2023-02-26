using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mob",menuName = "Custom/Data/Create MobData")]
public class MobData : ScriptableObject
{
    public string objName;
    public int MobID;
    public string MobName;
    [Tooltip("Original Icon")]
    public Sprite MobIcon;
    [Tooltip("Original Path's Icon")]
    public string iconPath;
    public GameObject MobPrefab;
}
