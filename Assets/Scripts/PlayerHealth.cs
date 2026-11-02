using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Daño")]
    [SerializeField] private int maxHits = 3;
    private int currentHits = 0;
    private bool isInvulnerable = false;

    [Header("Inmunidad y Velocidad Temporal")]
    [SerializeField] private float invulnerabilityDuration = 1.5f;

    [Header("Efecto Visual (Viñeta Roja)")]
    [SerializeField] private Image damageVignetteImage;

    [Header("Interfaz de Corazones (UI)")]
    [SerializeField] private Image[] heartImages; // Arrastra aquí tus 3 imágenes de corazones desde el Canvas en orden (0, 1, 2)
    [SerializeField] private Sprite fullHeartSprite; // Arrastra el sprite de corazon lleno
    [SerializeField] private Sprite emptyHeartSprite; // Arrastra el sprite de corazon muerto

    void Start()
    {
        // Asegurar que la viñeta empiece transparente
        if (damageVignetteImage != null)
        {
            Color c = damageVignetteImage.color;
            c.a = 0f;
            damageVignetteImage.color = c;
        }

        ActualizarCorazonesUI();
    }

    public void TakeDamage()
    {
        if (isInvulnerable || currentHits >= maxHits) return;

        currentHits++;

        ActualizarEfectoVisual();
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

    private void ActualizarEfectoVisual()
    {
        if (damageVignetteImage != null)
        {
            Color c = damageVignetteImage.color;

            // Bordes rojos según el golpe (centro libre)
            if (currentHits == 1) c.a = 0.2f;
            else if (currentHits == 2) c.a = 0.5f;
            else if (currentHits >= 3) c.a = 1.0f;

            damageVignetteImage.color = c;
        }
    }

    private void ActualizarCorazonesUI()
    {
        if (heartImages == null) return;

        // Recorre los corazones y cambia el sprite según los golpes recibidos
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] != null)
            {
                // Si el índice es menor a los golpes actuales, el corazón está "muerto/perdido"
                if (i < currentHits)
                {
                    heartImages[i].sprite = emptyHeartSprite;
                }
                else
                {
                    heartImages[i].sprite = fullHeartSprite;
                }
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
   // aca se  cambiara a la escena de muerte
    }
}