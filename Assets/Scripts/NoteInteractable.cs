using UnityEngine;

public class NoteInteractable : InteractableObject
{
    [Header("Contenido de la Nota")]
    [SerializeField] private string noteTitle = "Nota misteriosa";
    [SerializeField][TextArea(3, 10)] private string noteContent = "Aquí escribe el texto terrorífico o pistas del juego...";
    [SerializeField] private bool consumeOnRead = false; // Si la nota desaparece al leerse por primera vez

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

        // Guardamos estado del cursor y lo liberamos
        previousCursorVisible = Cursor.visible;
        previousCursorLockState = Cursor.lockState;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        panelAbierto = true;
        noteUI.OpenNote(noteTitle, noteContent);

        // Opcional: Detener el brillo para que no parpadee mientras se lee
        StopPulse();

        Debug.Log("Nota leída: " + noteTitle);
    }

    private void OnNoteClosed()
    {
        // Restauramos los controles del jugador y el cursor
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

        // Si configuraste que la nota desaparezca tras leerse
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