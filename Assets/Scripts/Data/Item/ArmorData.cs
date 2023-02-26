using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Armor", menuName = "Custom/Data/Create ArmorData")]
public class ArmorData : ItemData
{
    [System.Serializable]
    public struct ArmorStat
    {
        public float ArmorPoint;
        public float RechargeDelay;
        public float RechargeRate;
        public float RechargePercentage;
        public float RechargeTime;
    }

    public GameObject Prefab;
    public ArmorStat Stat;

}
