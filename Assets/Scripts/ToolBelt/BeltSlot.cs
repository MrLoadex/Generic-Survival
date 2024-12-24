using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BeltSlot : MonoBehaviour
{
    public Action<InventoryItem> ItemAddedEvent;
    public Action<BeltSlot> DragItemEvent;
    public Action<BeltSlot> DragEndItemEvent;
    public Action<BeltSlot> CursorInSlotEvent;

    [SerializeField] private InventoryItem item;
    [SerializeField] private GameObject selectedIcon;
    public InventoryItem Item { get { return item; } set { setItem(value); } }
    private RectTransform myRectTransform;

    [SerializeField] private Image icon;
    public Image Icon { get { return icon; } private set { icon = value; } }
    [SerializeField] private GameObject countBackground;
    [SerializeField] private TextMeshProUGUI countTMP;

    private void Awake()
    {
        myRectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        checkEnterCursor();
    }

    private void setItem(InventoryItem item)
    {
        if (item == null)
        {
            this.item = null;
            icon.gameObject.SetActive(false);
            countBackground.SetActive(false);
            countTMP.text = "";
        }
        else
        {
            this.item = item;
            icon.sprite = item.Icon;
            icon.gameObject.SetActive(true);
            countBackground.SetActive(item.Count > 1);
            countTMP.text = item.Count.ToString();
        }
    }

    public void SelectSlot()
    {
        selectedIcon.SetActive(true);
    }

    public void DeselectSlot()
    {
        selectedIcon.SetActive(false);
    }

    public void UseItem()
    {
        if (item != null)
        {
            item.Use();
        }
    }

    public void OnDragItem()
    {
        DragItemEvent?.Invoke(this);
    }

    public void OnEndDragItem()
    {
        DragEndItemEvent?.Invoke(this);
    }

    void checkEnterCursor()
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(
            myRectTransform, Input.mousePosition))
        {
            CursorInSlotEvent?.Invoke(this);
        }
    }
}

