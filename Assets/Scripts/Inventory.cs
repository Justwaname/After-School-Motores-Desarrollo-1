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

    private List<string> items = new List<string>();
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

    // AGREGAR OBJETOS Y PIEZAS
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

    // AGREGAR UNA NOTA AL INVENTARIO
    public bool AddNote(string noteID, string title, string content)
    {
        if (HasNote(noteID))
        {
            Debug.Log("Esta nota ya está guardada: " + title);
            return false;
        }

        InventoryNote newNote =
            new InventoryNote(noteID, title, content);

        notes.Add(newNote);

        Debug.Log("Nota guardada: " + title);

        return true;
    }

    // COMPROBAR SI YA TENEMOS UNA NOTA
    public bool HasNote(string noteID)
    {
        return notes.Exists(note => note.id == noteID);
    }

    // OBTENER UNA NOTA POR SU ID
    public InventoryNote GetNote(string noteID)
    {
        return notes.Find(note => note.id == noteID);
    }

    // OBTENER TODAS LAS NOTAS
    public List<InventoryNote> GetAllNotes()
    {
        return new List<InventoryNote>(notes);
    }

    // COMBINAR LAS TRES PIEZAS
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

            Debug.Log("¡LAS 3 PIEZAS SE HAN COMBINADO!");
            Debug.Log("¡HAS CONSEGUIDO LA LLAVE FINAL!");

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