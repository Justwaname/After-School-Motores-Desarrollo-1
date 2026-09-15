using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public abstract void Interact();

    public virtual string GetInteractText()
    {
        return gameObject.name + " interact";
    }
}