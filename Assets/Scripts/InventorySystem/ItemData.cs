using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Inventory Item")]
public class ItemData : ScriptableObject
{
    public string displayName;
    public Sprite icon;
}
