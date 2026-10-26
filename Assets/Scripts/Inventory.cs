using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryNote
{
    public string id;
    public string title;

    [TextArea(3, 10)]
    public string content;

    public InventoryNote(string noteID, string noteTitle, string noteContent)
    {
        id = noteID;
        title = noteTitle;
        content = noteContent;
    }
}

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    // --- LÓGICA DE OBJETOS ---
    private List<ItemData> items = new List<ItemData>();
    public IReadOnlyList<ItemData> Items => items;

    [SerializeField] private ItemData Blowtorch;
    [SerializeField] private ItemData MagnesiumRibbon;
    [SerializeField] private ItemData Thermite;
    [SerializeField] private ItemData FinalItem;

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    // --- LÓGICA DE NOTAS ---
    private List<InventoryNote> notes = new List<InventoryNote>();

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

    // --- AÑADIR OBJETO AL INVENTARIO ---
    public bool AddItem(ItemData item)
    {
        if (items.Contains(item))
        {
            return false;
        }

        items.Add(item);
        Debug.Log("Objeto obtenido: " + item.name);

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();

        return true;
    }

    // --- COMPROBAR SI TENEMOS UN OBJETO ---
    public bool HasItem(ItemData item)
    {
        return items.Contains(item);
    }

    // --- ELIMINAR OBJETO ---
    public bool RemoveItem(ItemData item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            if (onItemChangedCallback != null)
                onItemChangedCallback.Invoke();
            return true;
        }
        return false;
    }

    // --- OBTENER REFERENCIAS ---
    public ItemData GetBlowtorch() => Blowtorch;
    public ItemData GetMagnesiumRibbon() => MagnesiumRibbon;
    public ItemData GetThermite() => Thermite;
    public ItemData GetFinalItem() => FinalItem;

    // --- COMBINAR (Evita duplicados si ya tenemos el FinalItem) ---
    public void CheckCombination()
    {
        // Si ya tenemos el ítem final, no permitimos volver a combinar
        if (HasItem(FinalItem))
        {
            Debug.Log("Ya tienes el ítem final creado.");
            return;
        }

        // Verificamos que tenga los dos ingredientes necesarios
        if (HasItem(MagnesiumRibbon) && HasItem(Thermite))
        {
            RemoveItem(MagnesiumRibbon);
            RemoveItem(Thermite);

            items.Add(FinalItem);

            Debug.Log("=================================");
            Debug.Log("¡TERMITA Y MAGNESIO COMBINADOS!");
            Debug.Log("=================================");

            if (HorrorSFXManager.Instance != null)
            {
                HorrorSFXManager.Instance.PlaySFX(
                    HorrorSFXEvent.KeyCombined,
                    transform.position
                );
            }

            if (onItemChangedCallback != null)
                onItemChangedCallback.Invoke();
        }
        else
        {
            Debug.Log("No tienes los ingredientes necesarios para combinar (Thermite + MagnesiumRibbon).");
        }
    }

    // ==========================================
    // --- SISTEMA DE NOTAS ---
    // ==========================================

    public bool AddNote(string noteID, string title, string content)
    {
        if (HasNote(noteID)) return false;
        notes.Add(new InventoryNote(noteID, title, content));
        return true;
    }

    public bool HasNote(string noteID) => notes.Exists(note => note.id == noteID);
    public InventoryNote GetNote(string noteID) => notes.Find(note => note.id == noteID);
    public List<InventoryNote> GetAllNotes() => new List<InventoryNote>(notes);
}