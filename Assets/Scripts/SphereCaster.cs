using UnityEngine;
using UnityEngine.InputSystem;

public class SphereCaster : MonoBehaviour
{
    [Header("SphereCast Settings")]
    public float sphereRadius = 1.0f;
    public float castDistance = 10.0f;
    public LayerMask hitLayers;

    [Header("Detection Tags")]
    public string wallTag = "Wall";
    public string floorTag = "Floor";

    [Header("Input Settings")]

    public InputActionReference castActionReference;

    void OnEnable()
    {
        // se verifica que se haya asignado la referencia en el Inspector
        if (castActionReference != null)
        {
            castActionReference.action.Enable();
            castActionReference.action.performed += OnCastPerformed;
            Debug.Log("SphereCaster: Input Enabled via Action Reference.");
        }
        else
        {
            Debug.LogWarning("SphereCaster: No Input Action Reference assigned in the Inspector!");
        }
    }

    void OnDisable()
    {
        if (castActionReference != null)
        {
            castActionReference.action.performed -= OnCastPerformed;
            castActionReference.action.Disable();
            Debug.Log("SphereCaster: Input Disabled.");
        }
    }

    private void OnCastPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Input detected! Casting sphere...");
        PerformSphereCast();
    }

    private void PerformSphereCast()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;
        RaycastHit hitInfo;

        // Lanza el SphereCast considerando la máscara de colisión
        bool hasHit = Physics.SphereCast(origin, sphereRadius, direction, out hitInfo, castDistance, hitLayers);

        if (hasHit)
        {
            GameObject hitObject = hitInfo.collider.gameObject;
            Debug.Log("SphereCast HIT: " + hitObject.name);

            // Verifica si el objeto impactado tiene la etiqueta de pared o piso
            if (hitObject.CompareTag(wallTag))
            {
                Debug.Log("SUCCESS: Wall detected! Distance: " + hitInfo.distance);
            }
            else if (hitObject.CompareTag(floorTag))
            {
                Debug.Log("SUCCESS: Floor detected! Distance: " + hitInfo.distance);
            }
            else
            {
                Debug.Log("INFO: Other object detected. Tag: " + hitObject.tag);
            }
        }
        else
        {
            Debug.Log("SphereCast MISS: Trajectory is clear.");
        }
    }

    void OnDrawGizmos()
    {
        // Dibuja la esfera y la línea en el editor para visualizar la trayectoria
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);

        Vector3 endPosition = transform.position + (transform.forward * castDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(endPosition, sphereRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, endPosition);
    }
}
