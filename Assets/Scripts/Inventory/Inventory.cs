using System.Collections.Generic;
using UnityEngine;

public class Inventory : Singleton<Inventory> // CLASE INCOMPLETA
{
    PlayerController playerController = new PlayerFootController();
    bool _isOpen = false;
    int actualWeight = 0;
    int maxWeight = 50;

    [SerializeField] private List<InventoryItem> items;
    public List<InventoryItem> Items { get { return items; } set { items = value; } }

    [SerializeField] private Transform dropPoint;

    void Start() // INCOMPLETO
    {
        playerController = new PlayerFootController();
        
        // TEMPORAL PARA TESTING
        if (items == null) {
            items = new List<InventoryItem>();
        }
        else 
        {
            for (int i = 0; i < items.Count; i++) 
            {
                items[i] = items[i].Clone();
            }
        }
        //Implementar luego:
        /*
        TODO: Cargar la info desde GameManager
        Cargar los items del inventario desde GameManager
        calcular peso actual y peso maximo
        */
        // Calcular peso actual y peso maximo
        calculateWeight();
        InventoryEvents.inventoryEdit?.Invoke(items);

    }

    private void Update()
    {
        if (playerController.getOpenCloseInventoryInput())
        {
            OpenCloseInventory();
        }
    }

    void AddItem(InventoryItem item)
    {
        // Si el item es stackable y ya hay uno igual, se suma el count al item
        if (item.IsStackable)
        {
            List<InventoryItem> sameItems = CheckStock(item);
            if (sameItems.Count > 0)
            {
                foreach (var sameItem in sameItems)
                {
                    if (sameItem.Count + item.Count <= sameItem.StackSize)
                    {
                        sameItem.Count += item.Count;
                        calculateWeight();
                        InventoryEvents.PickupItem?.Invoke(item);
                        InventoryEvents.inventoryEdit?.Invoke(items);
                        return;
                    }
                }
                //Si no hay ningun item igual se agrega al inventario
                this.items.Add(item);
            }
            else
            {
                //Si el item no es stackable, se agrega al inventario
                this.items.Add(item);
            }
        }
        else
        {
            this.items.Add(item);
        }
        calculateWeight();
        InventoryEvents.PickupItem?.Invoke(item);
        InventoryEvents.inventoryEdit?.Invoke(items);
    }

    List<InventoryItem> CheckStock(InventoryItem item) // Devuelve una lista de slots con el mismo item
    {
        List<InventoryItem> sameItems = new List<InventoryItem>();
        foreach (var _item in this.items)
        {
            if (_item != null && _item.Name == item.Name)
            {
                sameItems.Add(_item);
            }
        }   
        return sameItems;
    }

    void calculateWeight()
    {
        actualWeight = 0;
        foreach (var item in items)
        {
            actualWeight += item.Weight * item.Count;
        }
        InventoryEvents.ChangedWeight?.Invoke(actualWeight, maxWeight);
        if (InventoryEvents.ChangedWeight == null) { Debug.Log("ChangedWeight is null"); } // Verificar que el evento ChangedWeight esté correctamente suscrito
    }

    public void OpenCloseInventory()
    {
        _isOpen = !_isOpen;
        InventoryEvents.OpenCloseInventory?.Invoke(_isOpen);
    }
    
    void DropItem(InventoryItem item)
    {
        foreach (var _item in items)
        {
            if (_item.ID == item.ID)
            {
                InventoryItem itemToDrop = _item.Clone();
                itemToDrop.Count = 1;
                itemToDrop.Drop(dropPoint);
                if (_item.Count > 1)
                {
                    _item.Count--;
                }
                else
                {
                    items.Remove(_item);
                }
                break;
            }
        }
        calculateWeight();
        InventoryEvents.inventoryEdit?.Invoke(items);
    }

    void DropAllItems()
    {
        foreach (var item in items)
        {
            item.Drop(dropPoint);
        }
        items.Clear();
        calculateWeight();
        InventoryEvents.inventoryEdit?.Invoke(items);
    }

    void DropAllSameItems(InventoryItem itemToDrop)
    {
        foreach (var _item in items)
        {
            if (_item.ID == itemToDrop.ID)
            {
                itemToDrop.Drop(dropPoint);
                items.Remove(_item);
                break;
            }
        }
        calculateWeight();
        InventoryEvents.inventoryEdit?.Invoke(items);
    }

    void RemoveItem(InventoryItem item)
    {
        items.Remove(item);
        calculateWeight();
        InventoryEvents.inventoryEdit?.Invoke(items);
    }

    #region EVENTS

    private void OnEnable()
    {
        InventoryEvents.CollisionItem += replyCollisionItem;
        InventoryEvents.DropItem += replyDropItem;
        InventoryEvents.DropAllSameItems += replyDropAllSameItems;
        PlayerEvents.Death += replyPlayerDeath;
        PlayerEvents.Revive += replyPlayerRevive;
        InventoryEvents.ConsumeItem += replyConsumeItem;
        InventoryEvents.SendItemToBelt += replySendItemToBelt;
        InventoryEvents.SendItemToInventory += replySendItemToInventory;
    }

    private void OnDisable()
    {
        InventoryEvents.CollisionItem -= replyCollisionItem;
        InventoryEvents.DropItem -= replyDropItem;
        InventoryEvents.DropAllSameItems -= replyDropAllSameItems;
        PlayerEvents.Death -= replyPlayerDeath;
        PlayerEvents.Revive -= replyPlayerRevive;
        InventoryEvents.ConsumeItem -= replyConsumeItem;
        InventoryEvents.SendItemToBelt -= replySendItemToBelt;
        InventoryEvents.SendItemToInventory -= replySendItemToInventory;
    }

    private void OnDestroy()
    {
        InventoryEvents.CollisionItem -= replyCollisionItem;
        InventoryEvents.DropItem -= replyDropItem;
        InventoryEvents.DropAllSameItems -= replyDropAllSameItems;
        PlayerEvents.Death -= replyPlayerDeath;
        PlayerEvents.Revive -= replyPlayerRevive;
        InventoryEvents.ConsumeItem -= replyConsumeItem;
        InventoryEvents.SendItemToBelt -= replySendItemToBelt;
        InventoryEvents.SendItemToInventory -= replySendItemToInventory;
    }

    private void replyCollisionItem(InventoryItem item)
    {
        AddItem(item);
    }
    
    private void replyDropItem(InventoryItem item)
    {
        DropItem(item);
    }

    private void replyDropAllSameItems(InventoryItem item)
    {
        DropAllSameItems(item);
    }

    private void replyPlayerDeath()
    {
        DropAllItems();
    }

    private void replyPlayerRevive()
    {
        DropAllItems();
    }

    private void replyConsumeItem(InventoryItem item)
    {
        if (items.Contains(item))
        {
            if (item.Count > 1)
            {
                item.Count--;
            }
            else
            {
                items.Remove(item);
            }
            calculateWeight();
            InventoryEvents.inventoryEdit?.Invoke(items);
        }
    }
    
    private void replySendItemToBelt(InventoryItem item)
    {
        RemoveItem(item);
    }

    private void replySendItemToInventory(InventoryItem item)
    {
        AddItem(item);
    }
    #endregion
}