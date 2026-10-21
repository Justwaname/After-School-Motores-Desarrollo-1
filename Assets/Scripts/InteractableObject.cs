using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public virtual void Interact()
    {
        Debug.Log("Se interactuó con el objeto.");
    }

    public virtual string GetInteractText()
    {
        return "[E] Interactuar";
    }
}