using UnityEngine;

public class DoorController : InteractableObject
{
    [Header("Puerta")]
    public float anguloApertura = 90f;
    public float velocidadApertura = 2f;

    private bool puertaAbierta = false;

    private Quaternion rotacionCerrada;
    private Quaternion rotacionAbierta;

    private void Start()
    {
        rotacionCerrada = transform.localRotation;

        rotacionAbierta =
            rotacionCerrada *
            Quaternion.Euler(0f, anguloApertura, 0f);
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

        if (Inventory.Instance == null)
        {
            Debug.LogError("No existe Inventory en la escena.");
            return;
        }

        if (Inventory.Instance.HasItem("LlaveFinal"))
        {
            Debug.Log("¡Llave correcta!");

            puertaAbierta = true;
        }
        else
        {
            Debug.Log("La puerta está cerrada.");
            Debug.Log("Necesitas encontrar las 3 piezas.");
        }
    }
}