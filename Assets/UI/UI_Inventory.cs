using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory : UIBase
{
    public Transform slotParent;
    public GameObject slotPrefab;

    private void OnEnable()
    {
        if (Inventory.instance != null)
        {
            Inventory.instance.OnInventoryChanged += RefreshUI;
            RefreshUI();
        }
    }

    private void OnDisable()
    {
        if (Inventory.instance != null)
        {
            Inventory.instance.OnInventoryChanged -= RefreshUI;
            RefreshUI();
        }
    }

    public void RefreshUI()
    {
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Item item in Inventory.instance.items)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotParent);
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
            if (slot != null) slot.AddItem(item);
        }
    }
}