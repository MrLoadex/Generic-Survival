using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private TextMeshProUGUI weightText;
    [SerializeField] private InventorySlot slotPrefab;
    [SerializeField] private Transform content;
    [SerializeField] private Transform inventoryPanel;
    [SerializeField] private RectTransform dropArea;
    [SerializeField] private RectTransform beltArea;

    [Header("Description Panel")]
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;

    private List<InventoryItem> items = new List<InventoryItem>();
    private List<InventorySlot> slots = new List<InventorySlot>();

    private PlayerController playerController = new PlayerFootController();

    private bool isCursorInDropArea = false;
    private bool isCursorInBeltArea = false;
    private bool canUse = true;
    private void Start() 
    {
        descriptionPanel.SetActive(false);
    }

    void Update()
    {
        if (!canUse) return;
        if (playerController.getAttackInput()) // Si el botón del mouse está presionado...
        {
            isCursorInDropArea = RectTransformUtility.RectangleContainsScreenPoint(
                dropArea, Input.mousePosition);

            isCursorInBeltArea = RectTransformUtility.RectangleContainsScreenPoint(
                beltArea, Input.mousePosition);

        }
    }

    InventorySlot CreateSlot(InventoryItem item)
    {
        if (item.Icon == null)
        {
            Debug.Log("Item without icon: " + item.Name);
        }
        InventorySlot slot = Instantiate(slotPrefab, transform);
        slot.transform.SetParent(content);
        slot.UpdateSlot(item);
        slots.Add(slot);
        return slot;
    }

    void ShowDescriptionPanel(InventoryItem item)
    {
        itemImage.sprite = item.Icon;
        itemName.text = item.Name;
        itemDescription.text = item.Description;
        descriptionPanel.SetActive(true);
    }

    public void HideDescriptionPanel()
    {
        descriptionPanel.SetActive(false);
    }

    void DrawSlots()
    {
        slots.Clear();
        foreach (Transform child in content) // borrar por si acaso
        {
            Destroy(child.gameObject);
        }
        foreach (var item in items)
        {
            CreateSlot(item);   
        }
    }

    #region Events

    private void OnEnable() 
    {
        InventoryEvents.ChangedWeight += replyChangedWeight;
        InventoryEvents.inventoryEdit += replyInventoryEdit;
        InventoryEvents.ShowDescription += replyShowDescription;
        InventoryEvents.HideDescription += replyHideDescription;
        InventoryEvents.DragItem += replyDragItem;
        InventoryEvents.DragEndItem += replyDragEndItem;
        PlayerEvents.Death += replyPlayerDeath;
        PlayerEvents.Revive += replyPlayerRevive;
    }

    private void OnDisable() 
    {
        InventoryEvents.ChangedWeight -= replyChangedWeight;
        InventoryEvents.inventoryEdit -= replyInventoryEdit;
        InventoryEvents.ShowDescription -= replyShowDescription;
        InventoryEvents.HideDescription -= replyHideDescription;
        InventoryEvents.DragItem -= replyDragItem;
        InventoryEvents.DragEndItem -= replyDragEndItem;
        PlayerEvents.Death -= replyPlayerDeath;
        PlayerEvents.Revive -= replyPlayerRevive;
    }
    
    private void OnDestroy() 
    {
        InventoryEvents.ChangedWeight -= replyChangedWeight;
        InventoryEvents.inventoryEdit -= replyInventoryEdit;
        InventoryEvents.ShowDescription -= replyShowDescription;
        InventoryEvents.HideDescription -= replyHideDescription;
        InventoryEvents.DragItem -= replyDragItem;
        InventoryEvents.DragEndItem -= replyDragEndItem;
        PlayerEvents.Death -= replyPlayerDeath;
        PlayerEvents.Revive -= replyPlayerRevive;
    }

    private void replyChangedWeight(int currentWeight, int maxWeight)
    {
        weightText.text = currentWeight.ToString() + "/" + maxWeight.ToString() + "kg";
    }

    private void replyInventoryEdit(List<InventoryItem> items)
    {
        this.items = items;
        DrawSlots();
    }
    
    private void replyShowDescription(InventoryItem item)
    {
        ShowDescriptionPanel(item);
    }

    private void replyHideDescription()
    {
        HideDescriptionPanel();
    }
    
    private void replyDragItem(InventorySlot slot)
    {
        slot.transform.SetParent(inventoryPanel);
        slot.transform.position = Input.mousePosition;
    }
    
    private void replyDragEndItem(InventorySlot slot)
    {
        slot.transform.SetParent(content);
        if (isCursorInDropArea)
        {
            if (playerController.getRunInput())
            {
                InventoryEvents.DropAllSameItems?.Invoke(slot.Item);
            }
            else
            {
                InventoryEvents.DropItem?.Invoke(slot.Item);
            }
        }
        else if (isCursorInBeltArea)
        {
            
            InventoryEvents.SendItemToBelt?.Invoke(slot.Item);
        }
        DrawSlots();
    }
        
    private void replyPlayerDeath()
    {
        canUse = false;
    }

    private void replyPlayerRevive()
    {
        canUse = true;
    }
    
    #endregion
}