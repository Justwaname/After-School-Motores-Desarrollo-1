using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    // Se ejecuta cuando el jugador interactúa con el objeto
    public virtual void Interact()
    {
        Debug.Log("Se interactuó con el objeto.");
    }

    // Texto que puede mostrar la interfaz
    public virtual string GetInteractText()
    {
        return "[E] Interactuar";
    }
}