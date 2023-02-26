using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusReceiver : MonoBehaviour
{
    [SerializeField] private CharacterCombat cc;
    [SerializeField] private Slider healthBar;

    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI magazineText;

    [SerializeField] private GameObject Guage;
    [SerializeField] private Image reloadGuauge;

    [SerializeField] private GameObject aguParent;
    [SerializeField] private Image allGadget_Using;

    [SerializeField] private GameObject selectorFirstGadget;
    [SerializeField] private Image firstGadget_Cooldown;
    [SerializeField] private TextMeshProUGUI firstGadget_Amount;
    private float timeout = 0.2f;
    private float timestamp;
    private bool isTimedout = false;
    private void LateUpdate()
    {
        if(Time.time > timestamp)
        {
            timestamp = 0;
            if(isTimedout == false)
            {
                isTimedout = true;
                aguParent.SetActive(false);
            }
        }
    }

    public void SetCC(CharacterCombat combat)
    {
        cc = combat;
        cc.OnHealthChange += UpdateHealth;
        cc.OnAmmoChange += UpdateAmmo;
        cc.OnMagazineChange += UpdateMagazine;
        cc.OnReloading += UpdateReloadGuage;
        cc.OnCooldownGadgetf += UpdateFirstGadgetCooldown;
        cc.OnGadgetfChange += UpdateFirstGadget;
        cc.OnUsingGadget += UpdateGadgetUsing;
        cc.OnEquipGadget += ActivateSelectors;
        cc.RequireInfo();
    }

    private void UpdateHealth(float value)
    {
        healthBar.value = value;
    }

    private void UpdateAmmo(int ammo)
    {
        ammoText.text = ammo.ToString();
    }

    private void UpdateMagazine(int magazine)
    {
        magazineText.text = magazine.ToString();
    }

    private void UpdateReloadGuage(float percentage)
    {
        if(percentage == 1)
        {
            Guage.SetActive(false);
            ammoText.gameObject.SetActive(true);
            magazineText.gameObject.SetActive(true);
        }
        else
        {
            if(Guage.activeInHierarchy == false)
            {
                ammoText.gameObject.SetActive(false);
                magazineText.gameObject.SetActive(false);
                Guage.SetActive(true);
            }
        }
        reloadGuauge.fillAmount = percentage;
    }

    private void UpdateFirstGadget(int amount)
    {
        firstGadget_Amount.text = amount.ToString();
    }


    private void UpdateFirstGadgetCooldown(float percentage)
    {
        firstGadget_Cooldown.fillAmount = 1 - percentage;
    }

    private void UpdateGadgetUsing(float percentage)
    {
        if (aguParent.activeInHierarchy == false)
        {
            isTimedout = false;
            aguParent.SetActive(true);
        }
        allGadget_Using.fillAmount = percentage;
        timestamp = Time.time + timeout;
    }

    private void ActivateSelectors(int index,bool value)
    {
        if(index == 0)
        {
            selectorFirstGadget.SetActive(value);
            return;
        }
    }
}
