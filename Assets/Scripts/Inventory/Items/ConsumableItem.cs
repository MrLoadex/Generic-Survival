using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class ConsumableItem : InventoryItem
{
    public ItemActionType consumableType;
    [SerializeField] private int value;
    
    public override void Use()
    {
        switch (consumableType)
        {
            case ItemActionType.Heal:
                PlayerEvents.Heal(value);
                break;
            case ItemActionType.Drink:
                PlayerEvents.Drink(value);
                break;
            case ItemActionType.Eat:
                PlayerEvents.Eat(value);
                break;
            case ItemActionType.Rest:
                PlayerEvents.Rest(value);
                break;
        }
        InventoryEvents.ConsumeItem?.Invoke(this);
    }

    public override void Equip()
    {
        Debug.Log("Impossible to equip consumable");
    }

    public override void Unequip()
    {
        Debug.Log("Impossible to unequip consumable");
    }

    public override void Select()
    {
        Debug.Log("Selecting consumable");
    }

    public override void Deselect()
    {
        Debug.Log("Deselecting consumable");
    }
}
