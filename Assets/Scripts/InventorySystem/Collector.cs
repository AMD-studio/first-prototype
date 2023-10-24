using Assets.Scripts.InventorySystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class Collector : MonoBehaviour
{
    private ICollectible ItemToPickup;
    private void OnTriggerEnter(Collider collision)
    {
        ICollectible collectible = collision.GetComponent<ICollectible>();
        if (collectible != null)
        {
            HUDController.Instance.ShowActionLabel("Підібрати Stick натисніть E");
            ItemToPickup = collectible;
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        HUDController.Instance.HideActionLabel();
        ItemToPickup = null;
    }

    private void Update()
    {
        if (ItemToPickup != null && Input.GetKeyDown(KeyCode.E))
        {
            ItemToPickup.Collect();
            HUDController.Instance.HideActionLabel();
            ItemToPickup = null;
        }
    }

}
