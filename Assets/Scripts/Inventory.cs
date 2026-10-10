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

    // Añadir objeto al inventario
    // Devuelve true si realmente se añadió.
    public bool AddItem(string itemID)
    {
        if (items.Contains(itemID))
        {
            return false;
        }

        items.Add(itemID);

        Debug.Log("Objeto obtenido: " + itemID);

        CheckCombination();

        return true;
    }

    // Comprobar si tenemos un objeto
    public bool HasItem(string itemID)
    {
        return items.Contains(itemID);
    }

    // Eliminar objeto
    private void RemoveItem(string itemID)
    {
        if (items.Contains(itemID))
        {
            items.Remove(itemID);
        }
    }

    // Comprobar si las 3 piezas están juntas
    private void CheckCombination()
    {
        if (HasItem("Pieza1") &&
            HasItem("Pieza2") &&
            HasItem("Pieza3"))
        {
            RemoveItem("Pieza1");
            RemoveItem("Pieza2");
            RemoveItem("Pieza3");

            // Crear el objeto nuevo
            items.Add("LlaveFinal");

            Debug.Log("=================================");
            Debug.Log("LAS 3 PIEZAS SE HAN COMBINADO");
            Debug.Log("HAS CREADO LA LLAVE FINAL");
            Debug.Log("=================================");
        }
    }
}