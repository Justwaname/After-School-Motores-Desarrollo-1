using UnityEngine;
using System;

public class ItemSpawner : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private GameObject keyItemPrefab;
    [SerializeField] private string spawnPointTag = "SpawnPoint";

    private void Start()
    {
        SpawnItems();
    }

    private void SpawnItems()
    {
        if (keyItemPrefab == null)
        {
            Debug.LogError("[ItemSpawner] No asignaste el Key Item Prefab.");
            return;
        }

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
    }
}