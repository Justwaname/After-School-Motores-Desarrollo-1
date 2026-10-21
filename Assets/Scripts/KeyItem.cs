using UnityEngine;

public class KeyItem : InteractableObject
{
    [Header("Información del objeto")]
    [SerializeField] private string itemID = "Pieza1";
    [SerializeField] private string itemName = "Pieza";

    public void SetItemData(string newID, string newName)
    {
        itemID = newID;
        itemName = newName;
        gameObject.name = newName;
    }

    public override void Interact()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("No existe un Inventory en la escena.");
            return;
        }

        if (Inventory.Instance.HasItem(itemID))
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

        bool added = Inventory.Instance.AddItem(itemID);

        if (!added)
            return;

        Debug.Log("Objeto recogido: " + itemName);

        if (GameManager.Instance != null)
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
