using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Configuración de Spawns")]
    [SerializeField] private GameObject keyItemPrefab; // Arrastra el prefab de la esfera aquí
    [SerializeField] private Transform[] spawnPoints;   // Arrastra los 3 SpawnPoints aquí

    void Start()
    {
        SpawnItems();
    }

    void SpawnItems()
    {
        if (keyItemPrefab == null || spawnPoints == null) return;

        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null)
            {
                Instantiate(keyItemPrefab, spawnPoint.position, spawnPoint.rotation);
            }
        }
    }
}