using Assets.Scripts.InventorySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : MonoBehaviour, ICollectible    
{
    public static event HandleStickCollected OnStickCollected;
    public delegate void HandleStickCollected(ItemData item);
    public ItemData stickData;

    public void Collect()
    {
        Destroy(gameObject);
        //OnStickCollected?.Invoke(stickData);
    }

    
}
