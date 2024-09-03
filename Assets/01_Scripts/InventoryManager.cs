using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<ItemUI> items = new List<ItemUI>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void AddItem(ItemSO newItem)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item != null)
            {
                if (items[i].item.itemName == newItem.itemName)
                {
                    items[i].itemCount++;
                    items[i].UpdateInfo();
                    return;
                }
            }
        }
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == null)
            {
                items[i].itemCount = 1;
                items[i].Init(newItem);
                return;
            }
        }
    }
}
