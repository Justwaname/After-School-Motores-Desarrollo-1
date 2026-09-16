using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Configuración de Spawns")]
    [SerializeField] private GameObject keyItemPrefab;
    [SerializeField] private string spawnPointTag = "SpawnPoint";

    // --- INICIALIZACIÓN ---
    void Start()
    {
        SpawnItems();
    }

    // --- GENERACIÓN DE OBJETOS ---
    void SpawnItems()
    {
        if (keyItemPrefab == null || spawnPointTag == null) return;

        GameObject[] spawnObjects = GameObject.FindGameObjectsWithTag(spawnPointTag);

        if (spawnObjects.Length == 0)
        {
            Debug.LogWarning($"[ItemSpawner] No se encontraron objetos con el Tag '{spawnPointTag}' en la escena.");
            return;
        }

        // --- CREAR ITEMS ---
        foreach (GameObject spawnPoint in spawnObjects)
        {
            if (spawnPoint != null)
            {
                Instantiate(keyItemPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            }
        }
    }

    // --- VISUALIZACIÓN DE SPAWNS ---
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        GameObject[] spawnObjects = GameObject.FindGameObjectsWithTag(spawnPointTag);

        foreach (GameObject spawnPoint in spawnObjects)
        {
            if (spawnPoint != null)
            {
                Gizmos.DrawSphere(spawnPoint.transform.position, 0.4f);
            }
        }
    }
}