using UnityEngine;

public class ExitDoor : InteractableObject
{
    // --- INTERACCIÓN CON LA PUERTA ---
    public override void Interact()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("No existe GameManager en la escena.");
            return;
        }

        if (GameManager.Instance.CanEscape())
        {
            Debug.Log("¡PUERTA ABIERTA!");

            GameManager.Instance.ShowNotification(
                "Door opened!"
            );

            Destroy(gameObject);
        }
        else
        {
            GameManager.Instance.ShowNotification(
                "You need to combine the 3 items!"
            );
        }
    }

    // --- TEXTO DE INTERACCIÓN ---
    public override string GetInteractText()
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.CanEscape())
        {
            return "Exit Door [E] to Escape";
        }

        return "Exit Door [Locked]";
    }
}