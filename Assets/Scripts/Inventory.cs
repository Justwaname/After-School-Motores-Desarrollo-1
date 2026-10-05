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

    public void AddItem(string itemID)
    {
        if (!items.Contains(itemID))
        {
            items.Add(itemID);

            Debug.Log("Objeto obtenido: " + itemID);

            CheckCombination();
        }
    }

    public bool HasItem(string itemID)
    {
        return items.Contains(itemID);
    }

    private void RemoveItem(string itemID)
    {
        if (items.Contains(itemID))
        {
            items.Remove(itemID);
        }
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

            AddItem("LlaveFinal");

            Debug.Log("¡LAS 3 PIEZAS SE HAN COMBINADO!");
            Debug.Log("¡HAS CONSEGUIDO LA LLAVE FINAL!");
            
        }
    }
}