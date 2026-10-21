using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    private List<ItemData> items = new List<ItemData>();
    public IReadOnlyList<ItemData> Items => items;

    [SerializeField] private ItemData Blowtorch;
    [SerializeField] private ItemData MagnesiumRibbon;
    [SerializeField] private ItemData Thermite;
    [SerializeField] private ItemData FinalItem;

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

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
    public bool AddItem(ItemData item)
    {
        if (items.Contains(item))
        {
            return false;
        }

        items.Add(item);

        Debug.Log("Objeto obtenido: " + item);

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();

        return true;
    }

    // Comprobar si tenemos un objeto
    public bool HasItem(ItemData item)
    {
        return items.Contains(item);
    }

    // Eliminar objeto
    private void RemoveItem(ItemData item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
        }
    }

    // Comprobar si las 3 piezas están juntas
    public void CheckCombination()
    {
       if (HasItem(Blowtorch) &&
            HasItem(MagnesiumRibbon) &&
            HasItem(Thermite))
        {
            items.Clear();
            // Crear el objeto nuevo
            items.Add(FinalItem);

            Debug.Log("=================================");
            Debug.Log("LAS 3 PIEZAS SE HAN COMBINADO");
            Debug.Log("HAS CREADO LA LLAVE FINAL");
            Debug.Log("=================================");

            if (onItemChangedCallback != null)
                onItemChangedCallback.Invoke();//actualizar la UI

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