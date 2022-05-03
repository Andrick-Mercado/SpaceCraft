using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private GameObject itemPrefab;

    private void Start()
    {
        InventorySystem.Instance.OnInventoryChangedEvent += OnUpdateInventory;
    }

    private void OnUpdateInventory()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }

        DrawInventory();
    }

    private void DrawInventory()
    {
        foreach (InventoryItem item in InventorySystem.Instance.inventory)
        {
            AddInventorySlot(item);
        }
    }

    private void AddInventorySlot(InventoryItem item)
    {
        GameObject obj = Instantiate(itemPrefab);
        obj.transform.SetParent(transform, false);

        InventoryUI slot = obj.GetComponent<InventoryUI>();
        slot.Set(item);
    }
}
