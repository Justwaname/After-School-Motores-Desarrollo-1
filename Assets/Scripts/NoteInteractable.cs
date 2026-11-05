using UnityEngine;

public class NoteInteractable : InteractableObject
{
    [Header("Contenido de la Nota")]
    [SerializeField] private string noteTitle = "Nota misteriosa";

    [Header("Usa {CODE} para mostrar la clave del candado")]
    [SerializeField]
    [TextArea(3, 10)]
    private string noteContent = "El código de seguridad del almacén es: {CODE}";

    [SerializeField] private bool consumeOnRead = false; // Si la nota desaparece al leerse

    [Header("Referencias de UI")]
    [SerializeField] private NoteUI noteUI;

    private PlayerController playerController;
    private SphereCastInteractor sphereInteractor;

    private bool previousPlayerControllerState;
    private bool previousSphereInteractorState;
    private bool previousCursorVisible;
    private CursorLockMode previousCursorLockState;

    private bool panelAbierto = false;

    protected override void Awake()
    {
        base.Awake(); // Mantiene el sistema de brillo/emisión

        if (noteUI != null)
        {
            noteUI.onNoteClosed.AddListener(OnNoteClosed);
        }
    }

    private void OnDestroy()
    {
        if (noteUI != null)
        {
            noteUI.onNoteClosed.RemoveListener(OnNoteClosed);
        }
    }

    public override void Interact()
    {
        if (panelAbierto) return;

        if (noteUI == null)
        {
            Debug.LogError("Asigna el componente NoteUI en la nota.");
            return;
        }

        // Procesamos el contenido para inyectar la clave del Padlock si existe la etiqueta {CODE}
        string finalContent = noteContent;
        if (finalContent.Contains("{CODE}"))
        {
            if (Padlock.Instance != null && !string.IsNullOrEmpty(Padlock.Instance.generatedCode))
            {
                finalContent = finalContent.Replace("{CODE}", Padlock.Instance.generatedCode);
            }
            else
            {
                finalContent = finalContent.Replace("{CODE}", "????");
                Debug.LogWarning("Padlock.Instance o generatedCode no están listos todavía.");
            }
        }

        // Control del jugador y cursor
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            sphereInteractor = player.GetComponent<SphereCastInteractor>();

            if (playerController != null)
            {
                previousPlayerControllerState = playerController.enabled;
                playerController.enabled = false;
            }

            if (sphereInteractor != null)
            {
                previousSphereInteractorState = sphereInteractor.enabled;
                sphereInteractor.enabled = false;
            }
        }

        previousCursorVisible = Cursor.visible;
        previousCursorLockState = Cursor.lockState;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        panelAbierto = true;
        noteUI.OpenNote(noteTitle, finalContent);

        Debug.Log("Nota leída: " + noteTitle);
    }

    private void OnNoteClosed()
    {
        if (playerController != null)
        {
            playerController.enabled = previousPlayerControllerState;
        }

        if (sphereInteractor != null)
        {
            sphereInteractor.enabled = previousSphereInteractorState;
        }

        Cursor.visible = previousCursorVisible;
        Cursor.lockState = previousCursorLockState;

        panelAbierto = false;

        if (consumeOnRead)
        {
            Destroy(gameObject);
        }
    }

    public override string GetInteractText()
    {
        if (panelAbierto) return "";
        return "Leer [E]";
    }
}