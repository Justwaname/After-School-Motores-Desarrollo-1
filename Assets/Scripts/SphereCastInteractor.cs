using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SphereCastInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactRadius = 0.5f;
    [SerializeField] private float interactDistance = 2.5f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private TextMeshProUGUI interactText;

    private PlayerInput playerInput;
    private InputAction interactAction;

    private InteractableObject currentInteractable;
    private PlayerHideController hideController;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        hideController = GetComponent<PlayerHideController>();

        if (playerInput == null)
        {
            Debug.LogError("No se encontró PlayerInput en el Player.");
        }
        else
        {
            interactAction = playerInput.actions.FindAction("Interact", false);

            if (interactAction == null)
            {
                Debug.LogError("No se encontró la acción Interact.");
            }
        }

        if (hideController == null)
        {
            Debug.LogError("No se encontró PlayerHideController en el Player.");
        }

        HideInteractText();
    }

    private void Update()
    {
        bool pressedE = interactAction != null &&
                        interactAction.WasPressedThisFrame();

        // Si está ocurriendo una transición, no permitir interacciones.
        if (hideController != null && hideController.IsTransitioning)
        {
            currentInteractable = null;
            HideInteractText();
            return;
        }

        // SI ESTÁ ESCONDIDO
        if (hideController != null && hideController.IsHidden)
        {
            currentInteractable = null;

            if (interactText != null)
            {
                interactText.text = "[E] Salir del casillero";
                interactText.gameObject.SetActive(true);
            }

            if (pressedE)
            {
                hideController.TryExitHide();
                HideInteractText();
            }

            return;
        }

        // SI NO ESTÁ ESCONDIDO
        CheckForInteractable();

        if (pressedE && currentInteractable != null)
        {
            Debug.Log(
                "E presionada: Interactuando con " +
                currentInteractable.gameObject.name
            );

            // No ejecutar animaciones al entrar en una taquilla.
            if (!(currentInteractable is LockerInteractable))
            {
                PlayerController player = GetComponent<PlayerController>();

                if (player != null)
                {
                    player.LockMovement();

                    if (currentInteractable is ExitDoor)
                    {
                        player.TriggerOpeningAnimation();
                        player.Invoke("UnlockMovement", 1.9f);
                    }
                    else
                    {
                        player.TriggerInteractAnimation();
                        player.Invoke("UnlockMovement", 1.3f);
                    }
                }
            }

            currentInteractable.Interact();

            HideInteractText();
        }
    }

    private void CheckForInteractable()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Collider hitCollider = null;

        Collider[] overlappingColliders = Physics.OverlapSphere(
            origin,
            interactRadius,
            interactableLayer
        );

        if (overlappingColliders.Length > 0)
        {
            hitCollider = overlappingColliders[0];
        }
        else if (Physics.SphereCast(
            origin,
            interactRadius,
            direction,
            out RaycastHit hitInfo,
            interactDistance,
            interactableLayer))
        {
            hitCollider = hitInfo.collider;
        }

        if (hitCollider != null)
        {
            InteractableObject interactable =
                hitCollider.GetComponentInParent<InteractableObject>();

            if (interactable != null)
            {
                currentInteractable = interactable;

                if (interactText != null)
                {
                    interactText.text =
                        currentInteractable.GetInteractText();

                    interactText.gameObject.SetActive(true);
                }

                return;
            }
        }

        currentInteractable = null;
        HideInteractText();
    }

    private void HideInteractText()
    {
        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRadius);

        Vector3 endPosition =
            transform.position + transform.forward * interactDistance;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(endPosition, interactRadius);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, endPosition);
    }
}