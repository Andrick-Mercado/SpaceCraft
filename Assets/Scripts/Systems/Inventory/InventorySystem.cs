using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NaughtyAttributes;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance;

    [SerializeField] private List<InventoryItem> inventoryList;

    public List<InventoryItem> inventory
    {
        get { return inventoryList; }
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

    [Button("Clear Inventory")]
    public void RemoveAll()
    {
        if (!Application.isPlaying)
        {
            Debug.Log("Only run when playing!");
            return;
        }
        
        if (m_itemDictionary == null || m_itemDictionary.Count == 0)
        {
            Debug.Log("Inventory is already empty!");
            return;
        }
        InventoryItemData[] itemKeys = m_itemDictionary.Keys.ToArray();
        InventoryItem[] itemValues = m_itemDictionary.Values.ToArray();
        for (int i = 0; i < itemValues.Length; i++)
        {
            
            while ( itemValues[i].stackSize > 0 )
            {
                itemValues[i].RemoveFromStack();
        
                if (itemValues[i].stackSize == 0)
                {
                    inventory.Remove(itemValues[i]);
                    m_itemDictionary.Remove(itemKeys[i]);
                }
            }
            OnInventoryChangedEvent?.Invoke();
        }
        
        Debug.Log("Inventory is now empty!");
    }
    
}
