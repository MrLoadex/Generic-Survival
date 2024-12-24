using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvItemOptionsCard : MonoBehaviour
{
    [SerializeField] private GameObject UseButton;
    [SerializeField] private GameObject EquipButton;
    [SerializeField] private GameObject DropButton;

    [SerializeField] private float autoDestroyTime = 1f;

    private InventoryItem item;
    public InventoryItem Item { get { return item; } set { SetItem(value); } }

    public void SetItem(InventoryItem item)
    {
        this.item = item;
        UseButton.SetActive(item.CanUse);
        EquipButton.SetActive(item.CanEquip);
        DropButton.SetActive(true);
        StartCoroutine(AutoDestroy());
    }

    public void UseItem()
    {
        Item.Use();
    }

    public void EquipItem()
    {
        Item.Equip();
    }

    public void DropItem()
    {
        InventoryEvents.DropItem?.Invoke(item);
    }

    public void OnEnterCursor()
    {
        StopAllCoroutines();
    }

    public void OnExitCursor()
    {
        Destroy(gameObject);
    }

    IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(autoDestroyTime);
        Destroy(gameObject);
    }
}
