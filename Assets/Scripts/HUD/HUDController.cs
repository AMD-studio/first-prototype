using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance;
    public GameObject ActionLabel;
    public GameObject Inventory;
    public GameObject MainCamera;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }
    public void ShowActionLabel(string action)
    {
        ActionLabel.SetActive(true);
        ActionLabel.GetComponentInChildren<TextMeshProUGUI>().text = action;
    }
    public void HideActionLabel()
    {
        ActionLabel.SetActive(false);
        ActionLabel.GetComponentInChildren<TextMeshProUGUI>().text = "";
    }

    public void ToggleInventory()
    {
        var cameraController = MainCamera.GetComponent<CameraController>();
        var isInventoryOpen = Inventory.active;
        Inventory.SetActive(!isInventoryOpen);
        cameraController.enabled = isInventoryOpen;
        Time.timeScale = isInventoryOpen ? 1 : 0;
    }
}
