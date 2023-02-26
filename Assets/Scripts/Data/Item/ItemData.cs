using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item",menuName = "Custom/Data/Create Item")]
public class ItemData : ScriptableObject
{
    public int ItemID;
    public string ItemName;
    public Sprite ItemIcon;
    public string ItemDescription;
}

[System.Serializable]
public struct ItemQuantity
{
    public ItemData item;
    public int quantity;
    public int characterID;
}
