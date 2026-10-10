using UnityEngine;
using UnityEngine.InputSystem;

public class SphereCastInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRadius = 0.5f;
    public float interactDistance = 2.5f;

    public LayerMask interactableLayer;

    private PlayerInput playerInput;

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
        if (playerInput != null &&
            playerInput.actions["Interact"].WasPressedThisFrame())
        {
            Debug.Log("E presionada");
            TryInteract();
        }
    }

    private void TryInteract()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        Collider hitCollider = null;

        // OverlapSphere
        Collider[] overlappingColliders = Physics.OverlapSphere(origin, interactRadius, interactableLayer);

        if (overlappingColliders.Length > 0)
        {
            hitCollider = overlappingColliders[0];
        }
        else
        {
            // SphereCast
            RaycastHit hitInfo;

            bool hasHit = Physics.SphereCast(
                origin,
                interactRadius,
                direction,
                out hitInfo,
                interactDistance,
                interactableLayer
            );

            if (hasHit)
            {
                hitCollider = hitInfo.collider;
            }
        }

        // 3. Comprobamos el resultado obtenido (ya sea por OverlapSphere o por SphereCast)
        if (hitCollider != null)
        {
            InteractableObject interactable =
                hitCollider.GetComponentInParent<InteractableObject>();

            if (interactable != null)
            {
                Debug.Log(
                    "SUCCESS: Interactable -> " +
                    hitCollider.gameObject.name
                );

                interactable.Interact();
            }
            else
            {
                Debug.Log(
                    "Objeto encontrado pero no tiene InteractableObject."
                );
            }
        }
        else
        {
            Debug.Log("MISS: No hay objetos interactuables.");
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

        Gizmos.DrawWireSphere(
            endPosition,
            interactRadius
        );

        Gizmos.color = Color.white;

        Gizmos.DrawLine(
            transform.position,
            endPosition
        );
    }
}