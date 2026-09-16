using UnityEngine;

public class ExitDoor : InteractableObject
{
    // --- INTERACCIÓN CON LA PUERTA ---
    public override void Interact()
    {
        if (GameManager.Instance.CanEscape())
        {
            Destroy(gameObject);
            GameManager.Instance.ShowNotification("Door opened!");
        }
        else
        {
            GameManager.Instance.ShowNotification("You need more items to unlock the exit!");
        }
    }

    // --- TEXTO DE INTERACCIÓN ---
    public override string GetInteractText()
    {
        if (GameManager.Instance.CanEscape())
        {
            return "Exit Door [E] to Escape";
        }
        else
        {
            return "Exit Door [Locked]";
        }
    }
}