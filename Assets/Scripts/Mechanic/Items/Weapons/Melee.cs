using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Melee : WeaponController
{
    public float ActionSpeed;
    public float Range;
    public CharacterCombat Combat;
    [SerializeField] private AudioSource meleePlayer;
    [SerializeField] private AudioClip daggerSfx;
    [SerializeField] private string targetTag = "Untagged";
    [SerializeField] private LayerMask targetMask;
    public override void PrimaryAction(InputManager.CallbackContext callback)
    {
        if(callback == InputManager.CallbackContext.Perform)
        {
            MeleeAttack();
        }
    }

    private void MeleeAttack()
    {
        MeleeData md = null;
        if (Profile is MeleeData)
        {
            md = Profile as MeleeData;
        }
        if (md == null) return;
        Collider[] cols = Physics.OverlapSphere(transform.position, md.Range, targetMask);
        foreach (var item in cols)
        {
            if (item.CompareTag(targetTag))
            {
                IDamagable damagable = item.GetComponent<IDamagable>();
                float randomDamage = Random.Range(Profile.Stat.MinDamage, Profile.Stat.MaxDamage);
                damagable.ReceiveDamage(randomDamage);
            }
        }
        meleePlayer.PlayOneShot(daggerSfx);
    }

    public override void SecondaryAction(InputManager.CallbackContext callback)
    {
        switch (callback)
        {
            case InputManager.CallbackContext.Perform:
                SetBlock(true);
                break;
            case InputManager.CallbackContext.Cancel:
                SetBlock(false);
                break;
            default:
                break;
        }
    }

    private void SetBlock(bool value)
    {

    }
}
