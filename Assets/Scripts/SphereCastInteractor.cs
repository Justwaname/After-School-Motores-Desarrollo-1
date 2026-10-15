using UnityEngine;
using UnityEngine.InputSystem;

public class SphereCastInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRadius = 0.5f;
    public float interactDistance = 2.5f;

    public LayerMask interactableLayer;

    private PlayerInput playerInput;
    private InputAction interactAction;
    private PlayerHideController hideController;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        hideController = GetComponent<PlayerHideController>();
    }

    private void Start()
    {
        if (playerInput == null)
        {
            Debug.LogError(
                "No se encontró PlayerInput en el Player."
            );
            return;
        }

        interactAction = playerInput.actions.FindAction(
            "Interact",
            false
        );

        if (interactAction == null)
        {
            Debug.LogError(
                "No existe la acción 'Interact' en PlayerInput."
            );
        }
    }

    private void Update()
    {
        if (interactAction == null ||
            !interactAction.WasPressedThisFrame())
        {
            return;
        }

        // Si está escondido, E sirve para salir.
        if (hideController != null && hideController.IsHidden)
        {
            hideController.TryExitHide();
            return;
        }

        TryInteract();
    }

    private void TryInteract()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        bool hasHit = Physics.SphereCast(
            origin,
            interactRadius,
            direction,
            out RaycastHit hitInfo,
            interactDistance,
            interactableLayer
        );

        if (!hasHit)
        {
            Debug.Log("No hay objetos interactuables delante.");
            return;
        }

        InteractableObject interactable =
            hitInfo.collider.GetComponentInParent<InteractableObject>();

        if (interactable != null)
        {
            interactable.Interact();
        }
        else
        {
            Debug.Log(
                "Objeto detectado, pero no tiene InteractableObject."
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(
            transform.position,
            interactRadius
        );

        Vector3 endPosition =
            transform.position +
            transform.forward * interactDistance;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(endPosition, interactRadius);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, endPosition);
    }
}