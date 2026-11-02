using UnityEngine;
using UnityEngine.InputSystem;

public class Padlockitem : MonoBehaviour
{
    [SerializeField] private GameObject padlockPanel;

    private bool playerInside;

    private void Start()
    {
        if (padlockPanel == null)
        {
            Debug.LogError("Padlock Panel no asignado en Padlockitem.");
        }
    }

    private void Update()
    {
        if (!playerInside || Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("Tecla E detectada.");

            if (padlockPanel != null)
            {
                padlockPanel.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") ||
            other.transform.root.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") ||
            other.transform.root.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}
