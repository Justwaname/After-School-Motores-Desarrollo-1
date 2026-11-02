using UnityEngine;

public class PadlockInteractable : InteractableObject
{
    [Header("Interfaz del candado")]
    [SerializeField] private PadlockUI padlockUI;

    private PlayerController playerController;
    private SphereCastInteractor sphereInteractor;

    private bool previousPlayerControllerState;
    private bool previousSphereInteractorState;
    private bool previousCursorVisible;
    private CursorLockMode previousCursorLockState;

    private bool panelAbierto = false;

    private void Awake()
    {
        if (padlockUI != null)
        {
            padlockUI.onPanelClosed.AddListener(OnPanelClosed);
        }
    }

    private void OnDestroy()
    {
        if (padlockUI != null)
        {
            padlockUI.onPanelClosed.RemoveListener(OnPanelClosed);
        }
    }

    public override void Interact()
    {
        if (panelAbierto)
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

        Debug.Log("Candado numérico abierto.");
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
        return "E - Introducir código";
    }
}