using UnityEngine;

public class KeyItem : InteractableObject
{
    [Header("Información del objeto")]
    [SerializeField] private string itemID = "Pieza1";
    [SerializeField] private string itemName = "Pieza";

    [Header("Progresión")]
    [SerializeField] private bool countsTowardsEscape = true; // Desmarca esto si es una llave de puerta que no debe sumar al contador

    public ItemData itemData;

    // Configurar el objeto desde ItemSpawner
    public void SetItemData(string newID, string newName, bool counts = true)
    {
        itemID = newID;
        itemName = newName;
        countsTowardsEscape = counts;
        gameObject.name = newName;
    }

    public override void Interact()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("No existe un Inventory en la escena.");
            return;
        }

        bool added = Inventory.Instance.AddItem(itemData);

        if (!added)
        {
            Debug.Log("Este objeto ya está en el inventario.");
            return;
        }

        if (HorrorSFXManager.Instance != null)
        {
            HorrorSFXManager.Instance.PlaySFX(
                HorrorSFXEvent.ItemPickup,
                transform.position
            );
        }

        Debug.Log("Objeto recogido: " + itemName);

        // Solo sumará al GameManager si está marcado como objeto de escape
        if (countsTowardsEscape && GameManager.Instance != null)
        {
            GameManager.Instance.ItemCollected();
        }

        Destroy(gameObject);
    }

    public override string GetInteractText()
    {
        return itemName + " [E] para recoger";
    }
}