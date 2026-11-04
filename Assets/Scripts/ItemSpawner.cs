using System;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnPair
    {
        [Tooltip("El punto exacto (Transform) donde aparecerá el objeto.")]
        public Transform spawnPoint;

        [Tooltip("El objeto específico que se spawnea en este punto.")]
        public GameObject itemPrefab;
    }

    [Header("Configuración de Spawns")]
    [SerializeField] private SpawnPair[] spawnPairs;
    [SerializeField] private string spawnPointTag = "SpawnPoint"; // Opcional si prefieres buscar por Tag

    private void Start()
    {
        SpawnSpecificItems();
    }

    private void SpawnSpecificItems()
    {
        if (spawnPairs == null || spawnPairs.Length == 0)
        {
            Debug.LogError("ItemSpawner: No hay pares de spawn configurados.");
            return;
        }

        foreach (var pair in spawnPairs)
        {
            if (pair.spawnPoint == null)
            {
                Debug.LogWarning("ItemSpawner: Hay un SpawnPoint vacío en la lista.");
                continue;
            }

            if (pair.itemPrefab == null)
            {
                Debug.LogWarning($"ItemSpawner: El punto '{pair.spawnPoint.name}' no tiene un prefab asignado.");
                continue;
            }

            // Instanciamos el prefab específico en la posición y rotación del punto asignado
            Instantiate(pair.itemPrefab, pair.spawnPoint.position, pair.spawnPoint.rotation);
        }
    }

    // Gizmos para visualizar los puntos en la escena
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // Si usas la lista de pares en el inspector
        if (spawnPairs != null)
        {
            foreach (var pair in spawnPairs)
            {
                if (pair.spawnPoint != null)
                {
                    Gizmos.DrawSphere(pair.spawnPoint.position, 0.4f);
                }
            }
        }

        // (Opcional) Si también quieres seguir detectando por Tag para los gizmos
        GameObject[] spawnObjects = GameObject.FindGameObjectsWithTag(spawnPointTag);
        foreach (GameObject spawnPoint in spawnObjects)
        {
            if (spawnPoint != null)
            {
                Gizmos.DrawWireSphere(spawnPoint.transform.position, 0.45f);
            }
        }
    }
}