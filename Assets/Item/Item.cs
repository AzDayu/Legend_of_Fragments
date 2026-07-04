using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("아이템 기본 정보")]
    public string _itemName;
    public Sprite _itemIcon;
    public int _maxStackSize;

    [TextArea(3, 5)]
    public string _description;

    public virtual void Use()
    {
        Debug.Log(_itemName + "을(를) 사용했습니다.");
    }
}
