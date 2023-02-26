using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun",menuName ="Custom/Data/Create GunData")]
public class GunData : WeaponData
{
    public int MaxAmmo;
    public int StartMagazine;
    public int MaxMagazine;
    public float Firerate;
    public float BulletSpeed;
    public float ReloadTime;

    public enum GunType
    {
        Null,
        Pistol,
        Rifle,
        Shotgun,
        Special
    }

    public struct GunAmmo
    {
        public int Ammo;
        public int MaxAmmo;

        public GunAmmo(int am,int ma)
        {
            Ammo = am;
            MaxAmmo = ma;
        }
    }

    public struct GunMagazine
    {
        public int Magazine;
        public int MaxMagazine;

        public GunMagazine(int mag,int max)
        {
            Magazine = mag;
            MaxMagazine = max;
        }
    }
}
