using UnityEngine;
using UnityEngine.UI;

public class InventorySlots : MonoBehaviour
{
    public Image icon; // Arrastra aquí la imagen hija del slot
    private ItemData item;

    public void AddItem(ItemData newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }
}
