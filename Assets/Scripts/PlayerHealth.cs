using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Daño")]
    [SerializeField] private int maxHits = 3;
    private int currentHits = 0;
    private bool isInvulnerable = false;

    [Header("Inmunidad y Velocidad Temporal")]
    [SerializeField] private float invulnerabilityDuration = 1.5f;

    [Header("Recuperación de Vida")]
    [SerializeField] private float healCooldown = 50f; // Segundos necesarios para recuperar un corazón
    private float timeSinceLastDamage = 0f;

    [Header("Efecto Visual (Viñeta Roja)")]
    [SerializeField] private Image damageVignetteImage;
    [SerializeField] private float vignetteFadeSpeed = 3f;
    private Coroutine vignetteFadeRoutine;

    [Header("Interfaz de Corazones (UI)")]
    [SerializeField] private Image[] heartImages;
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;

    [Header("Animación de Corazones Llenos")]
    [SerializeField] private float floatSpeed = 5f;    // Velocidad de subida y bajada
    [SerializeField] private float floatHeight = 6f;   // Distancia en píxeles que se mueven
    private Vector2[] heartOriginalPositions;          // Almacena las posiciones originales de los corazones

    [Header("Pantalla de Game Over")]
    [SerializeField] private GameObject gameOverTextObj;
    [SerializeField] private GameObject restartButtonObj;
    [SerializeField] private GameObject MenuButtonobj;

    void Start()
    {
        // Guardar las posiciones originales de los corazones en la UI
        if (heartImages != null && heartImages.Length > 0)
        {
            heartOriginalPositions = new Vector2[heartImages.Length];
            for (int i = 0; i < heartImages.Length; i++)
            {
                if (heartImages[i] != null)
                {
                    heartOriginalPositions[i] = heartImages[i].rectTransform.anchoredPosition;
                }
            }
        }

        if (damageVignetteImage != null)
        {
            Color c = damageVignetteImage.color;
            c.a = 0f;
            damageVignetteImage.color = c;
        }

        if (gameOverTextObj) gameOverTextObj.SetActive(false);
        if (restartButtonObj) restartButtonObj.SetActive(false);
        if (MenuButtonobj) MenuButtonobj.SetActive(false);
        ActualizarCorazonesUI();
    }

    void Update()
    {
        // Animamos los corazones en tiempo real
        AnimarCorazonesLlenos();

        // Control de recuperación de vida tras 50 segundos sin recibir daño
        if (currentHits > 0)
        {
            timeSinceLastDamage += Time.deltaTime;

            if (timeSinceLastDamage >= healCooldown)
            {
                CurarCorazon();
                timeSinceLastDamage = 0f; // Reiniciamos el cronómetro
            }
        }
    }

    public void TakeDamage()
    {
        if (isInvulnerable || currentHits >= maxHits) return;

        currentHits++;
        timeSinceLastDamage = 0f; // Reiniciamos el temporizador al recibir daño

        ActualizarEfectoVisualAlRecibirGolpe();
        ActualizarCorazonesUI();

        if (currentHits >= maxHits)
        {
            Morir();
        }
        else
        {
            StartCoroutine(DamageRoutine());
        }
    }

    private void CurarCorazon()
    {
        if (currentHits > 0)
        {
            currentHits--; // Restamos un golpe (recupera 1 corazón)
            ActualizarCorazonesUI();
            Debug.Log("¡Has sobrevivido el tiempo suficiente! Has recuperado 1 corazón.");
        }
    }

    private void ActualizarEfectoVisualAlRecibirGolpe()
    {
        if (damageVignetteImage == null) return;

        if (vignetteFadeRoutine != null)
        {
            StopCoroutine(vignetteFadeRoutine);
        }

        Color c = damageVignetteImage.color;

        if (currentHits == 1) c.a = 0.3f;
        else if (currentHits == 2) c.a = 0.4f;

        damageVignetteImage.color = c;

        vignetteFadeRoutine = StartCoroutine(FadeVignetteRoutine());
    }

    private IEnumerator FadeVignetteRoutine()
    {
        yield return new WaitForSeconds(0.6f);

        Color c = damageVignetteImage.color;
        float targetAlpha = 0f;

        if (currentHits == 2)
        {
            targetAlpha = 0.05f;
        }

        while (Mathf.Abs(c.a - targetAlpha) > 0.01f)
        {
            c.a = Mathf.MoveTowards(c.a, targetAlpha, Time.deltaTime * vignetteFadeSpeed);
            damageVignetteImage.color = c;
            yield return null;
        }

        c.a = targetAlpha;
        damageVignetteImage.color = c;
    }

    private void ActualizarCorazonesUI()
    {
        if (heartImages == null) return;

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] != null)
            {
                if (i < currentHits)
                {
                    heartImages[i].sprite = emptyHeartSprite;

                    // Si se vuelve vacío, lo devolvemos inmediatamente a su posición original exacta
                    if (heartOriginalPositions != null)
                    {
                        heartImages[i].rectTransform.anchoredPosition = heartOriginalPositions[i];
                    }
                }
                else
                {
                    heartImages[i].sprite = fullHeartSprite;
                }
            }
        }
    }

    private void AnimarCorazonesLlenos()
    {
        if (heartImages == null || heartOriginalPositions == null) return;

        for (int i = 0; i < heartImages.Length; i++)
        {
            // Solo animamos los corazones que están llenos (índice mayor o igual a currentHits)
            if (heartImages[i] != null && i >= currentHits)
            {
                // Usamos Mathf.Sin para crear un movimiento de flotación suave vertical
                float offsetY = Mathf.Sin(Time.time * floatSpeed + i) * floatHeight;

                Vector2 newPos = heartOriginalPositions[i];
                newPos.y += offsetY;

                heartImages[i].rectTransform.anchoredPosition = newPos;
            }
        }
    }

    private IEnumerator DamageRoutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }

    private void Morir()
    {
        Debug.Log("El jugador ha muerto tras recibir 3 golpes.");

        if (damageVignetteImage != null)
        {
            Color c = damageVignetteImage.color;
            c.a = .0f;
            damageVignetteImage.color = c;
        }

        if (gameOverTextObj != null) gameOverTextObj.SetActive(true);
        if (restartButtonObj != null) restartButtonObj.SetActive(true);
        if (MenuButtonobj != null) MenuButtonobj.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ReiniciarJuego()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void volverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("menuinicial");
    }
}