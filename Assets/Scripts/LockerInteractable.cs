using UnityEngine;

public class LockerInteractable : InteractableObject
{
    [Header("Posiciones")]
    [SerializeField] private Transform hidePoint;
    [SerializeField] private Transform exitPoint;

    [Header("Configuración de Sigilo")]
    [SerializeField] private float segundosEsperaPersecucion = 4f; // <--- Segundos exactos que defines tú

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

        if (EstaElEnemigoMuyCercaOPersiguiendo())
        {
            Debug.Log("¡El enemigo te vio o está muy cerca, debes esperar a despistarlo!");
            return;
        }

        player.TryEnterHide(this);
    }

    private bool EstaElEnemigoMuyCercaOPersiguiendo()
    {
        EnemyIA enemigo = Object.FindFirstObjectByType<EnemyIA>();

        if (enemigo == null || enemigo.target == null) return false;

        float distanciaAlJugador = Vector3.Distance(enemigo.transform.position, enemigo.target.transform.position);

        // 1. Si el enemigo está en rango de visión directo o atacando, bloqueamos al instante
        if (distanciaAlJugador <= enemigo.distanciaDeteccion || enemigo.atacando)
        {
            return true;
        }

        // 2. Comprobamos el periodo de gracia directamente desde la IA del enemigo usando tus segundos
        if (enemigo.EstaEnPeriodoDeGracia(segundosEsperaPersecucion))
        {
            return true; // Todavía está en el tiempo de gracia, no puedes esconderte
        }

        return false;
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

        if (EstaElEnemigoMuyCercaOPersiguiendo())
            return "¡El enemigo te vigila, espera a despistarlo!";

        return "E - Esconderse";
    }
}