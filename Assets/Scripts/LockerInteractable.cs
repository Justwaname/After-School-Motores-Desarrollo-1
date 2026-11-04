using UnityEngine;

public class LockerInteractable : InteractableObject
{
    [Header("Posiciones")]
    [SerializeField] private Transform hidePoint;
    [SerializeField] private Transform exitPoint;

    [Header("Animación de la taquilla")]
    [SerializeField] private Animator lockerAnimator;
    [SerializeField] private string openTrigger = "Open";
    [SerializeField] private string closeTrigger = "Close";

    public Transform HidePoint => hidePoint;
    public Transform ExitPoint => exitPoint;

    private PlayerHideController occupant;

    public override void Interact()
    {
        PlayerHideController player = Object.FindFirstObjectByType<PlayerHideController>();

        if (player == null)
        {
            Debug.LogError("No se encontró PlayerHideController en el jugador.");
            return;
        }

        player.TryEnterHide(this);
    }

    public bool TryOccupy(PlayerHideController player)
    {
        if (occupant != null && occupant != player)
            return false;

        occupant = player;
        return true;
    }

    public void Release(PlayerHideController player)
    {
        if (occupant == player)
        {
            occupant = null;
        }
    }

    public void PlayOpenAnimation()
    {
        SetAnimatorTrigger(openTrigger);
    }

    public void PlayCloseAnimation()
    {
        SetAnimatorTrigger(closeTrigger);
    }

    private void SetAnimatorTrigger(string triggerName)
    {
        if (lockerAnimator == null || string.IsNullOrEmpty(triggerName)) return;

        foreach (AnimatorControllerParameter parameter in lockerAnimator.parameters)
        {
            if (parameter.name == triggerName && parameter.type == AnimatorControllerParameterType.Trigger)
            {
                lockerAnimator.ResetTrigger(triggerName);
                lockerAnimator.SetTrigger(triggerName);
                return;
            }
        }
    }

    public override string GetInteractText()
    {
        if (occupant != null)
            return "Taquilla ocupada";

        return "E - Esconderse";
    }
}