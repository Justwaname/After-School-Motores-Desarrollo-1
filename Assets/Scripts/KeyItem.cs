using UnityEngine;

public class KeyItem : InteractableObject
{
    [Header("Información del objeto")]
    [SerializeField] private string itemID = "Pieza1";
    [SerializeField] private string itemName = "Pieza";

    // Configurar el objeto desde ItemSpawner
    public void SetItemData(string newID, string newName)
    {
        itemID = newID;
        itemName = newName;
        gameObject.name = newName;
    }

    // Interacción
    public override void Interact()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("No existe un Inventory en la escena.");
            return;
        }

        bool added = Inventory.Instance.AddItem(itemID);

        if (!added)
        {
            Debug.Log("Este objeto ya está en el inventario.");
            return;
        }

        Debug.Log("Objeto recogido: " + itemName);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ItemCollected();
        }

        Destroy(gameObject);
    }

    // Texto de interacción
    public override string GetInteractText()
    {
        return itemName + " [E] para recoger";
    }
}
