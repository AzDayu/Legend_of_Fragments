using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Item itemData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory.instance.AddItem(itemData);

            Debug.Log(itemData._itemName + "을(를) 획득하여 인벤토리에 넣었습니다!");

            Destroy(gameObject);
        }
    }
}
