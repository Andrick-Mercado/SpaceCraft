using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Image imageIcon;
    [SerializeField] private TextMeshProUGUI textItemName;
    [SerializeField] private GameObject counterGameObject;
    [SerializeField] private TextMeshProUGUI counterText;

    public void Set(InventoryItem item)
    {
        imageIcon.sprite = item.data.icon;
        textItemName.text = item.data.displayName;

        if (item.stackSize <= 1)
        {
            counterGameObject.SetActive(false);
            return;
        }

        counterText.text = item.stackSize.ToString();
    }
}
