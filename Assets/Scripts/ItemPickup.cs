using UnityEngine;

public class ItemPickup : InteractableObject
{
    [Header("Configuración")]
    public string itemID;
    public string nombreObjeto;

    public override void Interact()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("No existe un Inventory en la escena.");
            return;
        }

        Inventory.Instance.AddItem(itemID);

        Debug.Log("Recogiste: " + nombreObjeto);

        Destroy(gameObject);
    }
}