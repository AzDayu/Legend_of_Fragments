using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private PlayerInputHandler inputHandler;

    public Action OnInventoryChanged;

    public List<Item> items = new List<Item>();
    private bool isInventoryOpen = false;

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
        }
        else
        {
            UIManager.Instance.CloseUI(UIType.Inventory);
        }
    }

    public void AddItem(Item _item)
    {
        items.Add(_item);
        Debug.Log(_item._itemName + "이(가) 인벤토리 리스트에 저장되었습니다!");

        OnInventoryChanged?.Invoke();
    }

    public void RemoveItem(Item _item)
    {
        items.Remove(_item);
        OnInventoryChanged?.Invoke();
    }
}