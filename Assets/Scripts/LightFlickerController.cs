using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlickerController : MonoBehaviour
{
    private Light pointLight;

    [Header("Tiempos de espera (mínimo y máximo)")]
    public float minTiempoApagado = 0.5f;
    public float maxTiempoApagado = 2.0f;

    public float minTiempoEncendido = 0.1f;
    public float maxTiempoEncendido = 0.7f;

    [Header("Opcional: Efecto de intensidad aleatoria")]
    public bool cambiarIntensidad = false;
    public float intensidadMinima = 0.5f;
    public float intensidadMaxima = 2.0f;
    private float intensidadOriginal;

    void Start()
    {
        pointLight = GetComponent<Light>();
        intensidadOriginal = pointLight.intensity;

        // Iniciamos el bucle de parpadeo
        StartCoroutine(FlickerRoutine());
    }

    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // 1. Apagar la luz (o bajar intensidad)
            pointLight.enabled = false;

            // Esperar un tiempo aleatorio estando apagada
            yield return new WaitForSeconds(Random.Range(minTiempoApagado, maxTiempoApagado));

            // 2. Encender la luz
            pointLight.enabled = true;

            // Si quieres que además varíe la intensidad al encenderse
            if (cambiarIntensidad)
            {
                pointLight.intensity = Random.Range(intensidadMinima, intensidadMaxima);
            }
            else
            {
                pointLight.intensity = intensidadOriginal;
            }

            // Esperar un tiempo aleatorio estando encendida
            yield return new WaitForSeconds(Random.Range(minTiempoEncendido, maxTiempoEncendido));
        }
    }
}
