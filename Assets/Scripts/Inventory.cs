using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    private List<string> items = new List<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool AddItem(string itemID)
    {
        if (items.Contains(itemID))
            return false;

        items.Add(itemID);

        Debug.Log("Objeto obtenido: " + itemID);

        CheckCombination();

        return true;
    }

    public bool HasItem(string itemID)
    {
        return items.Contains(itemID);
    }

    private void RemoveItem(string itemID)
    {
        items.Remove(itemID);
    }

    private void CheckCombination()
    {
        if (HasItem("Pieza1") &&
            HasItem("Pieza2") &&
            HasItem("Pieza3"))
        {
            RemoveItem("Pieza1");
            RemoveItem("Pieza2");
            RemoveItem("Pieza3");

            items.Add("LlaveFinal");

            Debug.Log("=================================");
            Debug.Log("LAS 3 PIEZAS SE HAN COMBINADO");
            Debug.Log("HAS CREADO LA LLAVE FINAL");
            Debug.Log("=================================");

            if (HorrorSFXManager.Instance != null)
            {
                HorrorSFXManager.Instance.PlaySFX(
                    HorrorSFXEvent.KeyCombined,
                    transform.position
                );
            }
        }
    }
}