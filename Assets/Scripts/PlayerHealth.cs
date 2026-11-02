using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // <-- Necesario para reiniciar la escena

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
    [SerializeField] private Image[] heartImages;
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;

    [Header("Pantalla de Game Over")]
    [SerializeField] private GameObject gameOverTextObj;   // Arrastra aquí tu GameOverText
    [SerializeField] private GameObject restartButtonObj;  
    [SerializeField] private GameObject MenuButtonobj;     
    void Start()
    {
        // Asegurar que la viñeta y los elementos de muerte comiencen ocultos
        if (damageVignetteImage != null)
        {
            Color c = damageVignetteImage.color;
            c.a = 0f;
            damageVignetteImage.color = c;
        }

        if (gameOverTextObj) gameOverTextObj.SetActive(false);
        if (restartButtonObj ) restartButtonObj.SetActive(false);
        if (MenuButtonobj ) MenuButtonobj.SetActive(false);
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

           
            if (currentHits == 1) c.a = 0.2f;
            else if (currentHits == 2) c.a = 0.5f;
            else if (currentHits >= 3) c.a = 1.0f;

            damageVignetteImage.color = c;
        }
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
