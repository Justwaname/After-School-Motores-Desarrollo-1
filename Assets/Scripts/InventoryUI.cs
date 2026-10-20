using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent; // El contenedor (Grid Layout Group) donde van los slots
    public GameObject slotPrefab; // El prefab visual de cada casilla del inventario

    private Inventory inventory;
    private InventorySlots[] slots;

    void Start()
    {
        inventory = Inventory.Instance;
        inventory.onItemChangedCallback += UpdateUI;

        // Obtiene todos los slots hijos del contenedor
        slots = itemsParent.GetComponentsInChildren<InventorySlots>();
        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < inventory.Items.Count)
            {
                slots[i].AddItem(inventory.Items[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }
}
