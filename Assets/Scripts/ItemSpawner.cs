using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;           // Prefabs de los objetos a spawnear
    [SerializeField] private Transform[] spawnPoints;        // Puntos donde spawnear
    [SerializeField] private int spawnCount = 3;             // Cuántos spawnear

    private void Start()
    {
        SpawnUniqueItems();
    }

    private void SpawnUniqueItems()
    {
        if (prefabs == null || prefabs.Length == 0 ||
            spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("ItemSpawner: prefabs o spawnPoints no asignados.");
            return;
        }

        int maxPossible = Mathf.Min(prefabs.Length, spawnPoints.Length);
        int toSpawn = Mathf.Clamp(spawnCount, 1, maxPossible);

        // Copias para barajar sin modificar los originales
        List<int> prefabIndices = new List<int>();
        for (int i = 0; i < prefabs.Length; i++) prefabIndices.Add(i);
        Shuffle(prefabIndices);

        List<int> spawnIndices = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++) spawnIndices.Add(i);
        Shuffle(spawnIndices);

        for (int i = 0; i < toSpawn; i++)
        {
            GameObject prefab = prefabs[prefabIndices[i]];
            Transform spawnT = spawnPoints[spawnIndices[i]];
            if (prefab == null || spawnT == null) continue;
            Instantiate(prefab, spawnT.position, spawnT.rotation);
        }
    }

    // Fisher–Yates shuffle
    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }
    /*[Header("Configuración")]
    [SerializeField] private GameObject Thermite;
    [SerializeField] private GameObject Blowtorch;
    [SerializeField] private GameObject MagnesiumRibbon;
    [SerializeField] private string spawnPointTag = "SpawnPoint";

    private void Start()
    {
        SpawnItems();
    }

    private void SpawnItems()
    {

        GameObject[] spawnObjects =
            GameObject.FindGameObjectsWithTag(spawnPointTag);

        if (spawnObjects.Length < 3)
        {
            Debug.LogWarning(
                "[ItemSpawner] Necesitas al menos 3 SpawnPoints."
            );

            return;
        }

        // Ordenamos los SpawnPoints por nombre
        Array.Sort(
            spawnObjects,
            (a, b) => string.CompareOrdinal(a.name, b.name)
        );

        // PIEZA 1
        CrearItem(
            spawnObjects[0],
            "Pieza1",
            "Pieza 1"
        );

        // PIEZA 2
        CrearItem(
            spawnObjects[1],
            "Pieza2",
            "Pieza 2"
        );

        // PIEZA 3
        CrearItem(
            spawnObjects[2],
            "Pieza3",
            "Pieza 3"
        );
    }

    private void CrearItem(
        GameObject spawnPoint,
        string itemID,
        string itemName)
    {
        GameObject newItem = Instantiate(
            keyItemPrefab,
            spawnPoint.transform.position,
            spawnPoint.transform.rotation
        );

        KeyItem keyItem =
            newItem.GetComponent<KeyItem>();

        if (keyItem != null)
        {
            keyItem.SetItemData(itemID, itemName);
        }
        else
        {
            Debug.LogError(
                "[ItemSpawner] El prefab no tiene KeyItem."
            );
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        GameObject[] spawnObjects =
            GameObject.FindGameObjectsWithTag(spawnPointTag);

        foreach (GameObject spawnPoint in spawnObjects)
        {
            if (spawnPoint != null)
            {
                Gizmos.DrawSphere(
                    spawnPoint.transform.position,
                    0.4f
                );
            }
        }
    }*/
}