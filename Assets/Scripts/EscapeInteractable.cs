using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EscapeInteractable : InteractableObject
{
    [Header("Configuración de Escape")]
    [SerializeField] private float delayBeforeLoad = 1.5f;

    [Header("UI Opcional")]
    [SerializeField] private TMPro.TextMeshProUGUI escapeText;

    private bool hasEscaped = false;

    public override void Interact()
    {
        if (hasEscaped) return;
        hasEscaped = true;

        Debug.Log("¡El jugador ha decidido escapar!");

        if (HorrorSFXManager.Instance != null)
        {
            HorrorSFXManager.Instance.PlaySFX(HorrorSFXEvent.ExitDoor, transform.position);
        }

        if (escapeText != null)
        {
            escapeText.text = "¡ESCAPASTE!";
            escapeText.gameObject.SetActive(true);
        }

        StopPulse();

        // 1. Pausamos el juego en el momento de interactuar
        Time.timeScale = 0f;

        // 2. Iniciamos la transición
        StartCoroutine(LoadNextSceneRoutine());
    }

    private IEnumerator LoadNextSceneRoutine()
    {
        // Esperamos en tiempo real ignorando el Time.timeScale = 0
        yield return new WaitForSecondsRealtime(delayBeforeLoad);

        // 3. Restauramos el tiempo a la normalidad antes de cambiar de escena
        Time.timeScale = 1f;

        // Cargamos la siguiente escena
        SceneManager.LoadScene("Pantalla victoria");
    }

    public override string GetInteractText()
    {
        if (hasEscaped) return "";
        return "[E] ¡¡ESCAPAR!!";
    }
}