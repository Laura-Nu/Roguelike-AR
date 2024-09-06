using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_", menuName = "ScriptableObjects/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public Sprite sprite;
    public int coinCost;
}
