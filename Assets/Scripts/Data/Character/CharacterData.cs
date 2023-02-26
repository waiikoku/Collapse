using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Custom/Data/Create CharacterData")]
public class CharacterData : ScriptableObject
{
    public int CharacterID;
    public string CharacterName;
    /*
    public string characterDescription;
    public string characterType;
    public string characterClass;
    */
    public float MaximumHp;
    public float MaximumSp;
    public float Speed;
}
