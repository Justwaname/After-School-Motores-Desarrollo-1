using UnityEngine;

public class ExitDoor : InteractableObject
{
    public float anguloApertura = 90f;
    public float velocidadApertura = 2f;

    private bool puertaAbierta = false;

    private Quaternion rotacionCerrada;
    private Quaternion rotacionAbierta;

    private void Start()
    {
        rotacionCerrada = transform.localRotation;

        rotacionAbierta =
            rotacionCerrada *
            Quaternion.Euler(0f, anguloApertura, 0f);
    }

    private void Update()
    {
        if (puertaAbierta)
        {
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                rotacionAbierta,
                Time.deltaTime * velocidadApertura
            );
        }
    }
    public override void Interact()
    {
        if (puertaAbierta)
            return;

        if (GameManager.Instance == null)
        {
            Debug.LogError("No existe GameManager en la escena.");
            return;
        }

        if (GameManager.Instance.CanEscape())
        {
            if (HorrorSFXManager.Instance != null)
            {
                HorrorSFXManager.Instance.PlaySFX(
                    HorrorSFXEvent.ExitDoor,
                    transform.position
                );
            }

            Debug.Log("¡PUERTA ABIERTA!");
            GameManager.Instance.ShowNotification("Door opened!");
            
            if (Inventory.Instance != null)
            {
                Inventory.Instance.RemoveItem(Inventory.Instance.GetFinalItem());
                Inventory.Instance.RemoveItem(Inventory.Instance.GetBlowtorch());
            }

            puertaAbierta = true;
        }
        else
        {
            // Verificamos si al menos tiene los ingredientes para mostrarle un mensaje más descriptivo si se desea
            if (Inventory.Instance != null &&
                Inventory.Instance.HasItem(Inventory.Instance.GetMagnesiumRibbon()) &&
                Inventory.Instance.HasItem(Inventory.Instance.GetThermite()))
            {
                GameManager.Instance.ShowNotification("You need to combine the items in your inventory first!");
            }
            else
            {
                GameManager.Instance.ShowNotification("The door is locked.");
            }
        }
    }

    public override string GetInteractText()
    {
        if (GameManager.Instance != null && GameManager.Instance.CanEscape())
        {
            return "Exit Door [E] to Escape";
        }

        // Comprobamos el estado del inventario para cambiar el texto del prompt dinámicamente
        if (Inventory.Instance != null)
        {
            bool hasMagnesium = Inventory.Instance.HasItem(Inventory.Instance.GetMagnesiumRibbon());
            bool hasThermite = Inventory.Instance.HasItem(Inventory.Instance.GetThermite());

            // Si ya tiene los dos componentes de las piezas en el inventario pero no los ha combinado / falta el soplete
            if (hasMagnesium && hasThermite)
            {
                return "Need to combine items in inventory [E]";
            }
        }

        // Si no cumple lo anterior, muestra que tiene un candado
        return "Exit Door [Locked]";
    }
}