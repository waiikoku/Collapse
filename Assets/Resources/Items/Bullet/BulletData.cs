using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "Custom/Data/Create BulletData")]
public class BulletData : ItemData
{
    public GunData.GunType bulletTyp;
    public float bulletDamage = 0f;
    public float punchThrough = 0f;
}
