using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : ItemData
{
    [System.Serializable]
    public class WeaponStat
    {
        public float MinDamage;
        public float MaxDamage;
        public float CriticalChance;
        public float CriticalMultipier;
    }

    public GameObject Prefab;
    public WeaponStat Stat;
}
