using UnityEngine;

public class ExitDoor : InteractableObject
{
    public float anguloApertura = 90f;
    public float velocidadApertura = 2f;

    [Header("Puerta Gemela")]
    [Tooltip("Arrastra aquí el script ExitDoor de la otra hoja de la puerta.")]
    public ExitDoor otraPuerta;
    [Tooltip("Marcar solo si esta hoja debe girar en dirección contraria a la otra.")]
    public bool invertirDireccion = false;

    private bool puertaAbierta = false;

    private Quaternion rotacionCerrada;
    private Quaternion rotacionAbierta;

    private void Start()
    {
        rotacionCerrada = transform.localRotation;

        // Si esta puerta invierte la dirección, multiplicamos el ángulo por -1
        float anguloFinal = invertirDireccion ? -anguloApertura : anguloApertura;

        rotacionAbierta =
            rotacionCerrada *
            Quaternion.Euler(0f, anguloFinal, 0f);
    }

    private void Update()
    {
        if (puertaAbierta)
        {
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                rotacionAbierta,
                Time.deltaTime * velocidadApertura
            );
        }
    }

    public override void Interact()
    {
        if (puertaAbierta)
            return;

        if (GameManager.Instance == null)
        {
            Debug.LogError("No existe GameManager en la escena.");
            return;
        }

        if (GameManager.Instance.CanEscape())
        {
            if (HorrorSFXManager.Instance != null)
            {
                HorrorSFXManager.Instance.PlaySFX(
                    HorrorSFXEvent.ExitDoor,
                    transform.position
                );
            }

            Debug.Log("¡PUERTA ABIERTA!");
            GameManager.Instance.ShowNotification("¡PUERTA ABIERTA!");

            if (Inventory.Instance != null)
            {
                Inventory.Instance.RemoveItem(Inventory.Instance.GetFinalItem());
                Inventory.Instance.RemoveItem(Inventory.Instance.GetBlowtorch());
            }

            // Abrir esta puerta y ordenar a la otra que también se abra (evitando bucles infinitos)
            AbrirPuertaRemota();
        }
        else
        {
            if (Inventory.Instance != null &&
                Inventory.Instance.HasItem(Inventory.Instance.GetMagnesiumRibbon()) &&
                Inventory.Instance.HasItem(Inventory.Instance.GetThermite()))
            {
                GameManager.Instance.ShowNotification("!Necesitas combinar los items para hacer la termita primero!");
            }
            else
            {
                GameManager.Instance.ShowNotification("La salida esta encadenada con un candado industrial");
            }
        }
    }

    // Método público para abrir esta hoja de forma directa (llamado por la otra puerta o al cumplir la condición)
    public void AbrirPuertaRemota()
    {
        if (puertaAbierta) return;

        puertaAbierta = true;

        // Si tiene una puerta gemela asignada y todavía no está abierta, la abre también
        if (otraPuerta != null && !otraPuerta.puertaAbierta)
        {
            otraPuerta.AbrirPuertaRemota();
        }
    }

    public override string GetInteractText()
    {
        if (GameManager.Instance != null && GameManager.Instance.CanEscape())
        {
            return "Salida [E] para Escapar";
        }

        if (Inventory.Instance != null)
        {
            bool hasMagnesium = Inventory.Instance.HasItem(Inventory.Instance.GetMagnesiumRibbon());
            bool hasThermite = Inventory.Instance.HasItem(Inventory.Instance.GetThermite());

            if (hasMagnesium && hasThermite)
            {
                return "¿Seguro que tienes todo? [E]";
            }
        }

        return "Salida [Locked]";
    }
}