using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillData : ScriptableObject
{
    public int SkillID;
    public string SkillName;
    public Sprite SkillIcon;
    public string SkillDescription;
    public int SkillPointRequire;
    public float SpRequire;
    public float Cooldown;
}
