using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemStack
{
    public Item item;
    public int amount;

    public ItemStack(Item _item, int _amount)
    {
        item = _item;
        amount = _amount;
    }
}

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public List<ItemStack> items = new List<ItemStack>();
    public Action OnInventoryChanged;
    private bool isInventoryOpen = false;
    private PlayerInputHandler inputHandler;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        inputHandler = GetComponent<PlayerInputHandler>();

    }

    private void Update()
    {
        if (inputHandler.ToggleInventory)
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;

        if (isInventoryOpen)
        {
            UIManager.Instance.OpenUI(UIRootType.Popup, UIType.Inventory);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            UIManager.Instance.CloseUI(UIType.Inventory);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void AddItem(Item _item)
    {
        foreach (ItemStack stack in items)
        {
            if (stack.item == _item && stack.amount < _item._maxStackSize)
            {
                stack.amount++;
                OnInventoryChanged?.Invoke();
                return;
            }
        }

        items.Add(new ItemStack(_item, 1));
        OnInventoryChanged?.Invoke();
    }

    public void RemoveItem(ItemStack _stack)
    {
        _stack.amount--;

        if (_stack.amount <= 0)
        {
            items.Remove(_stack);
        }

        OnInventoryChanged?.Invoke();
    }
}