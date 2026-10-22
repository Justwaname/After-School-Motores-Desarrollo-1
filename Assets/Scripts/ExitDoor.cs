using UnityEngine;

public class ExitDoor : InteractableObject
{
    public override void Interact()
    {
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

            Destroy(gameObject);
        }
        else
        {
            GameManager.Instance.ShowNotification(
                "You need to combine the 3 items!"
            );
        }
    }

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