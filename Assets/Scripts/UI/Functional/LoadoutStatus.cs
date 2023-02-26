using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutStatus : MonoBehaviour
{
    [SerializeField] private CharacterData profile_Player;
    [SerializeField] private GunData profile_Gun;
    [SerializeField] private MedicineData profile_Medicine;

    [SerializeField] private StatusUI player;
    [SerializeField] private StatusUI gun;
    [SerializeField] private StatusUI gunStatus;
    [SerializeField] private StatusUI medicine;

    [SerializeField] private Sprite pIcon;
    [SerializeField] private Sprite gIcon;
    [SerializeField] private Sprite gsIcon;
    [SerializeField] private Sprite mIcon;

    private void Start()
    {
        player.AssignInfo(pIcon, $"{profile_Player.CharacterName} : {profile_Player.MaximumHp}Hp");
        gun.AssignInfo(gIcon, $"{profile_Gun.ItemName.ToUpper()} : {profile_Gun.Stat.MinDamage}~{profile_Gun.Stat.MaxDamage}Dmg");
        gunStatus.AssignInfo(gsIcon, $"Bullet : {profile_Gun.MaxAmmo}/{profile_Gun.StartMagazine}");
        medicine.AssignInfo(mIcon, $"{profile_Medicine.ItemName} : {profile_Medicine.RestoreHp}Hp");
    }
}
