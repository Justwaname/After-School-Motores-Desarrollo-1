using UnityEngine;

public class PadlockInteractable : InteractableObject
{
    [Header("Interfaz del candado")]
    [SerializeField] private PadlockUI padlockUI;

    [Header("Puerta deslizante asociada")]
    [Tooltip("Arrastra aquí la puerta no interactuable que se abrirá al resolver el candado.")]
    [SerializeField] private SlideDoor targetSlideDoor;

    private PlayerController playerController;
    private SphereCastInteractor sphereInteractor;

    private bool previousPlayerControllerState;
    private bool previousSphereInteractorState;
    private bool previousCursorVisible;
    private CursorLockMode previousCursorLockState;

    private bool panelAbierto = false;
    private bool isOpen = false;

    protected override void Awake()
    {
        base.Awake();

        if (padlockUI != null)
        {
            // Nos suscribimos a cuando se cierra el panel (para restaurar controles/cursor)
            padlockUI.onPanelClosed.AddListener(OnPanelClosed);

            // Nos suscribimos al evento de éxito del candado para abrir la puerta automáticamente
            padlockUI.onAccessGranted.AddListener(OnAccessGranted);
        }
    }

    private void OnDestroy()
    {
        if (padlockUI != null)
        {
            padlockUI.onPanelClosed.RemoveListener(OnPanelClosed);
            padlockUI.onAccessGranted.RemoveListener(OnAccessGranted);
        }
    }

    public override void Interact()
    {
        if (isOpen || panelAbierto)
            return;

        if (padlockUI == null)
        {
            Debug.LogError("Asigna PadlockUI en el candado.");
            return;
        }

        if (Padlock.Instance == null)
        {
            Debug.LogError("No existe PadlockSystem.");
            return;
        }

        GameObject player = GameObject.Find("Player");

        if (player == null)
        {
            Debug.LogError("No se encontró al jugador.");
            return;
        }

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

        previousCursorVisible = Cursor.visible;
        previousCursorLockState = Cursor.lockState;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        panelAbierto = true;
        padlockUI.OpenPanel();

        Debug.Log("Panel de candado numérico abierto.");
    }

    // Este método se ejecuta automáticamente cuando el usuario acierta el código (gracias a onAccessGranted)
    private void OnAccessGranted()
    {
        if (isOpen) return;
        isOpen = true;

        // 1. Detenemos el brillo / pulso del candado para que deje de parpadear
        StopPulse();


        // 2. Activamos el desplazamiento de la puerta hacia la izquierda
        if (targetSlideDoor != null)
        {
            targetSlideDoor.OpenSlideDoor();
            Debug.Log("¡Código correcto! La puerta se desplaza hacia la izquierda.");
        }
        else
        {
            Debug.LogWarning("El PadlockInteractable no tiene asignada una SlideDoor.");
        }
    }

    private void OnPanelClosed()
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
    }

    public override string GetInteractText()
    {
        if (isOpen) return "";
        return gameObject.name + " [E] to unlock";
    }
}