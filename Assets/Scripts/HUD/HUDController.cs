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

    private void Awake()
    {
        Instance = this;
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
}
