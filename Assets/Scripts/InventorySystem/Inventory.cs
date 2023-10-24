using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
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
        }
        else
        {
            InventoryItem newItem = new InventoryItem(data);
            inventory.Add(newItem);
            itemDictionary.Add(data, newItem);
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
        }
    }
}
