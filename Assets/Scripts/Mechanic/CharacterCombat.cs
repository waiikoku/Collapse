using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombat : MonoBehaviour, IDamagable
{
    [Header("References")]
    public CharacterStat Stat;
    public CharacterData Profile;
    public ArmorData CurrentArmor;
    public WeaponController PrimaryWeapon;
    public WeaponController SecondaryWeapon;
    public GadgetController firstGadget;
    public GadgetController secondGadget;
    [SerializeField] private Transform aim;
    [SerializeField] private Animator anim;
    [Header("Attributes")]
    public bool Interactable = false;
    public bool Invulnerable = false;
    public bool onWeapon = false;
    public int currentWeaponIndex = 0;
    public bool onGadget = false;
    public int currentGadgetIndex = 0;
    [SerializeField] private bool debugMode = false;

    private int currentGunAmmo;
    private int currentGunMagazine;
    private int currentGadgetFirst;
    private float Gadgetf_Cooldown;
    public event Action<bool> OnEquipWeapon;
    public event Action<int,bool> OnEquipGadget;
    public event Action<int> OnAmmoChange;
    public event Action<int> OnMagazineChange;
    public event Action<int> OnGadgetfChange;
    public event Action<float> OnReloading;
    public event Action<float> OnUsingGadget;
    public event Action<float> OnCooldownGadgetf;
    //Properties
    public bool IsAlive
    {
        get { return Stat.currentHp > 0; }
    }
    public event Action<float> OnHealthChange;
    public event Action OnDamaged;
    public void AddDamagedListenr(Action callback)
    {
        if (callback == null) return;
        OnDamaged += callback;
    }
    private void Awake()
    {
        Stat.maximumHp = Profile.MaximumHp;
        Stat.maximumSp = Profile.MaximumSp;
        Stat.maximumArmor = CurrentArmor.Stat.ArmorPoint;
        Stat.currentHp = Stat.maximumHp;
        Stat.currentSp = Stat.maximumSp;
        Stat.currentArmor = Stat.maximumArmor;
    }

    private void Start()
    {
        if(PrimaryWeapon is Gun)
        {
            Gun PrimGun = (Gun)PrimaryWeapon;
            PrimGun.UpdateFirerate();
            PrimGun.OnAmmoChange += UpdateCurrentAmmo;
            PrimGun.OnMagazineChange += UpdateCurrentMagazine;
            PrimGun.OnReloading += UpdateReloading;
            onWeapon = true;
            onGadget = false;
        }
        if(firstGadget is Medkit)
        {
            Medkit medkit = (Medkit)firstGadget;
            medkit.OnHeal += Healing;
            medkit.OnCooldown += UpdateGadgetfCooldown;
            medkit.OnHolding += Using;
            medkit.OnAmountChange += UpdateGadgetfChange;
        }
        Interactable = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            Healing(1000);
        }
    }

    private void Using(float percentage)
    {
        OnUsingGadget?.Invoke(percentage);
    }

    private void UpdateGadgetfChange(int amount)
    {
        currentGadgetFirst = amount;
        OnGadgetfChange?.Invoke(currentGadgetFirst);
    }

    private void UpdateGadgetfCooldown(float value)
    {
        Gadgetf_Cooldown = value;
        OnCooldownGadgetf?.Invoke(Gadgetf_Cooldown);
    }

    public void RequireInfo()
    {
        print("Request for Info!");
        float percentage = Stat.currentHp / Stat.maximumHp;
        OnHealthChange?.Invoke(percentage);
        if (PrimaryWeapon is Gun)
        {
            print("Requesting to Gun");
            Gun PrimGun = (Gun)PrimaryWeapon;
            PrimGun.UpdateInfo();
        }
        if (firstGadget is Medkit)
        {
            print("Requesting to Med");
            Medkit med = (Medkit)firstGadget;
            med.UpdateInfo();
        }
    }

    private void UpdateCurrentAmmo(GunData.GunAmmo data)
    {
        currentGunAmmo = data.Ammo;
        OnAmmoChange?.Invoke(currentGunAmmo);
    }

    private void UpdateCurrentMagazine(GunData.GunMagazine mag)
    {
        currentGunMagazine = mag.Magazine;
        OnMagazineChange?.Invoke(currentGunMagazine);
    }

    private void UpdateReloading(float percentage)
    {
        OnReloading?.Invoke(percentage);
    }

    private void LateUpdate()
    {
        if (isHit)
        {
            hitTimer += Time.deltaTime;
            if(hitTimer > hurtDuration)
            {
                isHit = false;
            }
        }
        else
        {
            Healing(passiveHealRate * Time.deltaTime);
        }
    }

    private bool isHit = false;
    private float hitTimer = 0f;
    [SerializeField] private float hurtDuration = 1f;
    [SerializeField] private float passiveHealRate = 10f;

    public void ReceiveDamage(float dmg)
    {
        if (Stat.currentHp == 0) return;
        Stat.currentHp -= dmg;
        Stat.currentHp = Mathf.Clamp(Stat.currentHp, 0, Stat.maximumHp);
        if(Stat.currentHp == 0)
        {
            Died();
            Debugf($"Player has Died!");
        }
        else
        {
            Debugf($"Player received {dmg}Dmg!");
        }
        float percentage = Stat.currentHp / Stat.maximumHp;
        OnHealthChange?.Invoke(percentage);
        isHit = true;
        hitTimer = 0f;
        OnDamaged?.Invoke();
        GameManager.Instance.PlayerHurt();
        /*
        float trueDamage = dmg;
        if (Stat.currentArmor > 0)
        {
            float scamd = Stat.currentArmor - dmg;
            if (scamd >= 0)
            {
                Stat.currentArmor -= dmg;
                trueDamage = 0;
            }
            else
            {
                trueDamage = Mathf.Abs(scamd);
                Stat.currentArmor = 0;
                goto PureDmg;
            }
        }
    PureDmg:
        if (Stat.currentHp - trueDamage > 0)
        {
            Stat.currentHp -= trueDamage;
            Debugf($"Player received {trueDamage}Dmg!");
        }
        else
        {
            Stat.currentHp = 0;
            Died();
        }

    */
    }

    public void Healing(float hp)
    {
        if (Stat.currentHp == Stat.maximumHp) return;
        Stat.currentHp += hp;
        Stat.currentHp = Mathf.Clamp(Stat.currentHp, 0, Stat.maximumHp);
        float percentage = Stat.currentHp / Stat.maximumHp;
        OnHealthChange?.Invoke(percentage);
    }

    private void Died()
    {
        GameManager.Instance.PlayerDied();
    }

    public void Interact(InputManager.MouseState mouse, InputManager.CallbackContext callback)
    {
        if (Interactable == false) return;
        if (onWeapon)
        {
            InteractWeapon(mouse,callback);
        }
        else if (onGadget)
        {
            InteractGadget(mouse, callback);
        }
        else
        {
            Debugf("Invalid Equipment! Nothing on hand");
        }
    }

    private void InteractWeapon(InputManager.MouseState mouse, InputManager.CallbackContext callback)
    {
        if (currentWeaponIndex == 0)
        {
            Debugf("Interact With MainWeapon");
            if (mouse == InputManager.MouseState.Mouse0)
            {
                Debugf(string.Format("Primary Action With MainWeapon({0})", callback));
                PrimaryWeapon.PrimaryAction(callback);
            }
            else if (mouse == InputManager.MouseState.Mouse1)
            {
                PrimaryWeapon.SecondaryAction(callback);
                /*
                switch (callback)
                {
                    case InputManager.CallbackContext.Perform:
                        anim.SetLayerWeight(1, 1);
                        break;
                    case InputManager.CallbackContext.Cancel:
                        anim.SetLayerWeight(1, 0);
                        break;
                    default:
                        break;
                }
                */
            }
        }
        else if (currentWeaponIndex == 1)
        {
            Debugf("Interact With SecondaryWeapon");
            if (mouse == InputManager.MouseState.Mouse0)
            {
                Debugf(string.Format("Primary Action With SecWeapon({0})", callback));
                SecondaryWeapon.PrimaryAction(callback);
            }
            else if (mouse == InputManager.MouseState.Mouse1)
            {
                SecondaryWeapon.SecondaryAction(callback);
            }
        }
    }

    private void InteractGadget(InputManager.MouseState mouse, InputManager.CallbackContext callback)
    {
        if (currentGadgetIndex == 0)
        {
            Debugf("Interact With FirstGadget");
            if (mouse == InputManager.MouseState.Mouse0)
            {
                Debugf(string.Format("Primary Action With 1st Gadget({0})", callback));
                firstGadget.PrimaryAction(callback);
            }
            else if (mouse == InputManager.MouseState.Mouse1)
            {
                firstGadget.SecondaryAction(callback);
            }
        }
        else if (currentGadgetIndex == 1)
        {
            Debugf("Interact With SecondGadget");
            if (mouse == InputManager.MouseState.Mouse0)
            {
                Debugf(string.Format("Primary Action With 2nd Gadget({0})", callback));
                secondGadget.PrimaryAction(callback);
            }
            else if (mouse == InputManager.MouseState.Mouse1)
            {
                secondGadget.SecondaryAction(callback);
            }
        }
    }

    public void ReloadGun()
    {
        if(currentWeaponIndex == 0)
        {
            if (PrimaryWeapon is not Gun) return;
            Gun PrimGun = (Gun)PrimaryWeapon;
            Debugf("Try to reload!");
            PrimGun.Reload();
        }
        else if(currentWeaponIndex == 1)
        {

        }
    }

    public void EquipWeapon(int index, InputManager.CallbackContext callback)
    {
        ActivateGadget(currentGadgetIndex, false);
        ActivateWeapon(index, true);
    }

    public void UseGadget(int index, InputManager.CallbackContext callback)
    {
        ActivateWeapon(currentWeaponIndex, false);
        currentGadgetIndex = index;
        ActivateGadget(currentGadgetIndex, true);
        if(currentGadgetIndex == 0)
        {
            firstGadget.TriggerAction(callback);
        }
        else if(currentGadgetIndex == 1)
        {
            secondGadget.TriggerAction(callback);
        }
    }
    
    private void ActivateWeapon(int index,bool value)
    {
        if (index == 0)
        {
            IInteractable interactable = PrimaryWeapon.GetComponent<IInteractable>();
            if (interactable == null)
            {
                print("PrimaryWeapon interactable Is Null");
                return;
            }
            interactable.Activate(value);
        }
        else if (index == 1)
        {
            IInteractable interactable = SecondaryWeapon.GetComponent<IInteractable>();
            if (interactable == null)
            {
                print("SecondaryWeapon interactable Is Null");
                return;
            }
            interactable.Activate(value);
        }
        currentWeaponIndex = index;
        onWeapon = value;
        OnEquipWeapon?.Invoke(onWeapon);
    }

    private void ActivateGadget(int index, bool value)
    {
        if (index == 0)
        {
            if(value == false)
            {
                firstGadget.Unequip();
            }
            IInteractable interactable = firstGadget.GetComponent<IInteractable>();
            if(interactable == null)
            {
                print("PrimaryGadget interactable Is Null");
                return;
            }
            interactable.Activate(value);
        }
        else if (index == 1)
        {
            /*
            IInteractable interactable = secondGadget.GetComponent<IInteractable>();
            if (interactable == null)
            {
                print("SecondaryGadget interactable Is Null");
                return;
            }
            interactable.Activate(value);
            */
        }
        currentGadgetIndex = index;
        onGadget = value;
        OnEquipGadget?.Invoke(currentGadgetIndex,onGadget);
    }


    private void Debugf(string message)
    {
        if (debugMode == false) return;
        print(message);
    }
}
