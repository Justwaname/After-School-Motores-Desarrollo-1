using UnityEngine;
using System.Collections;

public class SlideDoor : MonoBehaviour
{
    [Header("Configuración de Desplazamiento")]
    [SerializeField] private float slideDistance = 2f;    // Cuánto se desplaza en metros
    [SerializeField] private float slideSpeed = 2f;       // Velocidad de movimiento
    [SerializeField] private bool slideLeft = true;       // Dirección

    private Vector3 closedPosition;
    private Vector3 targetPosition;
    private bool isSliding = false;
    private bool isOpened = false;

    private void Start()
    {
        closedPosition = transform.position;

        // Calculamos hacia dónde se moverá usando el eje local izquierdo (-Transform.right) o derecho
        Vector3 direction = slideLeft ? -transform.right : transform.right;
        targetPosition = closedPosition + (direction * slideDistance);
    }

    // Método público para ordenar a la puerta que se abra deslizándose
    public void OpenSlideDoor()
    {
        if (isOpened) return;
        isOpened = true;

        StartCoroutine(SlideRoutine());
    }

    private IEnumerator SlideRoutine()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * slideSpeed;
            transform.position = Vector3.Lerp(startPos, targetPosition, elapsed);
            yield return null;
        }

        transform.position = targetPosition;
    }
}