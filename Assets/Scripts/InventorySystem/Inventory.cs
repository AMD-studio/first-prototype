using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static event Action<List<InventoryItem>> OnInventoryChanged;

    public List<InventoryItem> inventory = new List<InventoryItem>();
    private Dictionary<ItemData, InventoryItem> itemDictionary;

    private void OnEnable()
    {
        Stick.OnStickCollected += Add;
    }
    private void OnDisable()
    {
        Stick.OnStickCollected -= Add;
    }

    public void Add(ItemData data)
    {
        if (itemDictionary.TryGetValue(data, out InventoryItem item))
        {
            item.AddToStack();
            OnInventoryChanged?.Invoke(inventory);
            Debug.Log("Add item");
        }
        else
        {
            InventoryItem newItem = new InventoryItem(data);
            inventory.Add(newItem);
            itemDictionary.Add(data, newItem);
            OnInventoryChanged?.Invoke(inventory);
            Debug.Log("Add item");
        }
    }

    public void Remove(ItemData data)
    {
        if (itemDictionary.TryGetValue(data, out InventoryItem item))
        {
            item.RemoveFromStack();
            if(item.stackSize < 1)
            {
                inventory.Remove(item);
                itemDictionary.Remove(data);
            }
            OnInventoryChanged?.Invoke(inventory);
        }
    }
}
