using UnityEngine;
using TMPro; // Necesario para usar TextMeshPro

public class WinZone : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winText; // Arrastra aquí tu texto de UI

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el que cruza es el Player (puedes usar un Tag o verificar el componente)
        if (other.CompareTag("Player") || other.GetComponent<PlayerController>() != null)
        {
            if (winText != null)
            {
                winText.text = "¡ESCAPASTE!"; // O el mensaje que quieras mostrar
                winText.gameObject.SetActive(true);
            }

            // Opcional: Pausar el juego o congelar al jugador
            Time.timeScale = 0f;
        }
    }
}