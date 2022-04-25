using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;

    [SerializeField] private List<InventoryItem> inventoryList;
    
    public List<InventoryItem> inventory
    {
        get { return inventoryList;}
        private set { inventoryList = value; }
    }
    private Dictionary<InventoryItemData, InventoryItem> m_itemDictionary;
    
    public delegate void UpdateInventoryEvent();
    public event UpdateInventoryEvent OnInventoryChangedEvent;
    private void Awake()
    {
        Instance = this;
        inventory = new List<InventoryItem>();
        m_itemDictionary = new Dictionary<InventoryItemData, InventoryItem>();
    }

    public InventoryItem Get(InventoryItemData referenceData)
    {
        if (m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            return value;
        }
        return null;
    }

    public void Add(InventoryItemData referenceData)
    {
        if (m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            value.AddToStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(referenceData);
            inventory.Add(newItem);
            m_itemDictionary.Add(referenceData, newItem);
        }
        OnInventoryChangedEvent?.Invoke();
    }

    public void Remove(InventoryItemData referenceData)
    {
        if (m_itemDictionary.TryGetValue(referenceData, out InventoryItem value))
        {
            value.RemoveFromStack();

            if (value.stackSize == 0)
            {
                inventory.Remove(value);
                m_itemDictionary.Remove(referenceData);
            }
        }
        OnInventoryChangedEvent?.Invoke();
    }
}
