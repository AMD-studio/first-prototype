using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject slotPrefab;
    public List<InventorySlot> slots = new List<InventorySlot>(8);

    private void OnEnable()
    {
        Inventory.OnInventoryChanged += DrawInventory;
    }

    private void OnDisable()
    {
        Inventory.OnInventoryChanged -= DrawInventory;
    }

    private void ResetInventory()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        slots = new List<InventorySlot>(8);
    }

    private void DrawInventory(List<InventoryItem> inventory)
    {
        ResetInventory();
        for (int i = 0; i < slots.Capacity; i++) 
        {
            CreateInventorySlot();
        }

        for (int i = 0;i < inventory.Count; i++)
        {
            slots[i].DrawSlot(inventory[i]);
        }
    }

    void CreateInventorySlot()
    {
        GameObject newSlot = Instantiate(slotPrefab);
        newSlot.transform.SetParent(transform, false);

        InventorySlot newSlotComponent = newSlot.GetComponent<InventorySlot>();
        newSlotComponent.ClearSlot();

        slots.Add(newSlotComponent);
        Debug.Log("Create slot");
    }
}
