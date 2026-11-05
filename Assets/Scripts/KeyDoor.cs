using UnityEngine;

public class KeyDoor : InteractableObject
{
    [Header("Configuración de la Llave")]
    [Tooltip("El ItemData de la llave necesaria para abrir esta puerta.")]
    [SerializeField] private ItemData requiredKey;
    [SerializeField] private bool consumeKeyOnOpen = true; // Si la llave desaparece al usarse

    [Header("Parámetros de Apertura")]
    [SerializeField] private float anguloApertura = 90f;
    [SerializeField] private float velocidadApertura = 2f;
    [SerializeField] private bool invertirDireccion = false;

    [Header("Puerta Gemela (Opcional)")]
    [Tooltip("Arrastra aquí el KeyDoorInteractable de la otra hoja si es una puerta doble.")]
    [SerializeField] private KeyDoor otraPuerta;

    private bool puertaAbierta = false;
    private Quaternion rotacionCerrada;
    private Quaternion rotacionAbierta;

    protected override void Awake()
    {
        base.Awake(); // Necesario para mantener el sistema de pulso/outline de la clase padre

        rotacionCerrada = transform.localRotation;
        float anguloFinal = invertirDireccion ? -anguloApertura : anguloApertura;
        rotacionAbierta = rotacionCerrada * Quaternion.Euler(0f, anguloFinal, 0f);
    }

    private void Update()
    {
        if (puertaAbierta)
        {
            transform.localRotation = Quaternion.RotateTowards(
                transform.localRotation,
                rotacionAbierta,
                velocidadApertura * 100f * Time.deltaTime
            );
        }
    }

    public override void Interact()
    {
        if (puertaAbierta) return;

        if (requiredKey == null)
        {
            Debug.LogError($"La puerta {gameObject.name} no tiene asignada una llave (requiredKey).");
            return;
        }

        if (Inventory.Instance == null)
        {
            Debug.LogError("No se encontró un Inventory en la escena.");
            return;
        }

        // Comprobamos si el jugador tiene la llave en su inventario
        if (Inventory.Instance.HasItem(requiredKey))
        {
            // Si configuraste que se consuma, la removemos del inventario
            if (consumeKeyOnOpen)
            {
                Inventory.Instance.RemoveItem(requiredKey);
            }

            Debug.Log($"¡Puerta abierta con éxito usando {requiredKey.name}!");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ShowNotification("¡Puerta abierta!");
            }

            AbrirPuertaRemota();
        }
        else
        {
            // No tiene la llave
            if (HorrorSFXManager.Instance != null)
            {
                HorrorSFXManager.Instance.PlaySFX(HorrorSFXEvent.LockerOpen, transform.position);
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ShowNotification("Está cerrada con llave. Necesitas encontrarla.");
            }

            Debug.Log("La puerta está cerrada. Falta la llave: " + requiredKey.name);
        }
    }

    // Método para abrir la puerta (y propagarlo a su hoja gemela si la tiene)
    public void AbrirPuertaRemota()
    {
        if (puertaAbierta) return;

        puertaAbierta = true;

        // AQUÍ DETENEMOS EL PULSO AL ABRIRSE
        StopPulse();

        if (HorrorSFXManager.Instance != null)
        {
            HorrorSFXManager.Instance.PlaySFX(HorrorSFXEvent.Door, transform.position);
        }

        if (otraPuerta != null && !otraPuerta.puertaAbierta)
        {
            otraPuerta.AbrirPuertaRemota();
        }
    }

    public override string GetInteractText()
    {
        if (puertaAbierta) return "";

        if (Inventory.Instance != null && requiredKey != null && Inventory.Instance.HasItem(requiredKey))
        {
            return "Abrir [E]";
        }

        return "Cerrado con llave [Locked]";
    }
}
