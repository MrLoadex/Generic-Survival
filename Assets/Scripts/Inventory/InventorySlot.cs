using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private InventoryItem item;
    public InventoryItem Item { get { return item; } set { item = value; } }

    [SerializeField] private Image icon;
    public Image Icon { get { return icon; } set { icon = value; } }

    [SerializeField] private TextMeshProUGUI countTMP;
    public TextMeshProUGUI CountTMP { get { return countTMP; } set { countTMP = value; } }

    [SerializeField] private GameObject countBackground;
    public GameObject CountBackground { get { return countBackground; } set { countBackground = value; } }

    private bool isWaitForSecondClock = false;

    [SerializeField] private InvItemOptionsCard invItemOptionsCardPrefab;
    
    public void UpdateSlot(InventoryItem item)
    {
        this.item = item;
        icon.sprite = item.Icon;
        if (item.Count > 1)
        {
            countBackground.SetActive(true);
            countTMP.text = item.Count.ToString();
        }
        else
        {
            countBackground.SetActive(false);
        }
    }

    public void SelectSlot()
    {
        countBackground.SetActive(true);
    }

    public void DeselectSlot()
    {
        countBackground.SetActive(false);
    }

    public void UseItem()
    {
        item.Use();
    }   

    public void OnDrag()
    {
        InventoryEvents.DragItem?.Invoke(this);
    }

    public void OnEndDrag()
    {
        InventoryEvents.DragEndItem?.Invoke(this);
    }

    public void DropItem(Transform dropPoint)
    {
        item.Drop(dropPoint);
    }

    public void EquipItem()
    {
        item.Equip();
    }

    public void UnequipItem()
    {
        item.Unequip();
    }

    public void ShowDescription()
    {
        InventoryEvents.ShowDescription?.Invoke(item);
    }

    public void HideDescription()
    {
        InventoryEvents.HideDescription?.Invoke();
    }

    public void OnClick()
    {
        if (isWaitForSecondClock)
        {
            // Llevar al belt si es posible
            Debug.Log("Llevar al belt");
            isWaitForSecondClock = false;
        }
        else
        {
            StartCoroutine(WaitForClick());
        }
    }

    IEnumerator WaitForClick()
    {
        isWaitForSecondClock = true;
        yield return new WaitForSeconds(0.2f);
        if (isWaitForSecondClock)
        {
            showOptions();
        }
        isWaitForSecondClock = false;
    }
    
    void showOptions()
    {
        Instantiate(invItemOptionsCardPrefab, transform.position, transform.rotation, transform).SetItem(item);
    }
}

