using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    [SerializeField] private InventoryItemData itemData;
    [SerializeField] private int quantity;
    public InventoryItemData data
    {
        get => itemData;
        private set => itemData = value;
    }
    
    public int stackSize
    {
        get => quantity;
        private set => quantity = value;
    }

    public InventoryItem(InventoryItemData source)
    {
        data = source;
        AddToStack();
    }

    public void AddToStack()
    {
        stackSize++;
    }

    public void RemoveFromStack()
    {
        stackSize--;
    }
}
