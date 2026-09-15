using UnityEngine;

public class ExitDoor : InteractableObject
{
    public override void Interact()
    {
        if (GameManager.Instance.CanEscape())
        {
            GameManager.Instance.ShowNotification("You escaped!");
        }
        else
        {
            GameManager.Instance.ShowNotification("You need more items to unlock the exit!");
        }
    }

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