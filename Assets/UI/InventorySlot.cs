using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI amountText;

    private ItemStack currentStack;

    public void AddItem(ItemStack newStack)
    {
        currentStack = newStack;

        icon.sprite = currentStack.item._itemIcon;
        icon.gameObject.SetActive(true);

        if (currentStack.amount > 1)
        {
            amountText.text = currentStack.amount.ToString();
            amountText.gameObject.SetActive(true);
        }
        else
        {
            amountText.gameObject.SetActive(false);
        }
    }

    public void UseItem()
    {
        if (currentStack != null)
        {
            currentStack.item.Use();
            Inventory.instance.RemoveItem(currentStack);
        }
    }
}