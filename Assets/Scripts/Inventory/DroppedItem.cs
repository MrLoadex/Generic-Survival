using UnityEngine;
using System.Collections;

public class DroppedItem : MonoBehaviour
{
    [SerializeField] private InventoryItem item;
    public InventoryItem Item { get { return item; } set { item = value; } }
    private bool canPickup = true;

    private void Start() {
        item = item.Clone();
        //aplicar rotacion aleatoria
        GetComponent<Transform>().rotation = Quaternion.Euler(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180));
    }

    void Pickup()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }

    IEnumerator PickupCoroutine()
    {
        yield return new WaitForSeconds(3);
        canPickup = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!canPickup) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            canPickup = false;
            StartCoroutine(PickupCoroutine());
            InventoryEvents.CollisionItem?.Invoke(item);
        }
    }

    #region Events

    private void OnEnable()
    {
        InventoryEvents.PickupItem += replyPickupItem;
    }

    private void OnDisable()
    {
        InventoryEvents.PickupItem -= replyPickupItem;
    }

    private void OnDestroy()
    {
        InventoryEvents.PickupItem -= replyPickupItem;
    }

    private void replyPickupItem(InventoryItem item)
    {
        if (item.ID == this.item.ID)
        {
            Pickup();
        }
    }

    #endregion
}