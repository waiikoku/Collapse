using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : GadgetController , IInteractable
{
    private bool loaded = false;
    private MedicineData medicine;
    [SerializeField] private bool addable = false;
    public int Amount { get; private set; }

    private bool equiped = false;
    private bool used = false;
    private bool isCooldown = false;
    private bool holding = false;

    [SerializeField] private bool CanUnequip = false;
    [SerializeField] private bool hasCooldown = false; //If Not, Item can be use for One-time Only
    [SerializeField] private bool HoldToUse = false; //If Not, Item will instant use

    [SerializeField] private float cooldownDuration = 1f;
    [SerializeField] private float holdingDuration = 3f;

    public event Action<float> OnHeal;
    public event Action<float> OnCooldown;
    public event Action<float> OnHolding;
    public event Action<int> OnAmountChange;
    private Coroutine holdingUse;

    [SerializeField] private AudioClip soundEffect;
    private void Start()
    {
        medicine = Profile as MedicineData;
        Amount = 1;
        if(addable) GameManager.Instance.OnAddItem += AddKit;
        loaded = true;
    }
    public void UpdateInfo()
    {
        if (loaded == false)
        {
            StartCoroutine(WaitUntilLoad());
        }
        else
        {
            OnCooldown?.Invoke(1);
            OnHolding?.Invoke(0);
            OnAmountChange?.Invoke(Amount);
            print("Updated!");
        }
    }

    private IEnumerator WaitUntilLoad()
    {
        while (loaded == false)
        {
            print("Waiting!");
            yield return null;
        }
        OnCooldown?.Invoke(1);
        OnHolding?.Invoke(0);
        OnAmountChange?.Invoke(Amount);
        print("Wait then Updated!");
        yield break;
    }
    public override void PrimaryAction(InputManager.CallbackContext callback)
    {
        if (equiped == false) return; //Prevent Using without Equip
        if (isCooldown) return; //Cooling down
        if (used == true) return;
        if (Amount == 0)
        {
            print("Out of Medkit!");
            return;
        }
        if (callback == InputManager.CallbackContext.Perform)
        {
            if (HoldToUse)
            {
                if (holding == false)
                {
                    holding = true;
                    holdingUse = StartCoroutine(HoldingThread());
                }
            }
            else
            {
                Use();
            }
        }
        if(callback == InputManager.CallbackContext.Cancel)
        {
            if (HoldToUse)
            {
                if (holding)
                {
                    if(holdingUse != null)
                    {
                        StopCoroutine(holdingUse);
                    }
                    holding = false;
                }
            }
        }
    }

    public override void SecondaryAction(InputManager.CallbackContext callback)
    {
    }

    public override void TriggerAction(InputManager.CallbackContext callback)
    {
        switch (callback)
        {
            case InputManager.CallbackContext.Perform:
                //Design to be Toggle Function
                equiped = !equiped;
                print($"{gameObject.name} is Equip[{equiped}]");
                break;
            case InputManager.CallbackContext.Cancel:

                break;
            default:
                break;
        }
    }

    private IEnumerator CooldownThread()
    {
        float timer = 0f;
        float percentage;
        while (true)
        {
            timer += Time.deltaTime;
            percentage = timer / cooldownDuration;
            OnCooldown?.Invoke(percentage);
            yield return null;
            if(percentage >= cooldownDuration)
            {
                print("Cooldown Finished!");
                break;
            }
        }
        isCooldown = false;
        used = false;
        yield break;
    }

    private void Use()
    {
        used = true;
        Amount--;
        OnAmountChange?.Invoke(Amount);
        OnHeal?.Invoke(medicine.RestoreHp);
        print($"Medkit has applied! Restore {medicine.RestoreHp} HP");
        if(soundEffect != null)
        {
            SoundManager.Instance.PlaySfx(soundEffect);
        }
        if (hasCooldown)
        {
            isCooldown = true;
            StartCoroutine(CooldownThread());
        }
    }

    public override void Unequip()
    {
        if (CanUnequip == false) return;
        equiped = false;
    }

    private IEnumerator HoldingThread()
    {
        float timer = 0f;
        float percentage;
        print($"Using {medicine.ItemName}");
        while (true)
        {
            timer += Time.deltaTime;
            percentage = timer / holdingDuration;
            OnHolding?.Invoke(percentage);
            yield return null;
            if (timer >= holdingDuration)
            {
                break;
            }
            if(equiped == false)
            {
                yield break;
            }
        }
        holding = false;
        holdingUse = null;
        Use();
        yield break;
    }

    public void AddKit(ItemQuantity kit)
    {
        if (kit.item.ItemID != medicine.ItemID) return;
        Amount += kit.quantity;
        print("Add Medkit!");
        OnAmountChange?.Invoke(Amount);
    }
    [SerializeField] private GameObject model;
    public void Activate(bool value)
    {
        model.SetActive(value);
    }
}
