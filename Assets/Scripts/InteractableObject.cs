using System.Collections;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Header("Efecto de Brillo (Nativo)")]
    [SerializeField] private bool enablePulseEffect = true;
    [SerializeField] private float pulseInterval = 3f; // Segundos entre cada destello
    [SerializeField] private float pulseDuration = 1f;  // Cuánto dura el destello
    [SerializeField] private Color glowColor = Color.yellow; // Color del brillo
    [SerializeField] private float maxEmissionIntensity = 1f;  // Intensidad del brillo (HDR)
    [SerializeField] private Renderer targetRenderer;

    private MaterialPropertyBlock propBlock;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    protected virtual void Awake()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<Renderer>();
        }

        propBlock = new MaterialPropertyBlock();

        if (enablePulseEffect && targetRenderer != null)
        {
            StartCoroutine(PulseEffectRoutine());
        }
    }

    private IEnumerator PulseEffectRoutine()
    {
        while (true)
        {
            // Espera el tiempo configurado antes del siguiente pulso
            yield return new WaitForSeconds(pulseInterval);

            float elapsed = 0f;
            while (elapsed < pulseDuration)
            {
                elapsed += Time.deltaTime;

                // Usamos una función senoidal para que el brillo suba y baje suavemente
                float currentIntensity = Mathf.Sin((elapsed / pulseDuration) * Mathf.PI) * maxEmissionIntensity;

                if (targetRenderer != null)
                {
                    targetRenderer.GetPropertyBlock(propBlock);

                    // Multiplicamos el color por la intensidad para lograr el efecto de brillo
                    Color finalColor = glowColor * currentIntensity;
                    propBlock.SetColor(EmissionColor, finalColor);

                    targetRenderer.SetPropertyBlock(propBlock);
                }

                yield return null;
            }

            // Apagamos la emisión al terminar el pulso
            if (targetRenderer != null)
            {
                targetRenderer.GetPropertyBlock(propBlock);
                propBlock.SetColor(EmissionColor, Color.black);
                targetRenderer.SetPropertyBlock(propBlock);
            }
        }
    }

    public virtual void Interact()
    {
        Debug.Log("Se interactuó con el objeto.");
    }

    public virtual string GetInteractText()
    {
        return "[E] Interactuar";
    }
}