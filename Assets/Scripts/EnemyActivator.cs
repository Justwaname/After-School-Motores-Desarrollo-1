using UnityEngine;

public class EnemyActivator : MonoBehaviour
{
    [Header("Configuración del Activador")]
    [Tooltip("Arrastra aquí el GameObject del enemigo que se activará.")]
    [SerializeField] private GameObject enemyObject;

    [Header("Configuración de Audio")]
    [Tooltip("Distancia hacia la izquierda del jugador donde sonará el efecto.")]
    [SerializeField] private float soundOffsetDistance = 3f;

    [Tooltip("Si está marcado, el trigger se destruirá después de usarse una sola vez.")]
    [SerializeField] private bool destroyOnTrigger = true;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        // Comprobamos que el que cruza sea el jugador
        if (other.CompareTag("Player"))
        {
            triggered = true;

            if (enemyObject != null)
            {
                // 1. Activamos el GameObject del enemigo
                enemyObject.SetActive(true);

                // 2. Calculamos la posición a la izquierda del jugador
                // Usamos -other.transform.right para obtener el vector izquierdo exacto según hacia dónde mira
                Vector3 soundPosition = other.transform.position - (other.transform.right * soundOffsetDistance);

                // Reproducimos el sonido en esa posición específica
                if (HorrorSFXManager.Instance != null)
                {
                    HorrorSFXManager.Instance.PlaySFX(HorrorSFXEvent.ExitDoor, soundPosition);
                }

                Debug.Log("¡El enemigo se activó y el sonido sonó a la izquierda del jugador!");
            }
            else
            {
                Debug.LogWarning("No se ha asignado ningún enemigo en el EnemyActivatorTrigger.");
            }

            // 3. Destruimos el trigger para que no se vuelva a activar
            if (destroyOnTrigger)
            {
                Destroy(gameObject);
            }
        }
    }
}