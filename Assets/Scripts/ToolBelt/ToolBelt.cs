using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ToolBelt : MonoBehaviour
{
    private PlayerController playerController = new PlayerFootController();

    [Header("Configuración del Cinturón")]
    [SerializeField] private List<InventoryItem> items;
    [SerializeField] private BeltSlot beltSlotPrefab;
    [SerializeField] private int actualSlots = 3;
    [SerializeField] private int maxSlots = 8;
    [SerializeField] private int minSlots = 3;

    [Header("Áreas de la Interfaz")]
    [SerializeField] private RectTransform dropArea;
    [SerializeField] private RectTransform inventoryArea;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform inventoryPanel;

    [Header("Estado del Cinturón")]
    private List<BeltSlot> slots;
    private bool isCursorInDropArea = false;
    private bool isCursorInInventoryArea = false;
    private int selectedSlotIndex = 0;
    private bool isInventoryOpen = false;

    private void Start()
    {
        List<InventoryItem> tempItems = new List<InventoryItem>();
        //copiar items
        for (int i = 0; i < Mathf.Clamp(actualSlots, minSlots, maxSlots); i++)
        {
            if (i < items.Count)
            {
                tempItems.Add(items[i]);
            }
        }
        
        items = new List<InventoryItem>();
        //clonar items
        for (int i = 0; i < tempItems.Count; i++)
        {
            items.Add(tempItems[i].Clone());
        }

        // crear slots
        slots = new List<BeltSlot>();
        for (int i = 0; i < actualSlots; i++)
        {
            BeltSlot beltSlot = Instantiate(beltSlotPrefab, content);
            if (i < items.Count)
            {
                beltSlot.Item = items[i];
            }
            else
            {
                beltSlot.Item = null;
            }
            slots.Add(beltSlot);
            beltSlot.ItemAddedEvent += ReplyItemAdded;
            beltSlot.DragItemEvent += ReplyDragItem;
            beltSlot.DragEndItemEvent += ReplyDragEndItem;
            beltSlot.CursorInSlotEvent += ReplyCursorInSlot;
        }
        slots[selectedSlotIndex].SelectSlot();
        DrawSlots();
    }

    private void Update()
    {
        if (isInventoryOpen)
        {
            if (playerController.getAttackInput()) // Si el botón del mouse está presionado || CORREGIR Y USAR EL CONTROLADOR
            {
                isCursorInDropArea = RectTransformUtility.RectangleContainsScreenPoint(
                    dropArea, Input.mousePosition);

                isCursorInInventoryArea = RectTransformUtility.RectangleContainsScreenPoint(
                    inventoryArea, Input.mousePosition);
            }
        }
        else
        {
            //Check click yo use the tool
            if (playerController.getAttackInput())
            {
                if (slots[selectedSlotIndex].Item != null)
                {
                    slots[selectedSlotIndex].UseItem();
                }
            }
            //Check if the player is scrolling the mouse wheel
            if (Input.GetAxis("Mouse ScrollWheel") < 0) // Si está moviendo la ruedita del mouse
            {
                slots[selectedSlotIndex].DeselectSlot();
                selectedSlotIndex ++;
                if (selectedSlotIndex >= actualSlots)
                {
                    selectedSlotIndex = 0;
                }
                slots[selectedSlotIndex].SelectSlot();

            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0) // Si está moviendo la ruedita del mouse
            {
                slots[selectedSlotIndex].DeselectSlot();
                selectedSlotIndex = (selectedSlotIndex - 1 + actualSlots) % actualSlots;
                if (selectedSlotIndex < 0)
                {
                selectedSlotIndex = actualSlots - 1;
            }
            slots[selectedSlotIndex].SelectSlot();

        }
        }
    }

    public void DrawSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (i < items.Count && items[i] != null)
            {
                slots[i].Item = items[i];
            }
            else
            {
                slots[i].Item = null;
            }
        }
    }

    public void AddItem(InventoryItem item, int index)
    {
        if (!(index >= 0 && index < actualSlots)) return; // Si el índice no es válido, no se añade el item

        if (slots[index].Item != null) // Si el slot ya tiene un item
        {
            if (slots[index].Item.name == item.name && slots[index].Item.IsStackable) // Si el item es stackable y el nombre es el mismo
            {
                slots[index].Item.Count++;
            }
            else
            {
                InventoryEvents.SendItemToInventory?.Invoke(slots[index].Item);
                items[index] = item;
            }
        }
        else
        {
            if (index < items.Count)
            {
                items[index] = item;
            }
            else
            {
                items.Add(item);
            }
        }

        DrawSlots();
    }

    public void RemoveItem(InventoryItem item)
    {
        foreach (BeltSlot beltSlot in slots)
        {
            if (beltSlot.Item == item)
            {
                if (beltSlot.Item.Count > 1)
                {
                    beltSlot.Item.Count--;
                }
                else
                {
                    beltSlot.Item = null;
                }
            }
        }
        DrawSlots();
    }

    public void AddSlot()
    {
        if (actualSlots >= maxSlots) return;
        //eliminar los hijos del content
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        actualSlots++;
        slots.Clear();
        for (int i = 0; i < actualSlots; i++)
        {
            BeltSlot beltSlot = Instantiate(beltSlotPrefab, content);
            slots.Add(beltSlot);
            beltSlot.ItemAddedEvent += ReplyItemAdded;
            beltSlot.DragItemEvent += ReplyDragItem;
            beltSlot.DragEndItemEvent += ReplyDragEndItem;
            beltSlot.CursorInSlotEvent += ReplyCursorInSlot;
        }
        DrawSlots();
    }

    #region EVENTS
    private void OnEnable()
    {
        if (slots != null)
        {
            foreach (BeltSlot beltSlot in slots)
            {
                // BELT SLOT EVENTS
                beltSlot.ItemAddedEvent += ReplyItemAdded;
                beltSlot.DragItemEvent += ReplyDragItem;
                beltSlot.DragEndItemEvent += ReplyDragEndItem;
                beltSlot.CursorInSlotEvent += ReplyCursorInSlot;
            }
        }
        // INVENTORY EVENTS
        InventoryEvents.OpenCloseInventory += ReplyOpenCloseInventory;
        InventoryEvents.ConsumeItem += ReplyConsumeItem;
        InventoryEvents.SendItemToBelt += ReplySendItemToBelt;
    }

    private void OnDisable()
    {
        foreach (BeltSlot beltSlot in slots)
        {
            // BELT SLOT EVENTS
            beltSlot.ItemAddedEvent -= ReplyItemAdded;
            beltSlot.DragItemEvent -= ReplyDragItem;
            beltSlot.DragEndItemEvent -= ReplyDragEndItem;
            beltSlot.CursorInSlotEvent -= ReplyCursorInSlot;
        }
        // INVENTORY EVENTS
        InventoryEvents.OpenCloseInventory -= ReplyOpenCloseInventory;
        InventoryEvents.ConsumeItem -= ReplyConsumeItem;
        InventoryEvents.SendItemToBelt -= ReplySendItemToBelt;
    }

    private void OnDestroy()
    {
        foreach (BeltSlot beltSlot in slots)
        {
            // BELT SLOT EVENTS
            beltSlot.ItemAddedEvent -= ReplyItemAdded;
            beltSlot.DragItemEvent -= ReplyDragItem;
            beltSlot.DragEndItemEvent -= ReplyDragEndItem;
            beltSlot.CursorInSlotEvent -= ReplyCursorInSlot;
        }
        // INVENTORY EVENTS
        InventoryEvents.OpenCloseInventory -= ReplyOpenCloseInventory;
        InventoryEvents.ConsumeItem -= ReplyConsumeItem;
        InventoryEvents.SendItemToBelt -= ReplySendItemToBelt;
    }

    private void ReplyItemAdded(InventoryItem item)
    {
        AddItem(item, slots.Count);
    }

    private void ReplyDragItem(BeltSlot slot)
    {
        //pegar la imagen del item en el cursor
        slot.Icon.transform.SetParent(inventoryPanel);
        slot.Icon.transform.position = Input.mousePosition;
    }
   
    private void ReplyDragEndItem(BeltSlot slot)
    {
        print("DragEndItem");
        slot.Icon.transform.SetParent(slot.transform);
        slot.Icon.transform.position = slot.transform.position;
        
        if (isCursorInInventoryArea)
        {
            print("SendItemToInventory");
            InventoryEvents.SendItemToInventory?.Invoke(slot.Item);
            items[selectedSlotIndex] = null;
        }
        else if (isCursorInDropArea)
        {
            print("DropItem");
            InventoryEvents.DropItem?.Invoke(slot.Item);
            items[selectedSlotIndex] = null;
        }

        DrawSlots();
    }

    private void ReplyOpenCloseInventory(bool isOpen)
    {
        isInventoryOpen = isOpen;
    }
    
    private void ReplyConsumeItem(InventoryItem item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null && items[i].ID == item.ID)
            {
                if (items[i].Count > 1)
                {
                    items[i].Count--;
                }
                else
                {
                    items[i] = null;
                }
                break;
            }
        }
        //redibujar slots
        DrawSlots();
    }
    
    private void ReplySendItemToBelt(InventoryItem item)
    {
        AddItem(item, selectedSlotIndex);
    }

    private void ReplyCursorInSlot(BeltSlot beltSlot)
    {
        if (!isInventoryOpen) return;

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == beltSlot)
            {
                selectedSlotIndex = i;
                slots[i].SelectSlot();
            }
            else
            {
                slots[i].DeselectSlot();
            }
        }
    }
    #endregion
}
