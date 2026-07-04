using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("아이템 기본 정보")]
    public string _itemName;
    public Sprite _itemIcon;
    public int _maxStackSize;
    public float healAmount = 30f;

    [TextArea(3, 5)]
    public string _description;

    public virtual void Use()
    {
        PlayerStats playerStats = Inventory.instance.GetComponent<PlayerStats>();

        if (playerStats != null)
        {
            playerStats.Heal(healAmount);
            Debug.Log(_itemName + "을(를) 사용하여 체력을 " + healAmount + "만큼 회복했습니다!");
        }
    }
}
