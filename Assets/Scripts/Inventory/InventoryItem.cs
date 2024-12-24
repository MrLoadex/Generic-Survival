using UnityEngine;


public abstract class InventoryItem : ScriptableObject
{
    [Header("Item Information")]
    [SerializeField] private string id;
    public string ID { get { return id; } set { id = value; } }

    [SerializeField] protected string _name;
    public string Name { get { return _name; } set { _name = value; } }

    [SerializeField] protected string craftingDescription;
    public string CraftingDescription { get { return craftingDescription; } set { craftingDescription = value; } }

    [SerializeField][TextArea(3, 10)] protected string description;
    public string Description { get { return description; } set { description = value; } }

    [SerializeField] protected Sprite icon;
    public Sprite Icon { get { return icon; } set { icon = value; } }

    [SerializeField] protected int weight;
    public int Weight { get { return weight; } set { weight = value; } }

    [SerializeField] protected ItemType type;
    public ItemType Type { get { return type; } set { type = value; } }

    [SerializeField] protected bool isStackable;
    public bool IsStackable { get { return isStackable; } set { isStackable = value; } }

    [SerializeField] protected int count = 1;
    public int Count { get { return count; } set { count = value; } }

    [SerializeField] protected int stackSize = 99;
    public int StackSize { get { return stackSize; } set { stackSize = value; } }

    [SerializeField] protected DroppedItem droppedItem;
    public DroppedItem DroppedItem { get { return droppedItem; } set { droppedItem = value; } }

    [SerializeField] protected GameObject selectedGO;
    public GameObject SelectedGO { get { return selectedGO; } set { selectedGO = value; } }

    [SerializeField] private bool canUse;
    public bool CanUse { get { return canUse; } protected set { canUse = value; } }
    [SerializeField] private bool canEquip;
    public bool CanEquip { get { return canEquip; } protected set { canEquip = value; } }

    public abstract void Use();
    public abstract void Equip();
    public abstract void Unequip();
    public abstract void Select();// Instanciar el objeto seleccionado en la mano del jugador
    public abstract void Deselect();// Destruir el objeto seleccionado

    
    public void Drop(Transform dropPoint)
    {
        DroppedItem droppedItemInstance = Instantiate(droppedItem, dropPoint.position, dropPoint.rotation);
        droppedItemInstance.Item = this;
        droppedItemInstance.Item.Count = count;
    }
    public InventoryItem Clone()
    {
        InventoryItem item = Instantiate(this);
        item.ID = id + Random.Range(0, 1000000);
        return item;
    }

}