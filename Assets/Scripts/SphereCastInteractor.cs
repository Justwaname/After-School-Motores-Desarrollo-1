using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SphereCastInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactRadius = 0.5f; 
    [SerializeField] private float interactDistance = 2f; 
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private TextMeshProUGUI interactText;
    private PlayerInput playerInput;
    private InteractableObject currentInteractable;
    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogError("No se encontró PlayerInput en el Player.");
        }
    }

    private void Update()
    {
        // 1. Buscamos en cada frame si hay un interactuable
        CheckForInteractable();

        // 2. Si se presiona el botón de interacción y hay un objeto válido
        if (playerInput.actions["Interact"].WasPressedThisFrame() && currentInteractable != null)
        {
            currentInteractable.Interact();

            // Ocultamos la UI tras interactuar (útil si el objeto es destruido)
            if (interactText != null)
            {
                interactText.gameObject.SetActive(false);
            }
        }
    }

    private void CheckForInteractable()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        Collider hitCollider = null;

        // OverlapSphere para detectar dentro del origen
        Collider[] overlappingColliders = Physics.OverlapSphere(origin, interactRadius, interactableLayer);

        if (overlappingColliders.Length > 0)
        {
            hitCollider = overlappingColliders[0];
        }
        else if (Physics.SphereCast(origin, interactRadius, direction, out RaycastHit hitInfo, interactDistance, interactableLayer))
        {
            hitCollider = hitInfo.collider;
        }

        if (hitCollider != null)
        {
            InteractableObject interactable = hitCollider.GetComponent<InteractableObject>();

            if (interactable != null)
            {
                currentInteractable = interactable;

                // Actualizamos y mostramos el TextMeshPro
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

    // OnDrawGizmosSelected para que solo se dibuje cuando seleccionas el objeto en Unity
    void OnDrawGizmosSelected()
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
