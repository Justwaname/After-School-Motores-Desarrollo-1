using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PadlockUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private GameObject padlockPanel;
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private int maxDigits = 4;

    [Header("Eventos")]
    public UnityEvent onAccessGranted;
    public UnityEvent onAccessDenied;
    public UnityEvent onPanelClosed;

    private string currentInput = "";
    private bool inputLocked = false;

    private void Start()
    {
        if (displayText == null || padlockPanel == null)
        {
            Debug.LogError(
                "Asigna Padlock Panel y Display Text en PadlockUI."
            );
            enabled = false;
            return;
        }

        if (Padlock.Instance == null)
        {
            Debug.LogError("No existe Padlock en la escena.");
            enabled = false;
            return;
        }

        maxDigits = Padlock.Instance.CodeLength;

        
    }

    // ABRIR EL TECLADO
    public void OpenPanel()
    {
        if (padlockPanel == null)
            return;

        if (HorrorSFXManager.Instance != null)
        {
            HorrorSFXManager.Instance.PlaySFX(
                HorrorSFXEvent.UINotification,
                transform.position
            );
        }

        ClearDisplay();
        padlockPanel.SetActive(true);

        // CONGELAR ENEMIGO
        EnemyIA enemy = Object.FindFirstObjectByType<EnemyIA>();
        if (enemy != null)
        {
            enemy.PauseEnemy();
        }
    }

    // CERRAR EL TECLADO
    public void ClosePanel()
    {
        if (padlockPanel == null || !padlockPanel.activeSelf)
            return;

        CancelInvoke(nameof(ClearDisplay));

        currentInput = "";
        inputLocked = false;

        padlockPanel.SetActive(false);

        // REANUDAR ENEMIGO
        EnemyIA enemy = Object.FindFirstObjectByType<EnemyIA>();
        if (enemy != null)
        {
            enemy.ResumeEnemy();
        }

        onPanelClosed?.Invoke();
        ClearDisplay();
    }

    // INTRODUCIR NÚMEROS
    public void PressNumber(string number)
    {
        if (inputLocked || string.IsNullOrEmpty(number))
            return;

        if (number.Length != 1 ||
            number[0] < '0' || number[0] > '9')
            return;

        if (HorrorSFXManager.Instance != null)
        {
            HorrorSFXManager.Instance.PlaySFX(
                HorrorSFXEvent.UINotification,
                transform.position
            );
        }

        if (currentInput.Length < maxDigits)
        {
            currentInput += number;
            displayText.text = currentInput;
        }
    }

    // LIMPIAR PANTALLA
    public void ClearDisplay()
    {
        CancelInvoke(nameof(ClearDisplay));

        if (HorrorSFXManager.Instance != null)
        {
            HorrorSFXManager.Instance.PlaySFX(
                HorrorSFXEvent.UINotification,
                transform.position
            );
        }

        currentInput = "";
        inputLocked = false;

        if (displayText != null)
        {
            displayText.text = new string('-', maxDigits);
        }
    }

    // COMPROBAR CÓDIGO
    public void SubmitCode()
    {
        if (inputLocked || currentInput.Length != maxDigits)
            return;

        if (HorrorSFXManager.Instance != null)
        {
            HorrorSFXManager.Instance.PlaySFX(
                HorrorSFXEvent.UINotification,
                transform.position
            );
        }

        if (Padlock.Instance == null)
        {
            Debug.LogError("No existe Padlock.");
            return;
        }

        bool isCorrect = Padlock.Instance.VerifyCode(currentInput);

        if (isCorrect)
        {
            displayText.text = "OPEN";
            inputLocked = true;

            Debug.Log("¡CANDADO DESBLOQUEADO!");

            onAccessGranted?.Invoke();
            Invoke("ClosePanel", 1f); 
        }
        else
        {
            displayText.text = "ERROR";
            inputLocked = true;

            Debug.Log("Código incorrecto.");

            onAccessDenied?.Invoke();

            Invoke(nameof(ClearDisplay), 1f);
        }
    }
}