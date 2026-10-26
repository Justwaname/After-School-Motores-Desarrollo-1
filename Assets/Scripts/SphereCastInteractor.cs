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
    private InteractableObject currentInteractable;

    // --- INICIALIZACIÓN ---
    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogError("No se encontró PlayerInput en el Player.");
        }

        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }
    }

    // --- LÓGICA PRINCIPAL ---
    private void Update()
    {
        // 1. Buscamos en cada frame si hay un interactuable para actualizar la UI
        CheckForInteractable();

        // 2. Si se presiona la E y hay un objeto válido
        if (playerInput != null &&
            playerInput.actions["Interact"].WasPressedThisFrame() &&
            currentInteractable != null)
        {
            Debug.Log("E presionada: Interactuando con " + currentInteractable.gameObject.name);
            currentInteractable.Interact();

            // Ocultamos la UI temporalmente o dejamos que se actualice sola en el siguiente frame
            if (interactText != null)
            {
                interactText.gameObject.SetActive(false);
            }
        }
    }

    // --- DETECCIÓN DE OBJETOS ---
    private void CheckForInteractable()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        Collider hitCollider = null;

        // --- DETECCIÓN CERCANA (OverlapSphere) ---
        Collider[] overlappingColliders = Physics.OverlapSphere(origin, interactRadius, interactableLayer);

        if (overlappingColliders.Length > 0)
        {
            hitCollider = overlappingColliders[0];
        }
        // --- DETECCIÓN HACIA ADELANTE (SphereCast) ---
        else if (Physics.SphereCast(origin, interactRadius, direction, out RaycastHit hitInfo, interactDistance, interactableLayer))
        {
            hitCollider = hitInfo.collider;
        }

        // --- COMPROBAR OBJETO INTERACTUABLE ---
        if (hitCollider != null)
        {
            // Usamos GetComponentInParent por si el collider está en un hijo y el script en el padre
            InteractableObject interactable = hitCollider.GetComponentInParent<InteractableObject>();

            if (interactable != null)
            {
                currentInteractable = interactable;

                // --- MOSTRAR UI ---
                if (interactText != null)
                {
                    interactText.text = currentInteractable.GetInteractText();
                    interactText.gameObject.SetActive(true);
                }
                return;
            }
        }

        // Si no hay nada al alcance, reseteamos la referencia y ocultamos la UI
        currentInteractable = null;
        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }
    }

    // --- VISUALIZACIÓN DEL GIZMO ---
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactRadius);

        Vector3 endPosition = transform.position + (transform.forward * interactDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(endPosition, interactRadius);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, endPosition);
    }
}