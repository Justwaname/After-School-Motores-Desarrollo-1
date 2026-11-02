using UnityEngine;

public class NotePickup : InteractableObject
{
    [Header("Información de la nota")]
    [SerializeField] private string noteID = "Nota01";
    [SerializeField] private string noteTitle = "Nota encontrada";

    [TextArea(5, 15)]
    [SerializeField]
    private string noteContent =
        "Escribe aquí el contenido de la nota.";

    public override void Interact()
    {
        if (Inventory.Instance == null)
        {
            Debug.LogError("No existe un Inventory en la escena.");
            return;
        }

        bool added = Inventory.Instance.AddNote(
            noteID,
            noteTitle,
            noteContent
        );

        if (!added)
            return;

        Debug.Log("Has recogido la nota: " + noteTitle);

        if (HorrorSFXManager.Instance != null)
        {
            HorrorSFXManager.Instance.PlaySFX(
                HorrorSFXEvent.ItemPickup,
                transform.position
            );
        }

        Destroy(gameObject);
    }

    public override string GetInteractText()
    {
        return "E - Recoger nota: " + noteTitle;
    }
}