using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class WeaponItem : InventoryItem
{
    [SerializeField] private WeaponType _weaponType;
    public WeaponType WeaponType { get { return _weaponType; } set { _weaponType = value; } }
    
    [SerializeField] private int _damage;
    public int Damage { get { return _damage; } set { _damage = value; } }

    [SerializeField] private int _durability;
    public int Durability { get { return _durability; } set { _durability = value; } }

    [SerializeField] private int _range;
    public int Range { get { return _range; } set { _range = value; } }

    [SerializeField] private int _attackSpeed;
    public int AttackSpeed { get { return _attackSpeed; } set { _attackSpeed = value; } }

    [SerializeField] private int _critChance;
    public int CritChance { get { return _critChance; } set { _critChance = value; } }

    [SerializeField] private int _critMultiplier;
    public int CritMultiplier { get { return _critMultiplier; } set { _critMultiplier = value; } }

    [SerializeField] private float _attackCooldown;
    public float AttackCooldown { get { return _attackCooldown; } set { _attackCooldown = value; } }

    public override void Use()
    {
        Debug.Log("Using weapon");
    }
    public override void Equip()
    {
        Debug.Log("Impossible to equip weapon");
    }
    public override void Unequip()
    {
        Debug.Log("Impossible to unequip weapon");
    }

    public override void Select()
    {
        Debug.Log("Selecting weapon");
    }
    public override void Deselect()
    {
        Debug.Log("Deselecting weapon");
    }

}
