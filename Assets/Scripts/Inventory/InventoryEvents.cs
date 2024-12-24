using UnityEngine;
using System;
using System.Collections.Generic;
public class InventoryEvents : MonoBehaviour
{
    public static Action<InventoryItem> CollisionItem;
    public static Action<InventoryItem> PickupItem;
    public static Action<InventoryItem> DropItem;
    public static Action<InventoryItem> DropAllSameItems;
    public static Action<InventoryItem> UseItem;
    public static Action<InventoryItem> EquipItem;
    public static Action<InventoryItem> UnequipItem;
    public static Action<int, int> ChangedWeight; // int currentWeight, int maxWeight
    public static Action<bool> OpenCloseInventory; //bool isOpen
    public static Action<List<InventoryItem>> inventoryEdit;
    public static Action<InventoryItem> ShowDescription;
    public static Action HideDescription;
    public static Action<InventorySlot> DragItem;
    public static Action<InventorySlot> DragEndItem;
    public static Action<InventoryItem> ConsumeItem;
    public static Action<InventoryItem> SendItemToBelt;
    public static Action<InventoryItem> SendItemToInventory;
}