using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadoutUI : MonoBehaviour
{
    [SerializeField] private bool autoRun = false;
    [SerializeField] private ItemData itemData;
    public ItemData Item => itemData;

    [SerializeField] private TextMeshProUGUI loadoutName;
    [SerializeField] private Image loadoutIcon;
    public enum LoadoutType
    {
        Weapon,
        Armor,
        Gadget
    }
    [SerializeField] private LoadoutType ldType;
    public LoadoutType Type => ldType;

    private void Start()
    {
        if (autoRun)
        {
            LoadData(itemData, ldType);
        }
    }

    public void LoadData(ItemData data, LoadoutType type)
    {
        itemData = data;
        loadoutName.text = itemData.ItemName;
        loadoutIcon.sprite = itemData.ItemIcon;
        ldType = type;
    }

}
